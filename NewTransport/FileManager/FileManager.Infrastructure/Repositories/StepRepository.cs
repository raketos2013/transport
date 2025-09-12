using FileManager.Core.Entities;
using FileManager.Core.Enums;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class StepRepository(AppDbContext appDbContext) : IStepRepository
{
    public async Task<bool> ActivatedStep(int stepId)
    {
        try
        {
            TaskStepEntity? step = await appDbContext.TaskStep.FirstOrDefaultAsync(x => x.StepId == stepId);

            if (step != null)
            {
                TaskEntity? task = await appDbContext.Task.FirstOrDefaultAsync(x => x.TaskId == step.TaskId);
                if (task != null)
                {
                    step.IsActive = !step.IsActive;
                    appDbContext.TaskStep.Update(step);
                    await appDbContext.SaveChangesAsync();
                    task.LastModified = DateTime.Now;
                    appDbContext.Task.Update(task);
                    await appDbContext.SaveChangesAsync();
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

    public async Task<bool> CreateStep(TaskStepEntity taskStep)
    {
        try
        {

            var stepsAsync = await GetAllStepsByTaskId(taskStep.TaskId);
            var steps = stepsAsync.OrderBy(x => x.StepNumber).ToList();
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
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    

    public async Task<bool> DeleteStep(int stepId)
    {
        try
        {
            var step = await appDbContext.TaskStep.FirstOrDefaultAsync(x => x.StepId == stepId);
            if (step == null)
            {
                return false;
            }
            switch (step.OperationName)
            {
                case OperationName.Copy:
                    var copy = await appDbContext.OperationCopy.FirstOrDefaultAsync(x => x.StepId == step.StepId);
                    if (copy != null)
                    {
                        appDbContext.OperationCopy.Remove(copy);
                    }
                    break;
                case OperationName.Move:
                    var move = await appDbContext.OperationMove.FirstOrDefaultAsync(x => x.StepId == step.StepId);
                    if (move != null)
                    {
                        appDbContext.OperationMove.Remove(move);
                    }
                    break;
                case OperationName.Read:
                    var read = await appDbContext.OperationRead.FirstOrDefaultAsync(x => x.StepId == step.StepId);
                    if (read != null)
                    {
                        appDbContext.OperationRead.Remove(read);
                    }
                    break;
                case OperationName.Exist:
                    var exist = await appDbContext.OperationExist.FirstOrDefaultAsync(x => x.StepId == step.StepId);
                    if (exist != null)
                    {
                        appDbContext.OperationExist.Remove(exist);
                    }
                    break;
                case OperationName.Rename:
                    var rename = await appDbContext.OperationRename.FirstOrDefaultAsync(x => x.StepId == step.StepId);
                    if (rename != null)
                    {
                        appDbContext.OperationRename.Remove(rename);
                    }
                    break;
                case OperationName.Delete:
                    var delete = await appDbContext.OperationDelete.FirstOrDefaultAsync(x => x.StepId == step.StepId);
                    if (delete != null)
                    {
                        appDbContext.OperationDelete.Remove(delete);
                    }
                    break;
                case OperationName.Clrbuf:
                    var clrbuf = await appDbContext.OperationClrbuf.FirstOrDefaultAsync(x => x.StepId == step.StepId);
                    if (clrbuf != null)
                    {
                        appDbContext.OperationClrbuf.Remove(clrbuf);
                    }
                    break;
            }
            appDbContext.TaskStep.Remove(step);
            var steps = await GetAllStepsByTaskId(step.TaskId);
            foreach (var item in steps.Where(x => x.StepNumber > step.StepNumber))
            {
                item.StepNumber--;
            }
            steps.Remove(step);
            appDbContext.UpdateRange(steps);
            await appDbContext.SaveChangesAsync();
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteStepsByTaskId(string taskId)
    {
        try
        {
            var steps = await GetAllStepsByTaskId(taskId);
            foreach (var item in steps)
            {
                switch (item.OperationName)
                {
                    case OperationName.Copy:
                        var copy = await appDbContext.OperationCopy.FirstOrDefaultAsync(x => x.StepId == item.StepId);
                        if (copy != null)
                        {
                            appDbContext.OperationCopy.Remove(copy);
                        }
                        break;
                    case OperationName.Move:
                        var move = await appDbContext.OperationMove.FirstOrDefaultAsync(x => x.StepId == item.StepId);
                        if (move != null)
                        {
                            appDbContext.OperationMove.Remove(move);
                        }
                        break;
                    case OperationName.Read:
                        var read = await appDbContext.OperationRead.FirstOrDefaultAsync(x => x.StepId == item.StepId);
                        if(read != null)
                        {
                            appDbContext.OperationRead.Remove(read);
                        }
                        break;
                    case OperationName.Exist:
                        var exist = await appDbContext.OperationExist.FirstOrDefaultAsync(x => x.StepId == item.StepId);
                        if (exist != null)
                        {
                            appDbContext.OperationExist.Remove(exist);
                        }
                        break;
                    case OperationName.Rename:
                        var rename = await appDbContext.OperationRename.FirstOrDefaultAsync(x => x.StepId == item.StepId);
                        if (rename != null)
                        {
                            appDbContext.OperationRename.Remove(rename);
                        }
                        break;
                    case OperationName.Delete:
                        var delete = await appDbContext.OperationDelete.FirstOrDefaultAsync(x => x.StepId == item.StepId);
                        if(delete != null)
                        {
                            appDbContext.OperationDelete.Remove(delete);
                        }
                        break;
                    case OperationName.Clrbuf:
                        var clrbuf = await appDbContext.OperationClrbuf.FirstOrDefaultAsync(x => x.StepId == item.StepId);
                        if (clrbuf != null)
                        {
                            appDbContext.OperationClrbuf.Remove(clrbuf);
                        }
                        break;
                }
            }
            appDbContext.RemoveRange(steps);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> EditStep(TaskStepEntity taskStep)
    {
        try
        {
            appDbContext.TaskStep.Update(taskStep);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<TaskStepEntity>> GetAllSteps()
    {
        return await appDbContext.TaskStep.ToListAsync();
    }

    public async Task<List<TaskStepEntity>> GetAllStepsByTaskId(string taskId)
    {
        return await appDbContext.TaskStep.Where(x => x.TaskId == taskId).ToListAsync();
    }

    public async Task<TaskStepEntity?> GetStepByStepId(int stepId)
    {
        return await appDbContext.TaskStep.FirstOrDefaultAsync(x => x.StepId == stepId);
    }

    public async Task<TaskStepEntity?> GetStepByTaskId(string taskId, int stepNumber)
    {
        return await appDbContext.TaskStep.FirstOrDefaultAsync(x => x.TaskId == taskId &&
                                                                    x.StepNumber == stepNumber);
    }

    public async Task<bool> UpdateRangeSteps(List<TaskStepEntity> steps)
    {
        try
        {
            appDbContext.TaskStep.UpdateRange(steps);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

}
