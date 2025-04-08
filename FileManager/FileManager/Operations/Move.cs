﻿using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Server.Loggers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server.Operations
{
    public class Move : StepOperation
    {
        public Move(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext appDbContext)
            : base(step, operation, taskLogger, appDbContext)
        {
        }

        public override void Execute(List<string>? bufferFiles)
        {
            _taskLogger.StepLog(TaskStep, $"Перемещение: {TaskStep.Source} => {TaskStep.Destination}");
            _taskLogger.OperationLog(TaskStep);

            string[] files = [];
            string fileNameDestination, fileName;
            bool isMoveFile = true;
			List<FileInfo> infoFiles = new List<FileInfo>();
            OperationMoveEntity? operation = null;

			if (TaskStep.FileMask == "{BUFFER}")
			{
				if (bufferFiles != null)
				{
					foreach (var file in bufferFiles)
					{
						infoFiles.Add(new FileInfo(file));
					}
				}
			}
			else
			{
				files = Directory.GetFiles(TaskStep.Source, TaskStep.FileMask);
				foreach (var file in files)
				{
					infoFiles.Add(new FileInfo(file));
				}
			}
			_taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count()}");

            if (infoFiles.Count > 0)
            {
				operation = _appDbContext.OperationMove.FirstOrDefault(x => x.StepId == TaskStep.StepId);
				// список файлов с атрибутами

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
			}
            

            
            bool isOverwriteFile = false;
            foreach (var file in infoFiles)
            {
                FileAttributes attributs = File.GetAttributes(file.FullName);
                fileName = Path.GetFileName(file.FullName);
				isMoveFile = true;

				if (operation != null)
                {
                    // дубль по журналу
                    TaskLogEntity? taskLogs = _appDbContext.TaskLog.FirstOrDefault(x => x.StepId == TaskStep.StepId &&
                                                                                    x.FileName == fileName);
                    if (taskLogs != null)
                    {
                        if (operation.FileInLog)
                        {
                            isMoveFile = false;
                        }
                        else
                        {
                            isMoveFile = true;
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
                            isMoveFile = false;
                            if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                isMoveFile = true;
                            }
                            break;
                        case AttributeFile.A:
                            isMoveFile = false;
                            if ((attributs & FileAttributes.Compressed) == FileAttributes.Compressed)
                            {
                                isMoveFile = true;
                            }
                            break;
                        case AttributeFile.R:
                            isMoveFile = false;
                            if ((attributs & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                            {
                                isMoveFile = true;
                            }
                            break;
                        case AttributeFile.X:
                            isMoveFile = true;
                            break;
                        case AttributeFile.V:
                            isMoveFile = false;
                            if ((attributs & FileAttributes.Archive) == FileAttributes.Archive)
                            {
                                isMoveFile = true;
                            }
                            if ((attributs & FileAttributes.Hidden) == FileAttributes.Hidden)
                            {
                                isMoveFile = false;
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (isMoveFile)
                {
                    fileNameDestination = Path.Combine(TaskStep.Destination, fileName);
                    FileInfo destinationFileInfo = new FileInfo(fileNameDestination);
                    if (destinationFileInfo.Exists && destinationFileInfo.IsReadOnly && isOverwriteFile)
                    {
                        destinationFileInfo.IsReadOnly = false;
                        File.Move(file.FullName, fileNameDestination, isOverwriteFile);
                        _taskLogger.StepLog(TaskStep, "Файл успешно перемещён", fileName);
                        destinationFileInfo.IsReadOnly = true;
                    }
					else if ((destinationFileInfo.Exists && isOverwriteFile) || !destinationFileInfo.Exists)
					{
                        File.Move(file.FullName, fileNameDestination, isOverwriteFile);
                        _taskLogger.StepLog(TaskStep, "Файл успешно перемещён", fileName);
                    }
                }
            }

            if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }
    }
}
