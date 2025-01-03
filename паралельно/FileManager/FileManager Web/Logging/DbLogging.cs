using Microsoft.Extensions.Options;
using Npgsql;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace FileManager_Web.Logging
{

    public class DbLoggerOptions
    {

        public string DefaultConnection { get; init; }

        public string[] LogFields { get; init; }

        public string LogTable { get; init; }


#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        public DbLoggerOptions()
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
        {
        }

    }


    [ProviderAlias("Database")]
    public class DbLoggerProvider : ILoggerProvider
    {
        public readonly DbLoggerOptions _options;
        public readonly IHttpContextAccessor _contextAccessor;
        public DbLoggerProvider(IOptions<DbLoggerOptions> options, IHttpContextAccessor contextAccessor)
        {
            _options = options.Value; // Stores all the options// .
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Creates a new instance of the db logger.
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName)
        {
            return new DbLogger(this);
        }

        public void Dispose()
        {
        }
    }



    public static class DbLoggerExtensions
    {
        public static ILoggingBuilder AddDbLogger(this ILoggingBuilder builder, Action<DbLoggerOptions> configure)
        {
            builder.Services.AddSingleton<ILoggerProvider, DbLoggerProvider>();
            builder.Services.Configure(configure);
            return builder;
        }
    }






    public class DbLogger : ILogger, IDisposable
    {

        private readonly DbLoggerProvider _dbLoggerProvider;


        public DbLogger(DbLoggerProvider dbLoggerProvider)
        {
            _dbLoggerProvider = dbLoggerProvider;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }
        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Dispose() { }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                // Don't log the entry if it's not enabled.
                return;
            }

            var threadId = Thread.CurrentThread.ManagedThreadId; // Get the current thread ID to use in the log file. 

            var userName = _dbLoggerProvider._contextAccessor?.HttpContext?.User?.Identity?.Name;
            var remoteHost = _dbLoggerProvider._contextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            using (var connection = new NpgsqlConnection(_dbLoggerProvider._options.DefaultConnection))
            {
                connection.Open();

                // Add to database.

                // LogLevel
                // ThreadId
                // EventId
                // Exception Message (use formatter)
                // Exception Stack Trace
                // Exception Source

                var values = new JObject();

                if (_dbLoggerProvider?._options?.LogFields?.Any() ?? false)
                {
                    foreach (var logField in _dbLoggerProvider._options.LogFields)
                    {
                        switch (logField)
                        {
                            case "User":
                                values["User"] = userName?.ToString();
                                break;
                            case "RemoteIP":
                                values["RemoteIP"] = remoteHost;
                                break;
                            case "LogLevel":
                                if (!string.IsNullOrWhiteSpace(logLevel.ToString()))
                                {
                                    values["LogLevel"] = logLevel.ToString();
                                }
                                break;
                            case "ThreadId":
                                values["ThreadId"] = threadId;
                                break;
                            case "EventId":
                                values["EventId"] = eventId.Id;
                                break;
                            case "EventName":
                                if (!string.IsNullOrWhiteSpace(eventId.Name))
                                {
                                    values["EventName"] = eventId.Name;
                                }
                                break;
                            case "Message":
                                if (!string.IsNullOrWhiteSpace(formatter(state, exception)))
                                {
                                    values["Message"] = formatter(state, exception);
                                }
                                break;
                            case "ExceptionMessage":
                                if (exception != null && !string.IsNullOrWhiteSpace(exception.Message))
                                {
                                    values["ExceptionMessage"] = exception?.Message;
                                }
                                break;
                            case "ExceptionStackTrace":
                                if (exception != null && !string.IsNullOrWhiteSpace(exception.StackTrace))
                                {
                                    values["ExceptionStackTrace"] = exception?.StackTrace;
                                }
                                break;
                            case "ExceptionSource":
                                if (exception != null && !string.IsNullOrWhiteSpace(exception.Source))
                                {
                                    values["ExceptionSource"] = exception?.Source;
                                }
                                break;

                        }
                    }
                }

                using (var command = new NpgsqlCommand())
                {
                    var Jsonvalues = @values.ToString(Formatting.None);
                    command.Connection = connection;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = $"INSERT INTO \"{_dbLoggerProvider?._options.LogTable}\" (\"Values\", \"Created\") VALUES (@Values::json, @Created)";

                    var param = (NpgsqlParameter)command.CreateParameter() as NpgsqlParameter;
                    param.ParameterName = "@Values";
                    param.Value = Jsonvalues;

                    param.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Jsonb;
                    command.Parameters.Add(param);
                    command.Parameters.Add(new NpgsqlParameter("@Created", DateTimeOffset.UtcNow));

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

        }

    }



    public class UserLogging
    {

        private readonly ILogger<UserLogging> _logger;
        private readonly string _connectionString;

        public UserLogging(IConfiguration configuration, ILogger<UserLogging> logger)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public void Logging(string username, string action, string data)
        {

            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new NpgsqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = $"INSERT INTO \"UserLogs\" (\"UserName\", \"DateTimeLog\", \"Action\", \"Data\") VALUES (@Username, @DateTimeLog, @Action, @Data::json)";
                        command.Parameters.AddWithValue("@UserName", username);
                        command.Parameters.AddWithValue("@Action", action);
                        command.Parameters.AddWithValue("@Data", data);
                        command.Parameters.AddWithValue("@DateTimeLog", DateTimeOffset.UtcNow);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                Console.WriteLine(e.Message);
            }
        }

    }






}
