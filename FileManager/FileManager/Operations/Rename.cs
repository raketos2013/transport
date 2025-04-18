using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileManager_Server.Operations
{
    public class Rename : StepOperation
    {
        public Rename(TaskStepEntity step, TaskOperation? operation, ITaskLogger taskLogger, AppDbContext appDbContext, IMailSender mailSender)
            : base(step, operation, taskLogger, appDbContext, mailSender)
        { }


        public override void Execute(List<string>? bufferFiles)
        {
            _taskLogger.StepLog(TaskStep, $"ПЕРЕИМЕНОВАНИЕ: {TaskStep.Source}");
            _taskLogger.OperationLog(TaskStep);


            string[] files = [];
            string fileNameDestination, fileName, newFileName;
            bool isCopyFile = true;
            List<FileInfo> infoFiles = new List<FileInfo>();
            List<string> successFiles = new List<string>();
            OperationRenameEntity? operation = null;

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
            List<AddresseeEntity> addresses = new List<AddresseeEntity>();
            _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count()}");

            if (infoFiles.Count > 0)
            {
                operation = _appDbContext.OperationRename.FirstOrDefault(x => x.StepId == TaskStep.StepId);
                if (operation != null)
                {
                    foreach(var file in infoFiles)
                    {
                        fileName = Path.GetFileName(file.FullName);
                        newFileName = RenameFileNew(fileName, operation.OldPattern, operation.NewPattern);
                        FileSystem.Rename(file.FullName, file.DirectoryName + "\\\\" + newFileName);
                    }
                    
                }
            }



            if (_nextStep != null)
            {
                _nextStep.Execute(bufferFiles);
            }
        }

        private string RenameFileNew(string filename, string old_pattern, string new_pattern)
        {
            StringBuilder stringBuilder = new StringBuilder(new_pattern);

            if (Regex.IsMatch(filename, old_pattern, RegexOptions.IgnoreCase))
            {
                Regex regex = new Regex(old_pattern, RegexOptions.IgnoreCase);

                MatchCollection matches = regex.Matches(filename);
                foreach (Match match in matches)
                {
                    foreach (Group item in match.Groups)
                    {
                        if (item.Name != "0")
                        {
                            stringBuilder.Replace($"({item.Name})", $"{item.Value}");
                        }
                    }
                }
            }
            return stringBuilder.ToString();
        }

    }
}
