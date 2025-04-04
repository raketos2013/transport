using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
							TaskLogEntity taskLogs = dbContext.TaskLog.FirstOrDefault(x => x.StepId == taskStep.StepId &&
																							x.FileName == fileName);
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
							fileNameDestination = Path.Combine(taskStep.Destination, fileName);
							FileInfo destinationFileInfo = new FileInfo(fileNameDestination);
							if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
							{
								destinationFileInfo.IsReadOnly = false;
								File.Copy(file, fileNameDestination, isOverwriteFile);
								destinationFileInfo.IsReadOnly = true;
							}
							else
							{
								File.Copy(file, fileNameDestination, isOverwriteFile);
							}

						}

					}

				}
			}
		}
		public void Move()
		{
			using (var scope = _serviceProvider.CreateScope())
			{
				using (var dbContext = scope.ServiceProvider.GetService<AppDbContext>())
				{
					if (dbContext == null)
					{
						throw new ArgumentNullException(nameof(dbContext));
					}




				}
			}
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
