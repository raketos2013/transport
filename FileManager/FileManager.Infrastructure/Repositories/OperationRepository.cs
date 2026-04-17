using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class OperationRepository(AppDbContext appDbContext) : IOperationRepository
{
    public async Task<OperationClrbufEntity> CreateClrbuf(OperationClrbufEntity operation)
    {
        await appDbContext.OperationClrbuf.AddAsync(operation);
        return operation;
    }

    public async Task<OperationCopyEntity> CreateCopy(OperationCopyEntity operation)
    {
        await appDbContext.OperationCopy.AddAsync(operation);
        return operation;
    }

    public async Task<OperationDeleteEntity> CreateDelete(OperationDeleteEntity operation)
    {
        await appDbContext.OperationDelete.AddAsync(operation);
        return operation;
    }

    public async Task<OperationExistEntity> CreateExist(OperationExistEntity operation)
    {
        await appDbContext.OperationExist.AddAsync(operation);
        return operation;
    }

    public async Task<OperationMoveEntity> CreateMove(OperationMoveEntity operation)
    {
        await appDbContext.OperationMove.AddAsync(operation);
        return operation;
    }

    public async Task<OperationReadEntity> CreateRead(OperationReadEntity operation)
    {
        await appDbContext.OperationRead.AddAsync(operation);
        return operation;
    }

    public async Task<OperationRenameEntity> CreateRename(OperationRenameEntity operation)
    {
        await appDbContext.OperationRename.AddAsync(operation);
        return operation;
    }

    public bool DeleteClrbuf(OperationClrbufEntity operation)
    {
        try
        {
            appDbContext.OperationClrbuf.Remove(operation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteCopy(OperationCopyEntity operation)
    {
        try
        {
            appDbContext.OperationCopy.Remove(operation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteDelete(OperationDeleteEntity operation)
    {
        try
        {
            appDbContext.OperationDelete.Remove(operation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteExist(OperationExistEntity operation)
    {
        try
        {
            appDbContext.OperationExist.Remove(operation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteMove(OperationMoveEntity operation)
    {
        try
        {
            appDbContext.OperationMove.Remove(operation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteRead(OperationReadEntity operation)
    {
        try
        {
            appDbContext.OperationRead.Remove(operation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteRename(OperationRenameEntity operation)
    {
        try
        {
            appDbContext.OperationRename.Remove(operation);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<OperationClrbufEntity?> GetClrbufByOperationId(int operationId)
    {
        return await appDbContext.OperationClrbuf.FirstOrDefaultAsync(x => x.OperationId == operationId);
    }

    public async Task<OperationClrbufEntity?> GetClrbufByStepId(int stepId)
    {
        return await appDbContext.OperationClrbuf.FirstOrDefaultAsync(x => x.StepId == stepId);
    }

    public async Task<OperationCopyEntity?> GetCopyByOperationId(int operationId)
    {
        return await appDbContext.OperationCopy.FirstOrDefaultAsync(x => x.OperationId == operationId);
    }

    public async Task<OperationCopyEntity?> GetCopyByStepId(int stepId)
    {
        return await appDbContext.OperationCopy.FirstOrDefaultAsync(x => x.StepId == stepId);
    }

    public async Task<OperationDeleteEntity?> GetDeleteByOperationId(int operationId)
    {
        return await appDbContext.OperationDelete.FirstOrDefaultAsync(x => x.OperationId == operationId);
    }

    public async Task<OperationDeleteEntity?> GetDeleteByStepId(int stepId)
    {
        return await appDbContext.OperationDelete.FirstOrDefaultAsync(x => x.StepId == stepId);
    }

    public async Task<OperationExistEntity?> GetExistByOperationId(int operationId)
    {
        return await appDbContext.OperationExist.FirstOrDefaultAsync(x => x.OperationId == operationId);
    }

    public async Task<OperationExistEntity?> GetExistByStepId(int stepId)
    {
        return await appDbContext.OperationExist.FirstOrDefaultAsync(x => x.StepId == stepId);
    }

    public async Task<OperationMoveEntity?> GetMoveByOperationId(int operationId)
    {
        return await appDbContext.OperationMove.FirstOrDefaultAsync(x => x.OperationId == operationId);
    }

    public async Task<OperationMoveEntity?> GetMoveByStepId(int stepId)
    {
        return await appDbContext.OperationMove.FirstOrDefaultAsync(x => x.StepId == stepId);
    }

    public async Task<OperationReadEntity?> GetReadByOperationId(int operationId)
    {
        return await appDbContext.OperationRead.FirstOrDefaultAsync(x => x.OperationId == operationId);
    }

    public async Task<OperationReadEntity?> GetReadByStepId(int stepId)
    {
        return await appDbContext.OperationRead.FirstOrDefaultAsync(x => x.StepId == stepId);
    }

    public async Task<OperationRenameEntity?> GetRenameByOperationId(int operationId)
    {
        return await appDbContext.OperationRename.FirstOrDefaultAsync(x => x.OperationId == operationId);
    }

    public async Task<OperationRenameEntity?> GetRenameByStepId(int stepId)
    {
        return await appDbContext.OperationRename.FirstOrDefaultAsync(x => x.StepId == stepId);
    }

    public OperationClrbufEntity UpdateClrbuf(OperationClrbufEntity operation)
    {
        appDbContext.OperationClrbuf.Update(operation);
        return operation;
    }

    public OperationCopyEntity UpdateCopy(OperationCopyEntity operation)
    {
        appDbContext.OperationCopy.Update(operation);
        return operation;
    }

    public OperationDeleteEntity UpdateDelete(OperationDeleteEntity operation)
    {
        appDbContext.OperationDelete.Update(operation);
        return operation;
    }

    public OperationExistEntity UpdateExist(OperationExistEntity operation)
    {
        appDbContext.OperationExist.Update(operation);
        return operation;
    }

    public OperationMoveEntity UpdateMove(OperationMoveEntity operation)
    {
        appDbContext.OperationMove.Update(operation);
        return operation;
    }

    public OperationReadEntity UpdateRead(OperationReadEntity operation)
    {
        appDbContext.OperationRead.Update(operation);
        return operation;
    }

    public OperationRenameEntity UpdateRename(OperationRenameEntity operation)
    {
        appDbContext.OperationRename.Update(operation);
        return operation;
    }
}
