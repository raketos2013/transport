using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager_Server.Loggers;
using FileManager_Server.MailSender;
using Microsoft.VisualBasic;
using System.Text;
using System.Text.RegularExpressions;


namespace FileManager_Server.Operations;

public class Rename(TaskStepEntity step,
                    TaskOperation? operation,
                    ITaskLogger taskLogger,
                    AppDbContext appDbContext,
                    IMailSender mailSender)
            : StepOperation(step, operation, taskLogger, appDbContext, mailSender)
{
    public override void Execute(List<string>? bufferFiles)
    {
        _taskLogger.StepLog(TaskStep, $"ПЕРЕИМЕНОВАНИЕ: {TaskStep.Source}");
        _taskLogger.OperationLog(TaskStep);

        string[] files = [];
        string fileName, newFileName;
        List<FileInfo> infoFiles = [];
        List<string> successFiles = [];
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
        List<AddresseeEntity> addresses = [];
        _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count}");

        if (infoFiles.Count > 0)
        {
            operation = _appDbContext.OperationRename.FirstOrDefault(x => x.StepId == TaskStep.StepId);
            if (operation != null)
            {
                foreach (var file in infoFiles)
                {
                    fileName = Path.GetFileName(file.FullName);
                    newFileName = RenameFileNew(fileName, operation.OldPattern, operation.NewPattern);
                    FileSystem.Rename(file.FullName, file.DirectoryName + "\\\\" + newFileName);
                }

            }
        }
        else
        {
            if (TaskStep.IsBreak)
            {
                _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
                throw new Exception("Операция Rename: найдено 0 файлов");
            }
        }
        _nextStep?.Execute(bufferFiles);
    }

    private static string RenameFileNew(string filename, string old_pattern, string new_pattern)
    {
        StringBuilder stringBuilder = new(new_pattern);

        if (Regex.IsMatch(filename, old_pattern, RegexOptions.IgnoreCase))
        {
            Regex regex = new(old_pattern, RegexOptions.IgnoreCase);

            MatchCollection matches = regex.Matches(filename);
            foreach (Match match in matches)
            {
                foreach (var item in from Group item in match.Groups
                                     where item.Name != "0"
                                     select item)
                {
                    stringBuilder.Replace($"({item.Name})", $"{item.Value}");
                }
            }
        }
        Regex regexDate = new("(.*)({.*})(.*)", RegexOptions.IgnoreCase);
        MatchCollection matchesDate = regexDate.Matches(new_pattern);
        if (matchesDate.Count == 1)
        {
            var findedTypeDate = matchesDate[0].Groups[2].Value;
            var typeDate = findedTypeDate[1..^1];
            stringBuilder.Replace(findedTypeDate, DateTime.Now.ToString(typeDate));
        }
        return stringBuilder.ToString();
    }
}
