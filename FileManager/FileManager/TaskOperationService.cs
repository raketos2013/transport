using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server
{
	public class TaskOperationService
	{
		private readonly ILogger<DoSomething> _logger;
		private readonly IServiceProvider _serviceProvider;
		public TaskOperationService(ILogger<DoSomething> logger, IServiceProvider serviceProvider)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
		}
		public void Copy(TaskStepEntity taskStep)
		{
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

					files = Directory.GetFiles(taskStep.Source, taskStep.FileMask);
					OperationCopyEntity operation = dbContext.OperationCopy.First(x => x.StepId == taskStep.StepId);

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

						switch (operation.FileAttribute)
						{
							case AttributeFile.V:
								break;
							case AttributeFile.H:
								infoFiles.RemoveAll(x => x.Attributes == FileAttributes.Hidden);
								break;
							case AttributeFile.A:
								break;
							case AttributeFile.R:
								break;
							case AttributeFile.X:
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
						fileName = Path.GetFileName(file);

						if (operation != null)
						{
							// дубль по журналу
							isCopyFile = true;
							if (operation.FileInSource == FileInSource.OneDay)
							{
								TaskLogEntity taskLogs = dbContext.TaskLog.First(x => x.StepId == taskStep.StepId &&
																							x.FileName == fileName);
								if (taskLogs != null)
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

							// дубль по журналу
							// атрибуты

							



						}
						else
						{
							isCopyFile = true;
						}

						if (isCopyFile) 
						{
							

							fileNameDestination = Path.Combine(taskStep.Destination, fileName);
							File.Copy(file, fileNameDestination, isOverwriteFile);
						}

					}

				}
			}
		}
		public void Move()
		{

		}
		public void Delete()
		{

		}

		public void Exist()
		{

		}

		public void Rename()
		{

		}

		public void Read()
		{

		}
	}
}
