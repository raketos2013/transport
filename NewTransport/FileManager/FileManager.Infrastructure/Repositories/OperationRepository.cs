using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileManager.Infrastructure.Repositories;

public class OperationRepository(AppDbContext appDbContext) : IOperationRepository
{
    public async Task<bool> CreateClrbuf(OperationClrbufEntity operation)
    {
        try
        {
            await appDbContext.OperationClrbuf.AddAsync(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CreateCopy(OperationCopyEntity operation)
    {
        try
        {
            await appDbContext.OperationCopy.AddAsync(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CreateDelete(OperationDeleteEntity operation)
    {
        try
        {
            await appDbContext.OperationDelete.AddAsync(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CreateExist(OperationExistEntity operation)
    {
        try
        {
            await appDbContext.OperationExist.AddAsync(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CreateMove(OperationMoveEntity operation)
    {
        try
        {
            await appDbContext.OperationMove.AddAsync(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CreateRead(OperationReadEntity operation)
    {
        try
        {
            await appDbContext.OperationRead.AddAsync(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> CreateRename(OperationRenameEntity operation)
    {
        try
        {
            await appDbContext.OperationRename.AddAsync(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteClrbuf(OperationClrbufEntity operation)
    {
        try
        {
            appDbContext.OperationClrbuf.Remove(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteCopy(OperationCopyEntity operation)
    {
        try
        {
            appDbContext.OperationCopy.Remove(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteDelete(OperationDeleteEntity operation)
    {
        try
        {
            appDbContext.OperationDelete.Remove(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteExist(OperationExistEntity operation)
    {
        try
        {
            appDbContext.OperationExist.Remove(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteMove(OperationMoveEntity operation)
    {
        try
        {
            appDbContext.OperationMove.Remove(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteRead(OperationReadEntity operation)
    {
        try
        {
            appDbContext.OperationRead.Remove(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> DeleteRename(OperationRenameEntity operation)
    {
        try
        {
            appDbContext.OperationRename.Remove(operation);
            await appDbContext.SaveChangesAsync();
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

    public async Task<bool> UpdateClrbuf(OperationClrbufEntity operation)
    {
        try
        {
            appDbContext.OperationClrbuf.Update(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateCopy(OperationCopyEntity operation)
    {
        try
        {
            appDbContext.OperationCopy.Update(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateDelete(OperationDeleteEntity operation)
    {
        try
        {
            appDbContext.OperationDelete.Update(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateExist(OperationExistEntity operation)
    {
        try
        {
            appDbContext.OperationExist.Update(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateMove(OperationMoveEntity operation)
    {
        try
        {
            appDbContext.OperationMove.Update(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateRead(OperationReadEntity operation)
    {
        try
        {
            appDbContext.OperationRead.Update(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> UpdateRename(OperationRenameEntity operation)
    {
        try
        {
            appDbContext.OperationRename.Update(operation);
            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
