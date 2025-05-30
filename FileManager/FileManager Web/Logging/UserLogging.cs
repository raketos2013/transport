using Npgsql;

namespace FileManager_Web.Logging;

public class UserLogging(IConfiguration configuration,
                            ILogger<UserLogging> logger)
    : IUserLogging
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

    public void Logging(string username, string action, string data)
    {
        try
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = $"INSERT INTO \"UserLog\" (\"UserName\", \"DateTimeLog\", \"Action\", \"Data\") VALUES (@Username, @DateTimeLog, @Action, @Data::json)";
                command.Parameters.AddWithValue("@UserName", username);
                command.Parameters.AddWithValue("@DateTimeLog", DateTimeOffset.UtcNow);
                command.Parameters.AddWithValue("@Action", action);
                command.Parameters.AddWithValue("@Data", data);
                command.ExecuteNonQuery();
            }
            connection.Close();

        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            Console.WriteLine(e.Message);
        }
    }

}
