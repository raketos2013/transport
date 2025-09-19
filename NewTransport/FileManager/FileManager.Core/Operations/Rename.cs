using FileManager.Core.Constants;
using FileManager.Core.Entities;
using FileManager.Core.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using System.Text;
using System.Text.RegularExpressions;

namespace FileManager.Core.Operations;

public class Rename(TaskStepEntity step,
                    TaskOperation? operation,
                    IServiceScopeFactory scopeFactory)
             : StepOperation(step, operation, scopeFactory)
{
    public override async Task Execute(List<string>? bufferFiles)
    {
        await _taskLogger.StepLog(TaskStep, $"ПЕРЕИМЕНОВАНИЕ: {TaskStep.Source}");
        await _taskLogger.OperationLog(TaskStep);

        string[] files;
        string fileName, newFileName;
        List<FileInfo> infoFiles = [];
        if (TaskStep.FileMask == AppConstants.BUFFER_FILE_MASK)
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
        await _taskLogger.StepLog(TaskStep, $"Количество найденный файлов по маске '{TaskStep.FileMask}': {infoFiles.Count}");

        if (infoFiles.Count > 0)
        {
            //List<string> successFiles = [];
            OperationRenameEntity? operation = await _operationService.GetRenameByStepId(TaskStep.StepId);
            if (operation != null)
            {
                if (operation.InformSuccess)
                {
                    var addressesAsync = await _addresseeService.GetAllAddressees();
                    addresses = addressesAsync.Where(x => x.AddresseeGroupId == operation.AddresseeGroupId &&
                                                          x.IsActive == true).ToList();
                }
                foreach (var file in infoFiles)
                {
                    fileName = Path.GetFileName(file.FullName);
                    newFileName = RenameFileNew(fileName, operation.OldPattern, operation.NewPattern);
                    FileSystem.Rename(file.FullName, file.DirectoryName + "\\\\" + newFileName);
                    await _taskLogger.StepLog(TaskStep, $"Файл переименован в {newFileName}", fileName);
                }
            }
        }
        else
        {
            if (TaskStep.IsBreak)
            {
                await _taskLogger.StepLog(TaskStep, $"Прерывание задачи: найдено 0 файлов", "", ResultOperation.W);
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