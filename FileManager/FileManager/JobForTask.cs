using FileManager.DAL;
using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using Quartz;
using System.IO;
using System.Text.RegularExpressions;

//using System.Xml.Linq;
using System.Xml;

namespace FileManager_Server
{

	[DisallowConcurrentExecution]
	public class JobForTask : IJob
	{

		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<JobForTask> _logger;
		//private readonly DoSomething _doSomething;
		private readonly TaskOperationService _taskOperationService;
		private readonly ITaskLogger _taskLogger;

		public JobForTask(IServiceProvider serviceProvider, ILogger<JobForTask> logger, TaskOperationService taskOperationService, ITaskLogger taskLogger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
			_taskOperationService = taskOperationService;
			_taskLogger = taskLogger;
		}



		public async Task Execute(IJobExecutionContext context)
		{
			// log - 
			// log - начало выполнения задачи

			_taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Начало работы задачи {context.JobDetail.Key.Name} >>>");

			if (context.RefireCount > 5)
			{
				_logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - RefireCount > 5");
			}
			try
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
					{
						if (dbContext == null)
						{
							throw new ArgumentNullException(nameof(dbContext));
						}


						TaskEntity? taskEntity = dbContext.Task.First(x => x.TaskId == context.JobDetail.Key.Name);



						if (taskEntity is null)
						{
							throw new ArgumentNullException(nameof(taskEntity));
						}
						List<TaskStepEntity> taskSteps = dbContext.TaskStep.Where(x => x.TaskId == taskEntity.TaskId).OrderBy(x => x.StepNumber).ToList();

						List<ITaskStep> steps = new List<ITaskStep>();
						int i = 0;
						TaskOperation? operation;
						foreach (var step in taskSteps)
						{
							if (step.IsActive)
							{
								switch (step.OperationName)
								{
									case OperationName.Copy:
										operation = dbContext.OperationCopy.First(x => x.StepId == step.StepId);
										CreatorFactoryMethod copyCreator = new CopyCreator();
										steps.Add(copyCreator.FactoryMethod(step, operation, _serviceProvider, _taskLogger));
										break;
									case OperationName.Move:
										operation = dbContext.OperationMove.First(x => x.StepId == step.StepId);
										CreatorFactoryMethod moveCreator = new MoveCreator();
										steps.Add(moveCreator.FactoryMethod(step, operation, _serviceProvider, _taskLogger));
										break;
									case OperationName.Read:
										operation = dbContext.OperationRead.First(x => x.StepId == step.StepId);
										CreatorFactoryMethod readCreator = new ReadCreator();
										steps.Add(readCreator.FactoryMethod(step, operation, _serviceProvider, _taskLogger));
										break;
									case OperationName.Exist:
										break;
									case OperationName.Rename:
										break;
									case OperationName.Delete:
										break;
									default:
										break;
								}
								if (i != 0)
								{
									steps[i - 1].SetNext(steps[i]);
								}

								i++;
							}


						}
						if (steps.Count > 0)
						{
							steps[0].Execute();
						}


					}
				}
				await Task.CompletedTask;
				// log - завершение работы задачи
				_taskLogger.TaskLog(context.JobDetail.Key.Name, $"<<< Окончание работы задачи {context.JobDetail.Key.Name} >>>");

			}
			catch (Exception ex)
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					try
					{

						var service = scope.ServiceProvider.GetService<AppDbContext>();
						if (service != null)
						{
							TaskStatusEntity status = service.TaskStatuse.First(x => x.TaskId == context.JobDetail.Key.Name);
							if (status != null)
							{
								status.IsProgress = false;
								status.IsError = true;
								status.DateLastExecute = DateTime.Now;
								service.TaskStatuse.Update(status);
								service.SaveChanges();
							}
						}
						_logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex.Message}");
					}
					catch (Exception ex2)
					{
						_logger.LogError($"{DateTime.Now} задача: {context.JobDetail.Key.Name} - {ex2.Message}");
					}

				}
				throw new JobExecutionException(msg: "", refireImmediately: true, cause: ex);
			}
		}


	}















	public interface ITaskStep
	{
		void SetNext(ITaskStep nextStep);
		void Execute();
	}


	public abstract class TaskStep : ITaskStep
	{
		protected ITaskStep _nextStep;

		public TaskStepEntity TaskStepEntity { get; set; }

		public TaskOperation TaskOperation { get; set; }

		protected readonly IServiceProvider _serviceProvider;
		protected readonly ITaskLogger _taskLogger;

		public TaskStep(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
		{
			TaskStepEntity = step;
			TaskOperation = operation;
			_serviceProvider = serviceProvider;
			_taskLogger = taskLogger;
		}

		public abstract void Execute();

		public void SetNext(ITaskStep nextStep)
		{
			_nextStep = nextStep;
		}
	}


	public class Copy : TaskStep
	{

		public Copy(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
			: base(step, operation, serviceProvider, taskLogger)
		{

		}


		public override void Execute()
		{
			_taskLogger.StepLog(TaskStepEntity, $"Копирование: {TaskStepEntity.Source} => {TaskStepEntity.Destination}");
			_taskLogger.OperationLog(TaskStepEntity);

			using (var scope = _serviceProvider.CreateScope())
			{
				using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
				{
					if (dbContext == null)
					{
						throw new ArgumentNullException(nameof(dbContext));
					}


					string[] files = [];
					string fileNameDestination, fileName;
					bool isCopyFile = true;
					List<FileInformation> filesSet = new();
					List<FileInformation> sortedFilesSet = new();



					files = Directory.GetFiles(TaskStepEntity.Source, TaskStepEntity.FileMask);
					_taskLogger.StepLog(TaskStepEntity, $"Количество найденный файлов по маске '{TaskStepEntity.FileMask}': {files.Count()}");
					OperationCopyEntity operation = dbContext.OperationCopy.First(x => x.StepId == TaskStepEntity.StepId);

					// список файлов с атрибутами
					List<FileInfo> infoFiles = new List<FileInfo>();
					foreach (var file in files)
					{
						infoFiles.Add(new FileInfo(file));
					}

					if (operation != null)
					{
						// сортировка
						switch (operation.Sort)
						{
							case SortFiles.NoSortFiles:
								break;
							case SortFiles.NameAscending:
								infoFiles = infoFiles.OrderBy(o => o.Name).ToList();
								break;
							case SortFiles.NameDescending:
								infoFiles = infoFiles.OrderByDescending(o => o.Name).ToList();
								break;
							case SortFiles.TimeAscending:
								infoFiles = infoFiles.OrderBy(o => o.CreationTime).ToList();
								break;
							case SortFiles.TimeDescending:
								infoFiles = infoFiles.OrderByDescending(o => o.CreationTime).ToList();
								break;
							case SortFiles.SizeAscending:
								infoFiles = infoFiles.OrderBy(o => o.Length).ToList();
								break;
							case SortFiles.SizeDescending:
								infoFiles = infoFiles.OrderByDescending(o => o.Length).ToList();
								break;
							default:
								break;
						}


						// макс файлов
						if (operation.FilesForProcessing != 0 & operation.FilesForProcessing < infoFiles.Count - 2)
						{
							infoFiles.RemoveRange(operation.FilesForProcessing, infoFiles.Count - 2);
						}


					}
					bool isOverwriteFile = false;
					foreach (string file in files)
					{
						FileAttributes attributs = File.GetAttributes(file);


						fileName = Path.GetFileName(file);

						if (operation != null)
						{
							isCopyFile = true;

							// дубль по журналу и файл в источнике
							TaskLogEntity taskLogs = dbContext.TaskLog.FirstOrDefault(x => x.StepId == TaskStepEntity.StepId &&
																							x.FileName == fileName);
							if (taskLogs != null)
							{
								if (operation.FileInSource == FileInSource.OneDay && operation.FileInLog == true)
								{
									isCopyFile = false;
								}
								else if (operation.FileInSource == FileInSource.Always && operation.FileInLog == true)
								{
									// stop task
								}
								else if (operation.FileInSource == FileInSource.OneDay && operation.FileInLog == false)
								{
									isCopyFile = false;
								}
							}




							// файл в назначении

							if (operation.FileInDestination == FileInDestination.OVR)
							{
								isOverwriteFile = true;
							}
							else if (operation.FileInDestination == FileInDestination.RNM)
							{
								isOverwriteFile = true;
							}
							else if (operation.FileInDestination == FileInDestination.ERR)
							{
								isOverwriteFile = false;
							}



							// атрибуты

							switch (operation.FileAttribute)
							{
								case AttributeFile.H:
									isCopyFile = false;
									if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
									{
										isCopyFile = true;
									}
									break;
								case AttributeFile.A:
									isCopyFile = false;
									if ((attributs & FileAttributes.Compressed) == FileAttributes.Compressed)
									{
										isCopyFile = true;
									}
									break;
								case AttributeFile.R:
									isCopyFile = false;
									if ((attributs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
									{
										isCopyFile = true;
									}
									break;
								case AttributeFile.X:
									isCopyFile = true;
									break;
								case AttributeFile.V:
									isCopyFile = false;
									if ((attributs & FileAttributes.Archive) == FileAttributes.Archive)
									{
										isCopyFile = true;
									}
									if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
									{
										isCopyFile = false;
									}
									break;
								default:
									break;
							}

						}
						else
						{
							isCopyFile = true;
						}

						if (isCopyFile)
						{
							fileNameDestination = Path.Combine(TaskStepEntity.Destination, fileName);
							FileInfo destinationFileInfo = new FileInfo(fileNameDestination);
							if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
							{
								destinationFileInfo.IsReadOnly = false;
								File.Copy(file, fileNameDestination, isOverwriteFile);
								_taskLogger.StepLog(TaskStepEntity, "Файл успешно скопирован", fileName);
								destinationFileInfo.IsReadOnly = true;
							}
							else
							{
								File.Copy(file, fileNameDestination, isOverwriteFile);
								_taskLogger.StepLog(TaskStepEntity, "Файл успешно скопирован", fileName);
							}


							// log - результат операции над конкретным файлом: успешно или нет


						}

					}

				}
			}




			if (_nextStep != null)
			{
				_nextStep.Execute();
			}

		}
	}

	public class Move : TaskStep
	{
		public Move(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
			: base(step, operation, serviceProvider, taskLogger)
		{
		}

		public override void Execute()
		{
			_taskLogger.StepLog(TaskStepEntity, $"Перемещение: {TaskStepEntity.Source} => {TaskStepEntity.Destination}");
			_taskLogger.OperationLog(TaskStepEntity);

			using (var scope = _serviceProvider.CreateScope())
			{
				using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
				{
					if (dbContext == null)
					{
						throw new ArgumentNullException(nameof(dbContext));
					}


					string[] files = [];
					string fileNameDestination, fileName;
					bool isCopyFile = true;
					List<FileInformation> filesSet = new();
					List<FileInformation> sortedFilesSet = new();

					files = Directory.GetFiles(TaskStepEntity.Source, TaskStepEntity.FileMask);
					_taskLogger.StepLog(TaskStepEntity, $"Количество найденный файлов по маске '{TaskStepEntity.FileMask}': {files.Count()}");
					OperationMoveEntity operation = dbContext.OperationMove.First(x => x.StepId == TaskStepEntity.StepId);

					// список файлов с атрибутами
					List<FileInfo> infoFiles = new List<FileInfo>();
					foreach (var file in files)
					{
						infoFiles.Add(new FileInfo(file));
					}

					if (operation != null)
					{
						// сортировка
						switch (operation.Sort)
						{
							case SortFiles.NoSortFiles:
								break;
							case SortFiles.NameAscending:
								infoFiles = infoFiles.OrderBy(o => o.Name).ToList();
								break;
							case SortFiles.NameDescending:
								infoFiles = infoFiles.OrderByDescending(o => o.Name).ToList();
								break;
							case SortFiles.TimeAscending:
								infoFiles = infoFiles.OrderBy(o => o.CreationTime).ToList();
								break;
							case SortFiles.TimeDescending:
								infoFiles = infoFiles.OrderByDescending(o => o.CreationTime).ToList();
								break;
							case SortFiles.SizeAscending:
								infoFiles = infoFiles.OrderBy(o => o.Length).ToList();
								break;
							case SortFiles.SizeDescending:
								infoFiles = infoFiles.OrderByDescending(o => o.Length).ToList();
								break;
							default:
								break;
						}


						// макс файлов
						if (operation.FilesForProcessing != 0 & operation.FilesForProcessing < infoFiles.Count - 2)
						{
							infoFiles.RemoveRange(operation.FilesForProcessing, infoFiles.Count - 2);
						}


					}
					bool isOverwriteFile = false;
					foreach (string file in files)
					{
						FileAttributes attributs = File.GetAttributes(file);


						fileName = Path.GetFileName(file);

						if (operation != null)
						{
							isCopyFile = true;

							// дубль по журналу
							TaskLogEntity taskLogs = dbContext.TaskLog.FirstOrDefault(x => x.StepId == TaskStepEntity.StepId &&
																							x.FileName == fileName);
							if (taskLogs != null)
							{
								if (operation.FileInLog)
								{
									isCopyFile = false;
								}
								else
								{
									isCopyFile = true;
								}
							}





							// файл в назначении

							if (operation.FileInDestination == FileInDestination.OVR)
							{
								isOverwriteFile = true;
							}
							else if (operation.FileInDestination == FileInDestination.RNM)
							{
								isOverwriteFile = true;
							}
							else if (operation.FileInDestination == FileInDestination.ERR)
							{
								isOverwriteFile = false;
							}



							// атрибуты

							switch (operation.FileAttribute)
							{
								case AttributeFile.H:
									isCopyFile = false;
									if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
									{
										isCopyFile = true;
									}
									break;
								case AttributeFile.A:
									isCopyFile = false;
									if ((attributs & FileAttributes.Compressed) == FileAttributes.Compressed)
									{
										isCopyFile = true;
									}
									break;
								case AttributeFile.R:
									isCopyFile = false;
									if ((attributs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
									{
										isCopyFile = true;
									}
									break;
								case AttributeFile.X:
									isCopyFile = true;
									break;
								case AttributeFile.V:
									isCopyFile = false;
									if ((attributs & FileAttributes.Archive) == FileAttributes.Archive)
									{
										isCopyFile = true;
									}
									if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
									{
										isCopyFile = false;
									}
									break;
								default:
									break;
							}

						}
						else
						{
							isCopyFile = true;
						}

						if (isCopyFile)
						{
							fileNameDestination = Path.Combine(TaskStepEntity.Destination, fileName);
							FileInfo destinationFileInfo = new FileInfo(fileNameDestination);
							if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
							{
								destinationFileInfo.IsReadOnly = false;
								File.Move(file, fileNameDestination, isOverwriteFile);
								_taskLogger.StepLog(TaskStepEntity, "Файл успешно перемещён", fileName);
								destinationFileInfo.IsReadOnly = true;
							}
							else
							{
								File.Move(file, fileNameDestination, isOverwriteFile);
								_taskLogger.StepLog(TaskStepEntity, "Файл успешно перемещён", fileName);
							}

						}

					}

				}
			}

			if (_nextStep != null)
			{
				_nextStep.Execute();
			}
		}
	}


	public class Read : TaskStep
	{
		public Read(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
			: base(step, operation, serviceProvider, taskLogger)
		{
		}

		public override void Execute()
		{
			if (_nextStep != null)
			{
				_nextStep.Execute();
			}
		}
	}

	public class Delete : TaskStep
	{
		public Delete(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
			: base(step, operation, serviceProvider, taskLogger)
		{
		}

		public override void Execute()
		{


			_taskLogger.StepLog(TaskStepEntity, $"Копирование: {TaskStepEntity.Source} => {TaskStepEntity.Destination}");
			_taskLogger.OperationLog(TaskStepEntity);

			using (var scope = _serviceProvider.CreateScope())
			{
				using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
				{
					if (dbContext == null)
					{
						throw new ArgumentNullException(nameof(dbContext));
					}


					string[] files = [];
					string fileNameDestination, fileName;
					bool isCopyFile = true;
					List<FileInformation> filesSet = new();
					List<FileInformation> sortedFilesSet = new();



					files = Directory.GetFiles(TaskStepEntity.Source, TaskStepEntity.FileMask);
					_taskLogger.StepLog(TaskStepEntity, $"Количество найденный файлов по маске '{TaskStepEntity.FileMask}': {files.Count()}");
					OperationDeleteEntity operation = dbContext.OperationDelete.First(x => x.StepId == TaskStepEntity.StepId);

					// список файлов с атрибутами
					List<FileInfo> infoFiles = new List<FileInfo>();
					foreach (var file in files)
					{
						infoFiles.Add(new FileInfo(file));
					}

					
					bool isOverwriteFile = false;
					foreach (string file in files)
					{
						FileAttributes attributs = File.GetAttributes(file);


						fileName = Path.GetFileName(file);

						File.Delete(file);
						_taskLogger.StepLog(TaskStepEntity, "Файл успешно удалён", fileName);

					}

				}
			}




			if (_nextStep != null)
			{
				_nextStep.Execute();
			}
		}
	}

	public class Exist : TaskStep
	{
		public Exist(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
			: base(step, operation, serviceProvider, taskLogger)
		{
		}

		public override void Execute()
		{
			if (_nextStep != null)
			{
				_nextStep.Execute();
			}
		}
	}

	public class Rename : TaskStep
	{
		public Rename(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
			: base(step, operation, serviceProvider, taskLogger)
		{
		}

		public override void Execute()
		{
			if (_nextStep != null)
			{
				_nextStep.Execute();
			}
		}
	}







	public abstract class CreatorFactoryMethod
	{
		internal abstract ITaskStep FactoryMethod(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger);
	}

	public class CopyCreator : CreatorFactoryMethod
	{
		internal override ITaskStep FactoryMethod(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
		{
			return new Copy(step, operation, serviceProvider, taskLogger);
		}
	}

	public class MoveCreator : CreatorFactoryMethod
	{
		internal override ITaskStep FactoryMethod(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
		{
			return new Move(step, operation, serviceProvider, taskLogger);
		}
	}
	public class ReadCreator : CreatorFactoryMethod
	{
		internal override ITaskStep FactoryMethod(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
		{
			return new Read(step, operation, serviceProvider, taskLogger);
		}
	}
	public class DeleteCreator : CreatorFactoryMethod
	{
		internal override ITaskStep FactoryMethod(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
		{
			return new Delete(step, operation, serviceProvider, taskLogger);
		}
	}

	public class ExistCreator : CreatorFactoryMethod
	{
		internal override ITaskStep FactoryMethod(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
		{
			return new Exist(step, operation, serviceProvider, taskLogger);
		}
	}

	public class RenameCreator : CreatorFactoryMethod
	{
		internal override ITaskStep FactoryMethod(TaskStepEntity step, TaskOperation operation, IServiceProvider serviceProvider, ITaskLogger taskLogger)
		{
			return new Rename(step, operation, serviceProvider, taskLogger);
		}
	}









}


