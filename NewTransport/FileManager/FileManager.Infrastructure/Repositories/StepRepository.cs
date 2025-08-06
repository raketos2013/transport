using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;

namespace FileManager.Infrastructure.Repositories;

public class StepRepository(AppDbContext appDbContext) : IStepRepository
{
    public bool ActivatedStep(int stepId)
    {
        try
        {
            TaskStepEntity? step = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == stepId);

            if (step != null)
            {
                TaskEntity? task = appDbContext.Task.FirstOrDefault(x => x.TaskId == step.TaskId);
                if (task != null)
                {
                    step.IsActive = !step.IsActive;
                    appDbContext.TaskStep.Update(step);
                    appDbContext.SaveChanges();
                    task.LastModified = DateTime.Now;
                    appDbContext.Task.Update(task);
                    appDbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateStep(TaskStepEntity taskStep)
    {
        try
        {

            List<TaskStepEntity> steps = GetAllStepsByTaskId(taskStep.TaskId).OrderBy(x => x.StepNumber).ToList();
            if (steps.Count != 0)
            {
                foreach (var step in steps)
                {
                    if (step.StepNumber >= taskStep.StepNumber)
                    {
                        step.StepNumber++;
                    }
                }
                appDbContext.TaskStep.UpdateRange(steps);
            }

            appDbContext.TaskStep.Add(taskStep);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    

    public bool DeleteStep(int stepId)
    {
        try
        {
            var step = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == stepId);
            if (step == null)
            {
                return false;
            }
            switch (step.OperationName)
            {
                case OperationName.Copy:
                    var copy = appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == step.StepId);
                    if (copy != null)
                    {
                        appDbContext.OperationCopy.Remove(copy);
                    }
                    break;
                case OperationName.Move:
                    var move = appDbContext.OperationMove.FirstOrDefault(x => x.StepId == step.StepId);
                    if (move != null)
                    {
                        appDbContext.OperationMove.Remove(move);
                    }
                    break;
                case OperationName.Read:
                    var read = appDbContext.OperationRead.FirstOrDefault(x => x.StepId == step.StepId);
                    if (read != null)
                    {
                        appDbContext.OperationRead.Remove(read);
                    }
                    break;
                case OperationName.Exist:
                    var exist = appDbContext.OperationExist.FirstOrDefault(x => x.StepId == step.StepId);
                    if (exist != null)
                    {
                        appDbContext.OperationExist.Remove(exist);
                    }
                    break;
                case OperationName.Rename:
                    var rename = appDbContext.OperationRename.FirstOrDefault(x => x.StepId == step.StepId);
                    if (rename != null)
                    {
                        appDbContext.OperationRename.Remove(rename);
                    }
                    break;
                case OperationName.Delete:
                    var delete = appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == step.StepId);
                    if (delete != null)
                    {
                        appDbContext.OperationDelete.Remove(delete);
                    }
                    break;
                case OperationName.Clrbuf:
                    var clrbuf = appDbContext.OperationClrbuf.FirstOrDefault(x => x.StepId == step.StepId);
                    if (clrbuf != null)
                    {
                        appDbContext.OperationClrbuf.Remove(clrbuf);
                    }
                    break;
            }
            appDbContext.TaskStep.Remove(step);
            var steps = GetAllStepsByTaskId(step.TaskId);
            foreach (var item in steps.Where(x => x.StepNumber > step.StepNumber))
            {
                item.StepNumber--;
            }
            steps.Remove(step);
            appDbContext.UpdateRange(steps);
            appDbContext.SaveChanges();
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteStepsByTaskId(string taskId)
    {
        try
        {
            var steps = GetAllStepsByTaskId(taskId);
            foreach (var item in steps)
            {
                switch (item.OperationName)
                {
                    case OperationName.Copy:
                        var copy = appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == item.StepId);
                        if (copy != null)
                        {
                            appDbContext.OperationCopy.Remove(copy);
                        }
                        break;
                    case OperationName.Move:
                        var move = appDbContext.OperationMove.FirstOrDefault(x => x.StepId == item.StepId);
                        if (move != null)
                        {
                            appDbContext.OperationMove.Remove(move);
                        }
                        break;
                    case OperationName.Read:
                        var read = appDbContext.OperationRead.FirstOrDefault(x => x.StepId == item.StepId);
                        if(read != null)
                        {
                            appDbContext.OperationRead.Remove(read);
                        }
                        break;
                    case OperationName.Exist:
                        var exist = appDbContext.OperationExist.FirstOrDefault(x => x.StepId == item.StepId);
                        if (exist != null)
                        {
                            appDbContext.OperationExist.Remove(exist);
                        }
                        break;
                    case OperationName.Rename:
                        var rename = appDbContext.OperationRename.FirstOrDefault(x => x.StepId == item.StepId);
                        if (rename != null)
                        {
                            appDbContext.OperationRename.Remove(rename);
                        }
                        break;
                    case OperationName.Delete:
                        var delete = appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == item.StepId);
                        if(delete != null)
                        {
                            appDbContext.OperationDelete.Remove(delete);
                        }
                        break;
                    case OperationName.Clrbuf:
                        var clrbuf = appDbContext.OperationClrbuf.FirstOrDefault(x => x.StepId == item.StepId);
                        if (clrbuf != null)
                        {
                            appDbContext.OperationClrbuf.Remove(clrbuf);
                        }
                        break;
                }
            }
            appDbContext.RemoveRange(steps);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool EditStep(TaskStepEntity taskStep)
    {
        try
        {
            appDbContext.TaskStep.Update(taskStep);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<TaskStepEntity> GetAllSteps()
    {
        return appDbContext.TaskStep.ToList();
    }

    public List<TaskStepEntity> GetAllStepsByTaskId(string taskId)
    {
        return appDbContext.TaskStep.Where(x => x.TaskId == taskId).ToList();
    }

    public TaskStepEntity? GetStepByStepId(int stepId)
    {
        return appDbContext.TaskStep.FirstOrDefault(x => x.StepId == stepId);
    }

    public TaskStepEntity? GetStepByTaskId(string taskId, int stepNumber)
    {
        return appDbContext.TaskStep.FirstOrDefault(x => x.TaskId == taskId &&
                                                            x.StepNumber == stepNumber);
    }

    public bool UpdateRangeSteps(List<TaskStepEntity> steps)
    {
        try
        {
            appDbContext.TaskStep.UpdateRange(steps);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}
