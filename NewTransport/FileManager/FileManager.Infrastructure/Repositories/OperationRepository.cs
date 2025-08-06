using FileManager.Core.Entities;
using FileManager.Core.Interfaces.Repositories;
using FileManager.Infrastructure.Data;

namespace FileManager.Infrastructure.Repositories;

public class OperationRepository(AppDbContext appDbContext) : IOperationRepository
{
    public bool CreateClrbuf(OperationClrbufEntity operation)
    {
        try
        {
            appDbContext.OperationClrbuf.Add(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateCopy(OperationCopyEntity operation)
    {
        try
        {
            appDbContext.OperationCopy.Add(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateDelete(OperationDeleteEntity operation)
    {
        try
        {
            appDbContext.OperationDelete.Add(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateExist(OperationExistEntity operation)
    {
        try
        {
            appDbContext.OperationExist.Add(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateMove(OperationMoveEntity operation)
    {
        try
        {
            appDbContext.OperationMove.Add(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateRead(OperationReadEntity operation)
    {
        try
        {
            appDbContext.OperationRead.Add(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool CreateRename(OperationRenameEntity operation)
    {
        try
        {
            appDbContext.OperationRename.Add(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool DeleteClrbuf(OperationClrbufEntity operation)
    {
        try
        {
            appDbContext.OperationClrbuf.Remove(operation);
            appDbContext.SaveChanges();
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
            appDbContext.SaveChanges();
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
            appDbContext.SaveChanges();
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
            appDbContext.SaveChanges();
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
            appDbContext.SaveChanges();
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
            appDbContext.SaveChanges();
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
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public OperationClrbufEntity? GetClrbufByOperationId(int operationId)
    {
        return appDbContext.OperationClrbuf.FirstOrDefault(x => x.OperationId == operationId);
    }

    public OperationClrbufEntity? GetClrbufByStepId(int stepId)
    {
        return appDbContext.OperationClrbuf.FirstOrDefault(x => x.StepId == stepId);
    }

    public OperationCopyEntity? GetCopyByOperationId(int operationId)
    {
        return appDbContext.OperationCopy.FirstOrDefault(x => x.OperationId == operationId);
    }

    public OperationCopyEntity? GetCopyByStepId(int stepId)
    {
        return appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == stepId);
    }

    public OperationDeleteEntity? GetDeleteByOperationId(int operationId)
    {
        return appDbContext.OperationDelete.FirstOrDefault(x => x.OperationId == operationId);
    }

    public OperationDeleteEntity? GetDeleteByStepId(int stepId)
    {
        return appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == stepId);
    }

    public OperationExistEntity? GetExistByOperationId(int operationId)
    {
        return appDbContext.OperationExist.FirstOrDefault(x => x.OperationId == operationId);
    }

    public OperationExistEntity? GetExistByStepId(int stepId)
    {
        return appDbContext.OperationExist.FirstOrDefault(x => x.StepId == stepId);
    }

    public OperationMoveEntity? GetMoveByOperationId(int operationId)
    {
        return appDbContext.OperationMove.FirstOrDefault(x => x.OperationId == operationId);
    }

    public OperationMoveEntity? GetMoveByStepId(int stepId)
    {
        return appDbContext.OperationMove.FirstOrDefault(x => x.StepId == stepId);
    }

    public OperationReadEntity? GetReadByOperationId(int operationId)
    {
        return appDbContext.OperationRead.FirstOrDefault(x => x.OperationId == operationId);
    }

    public OperationReadEntity? GetReadByStepId(int stepId)
    {
        return appDbContext.OperationRead.FirstOrDefault(x => x.StepId == stepId);
    }

    public OperationRenameEntity? GetRenameByOperationId(int operationId)
    {
        return appDbContext.OperationRename.FirstOrDefault(x => x.OperationId == operationId);
    }

    public OperationRenameEntity? GetRenameByStepId(int stepId)
    {
        return appDbContext.OperationRename.FirstOrDefault(x => x.StepId == stepId);
    }

    public bool UpdateClrbuf(OperationClrbufEntity operation)
    {
        try
        {
            appDbContext.OperationClrbuf.Update(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateCopy(OperationCopyEntity operation)
    {
        try
        {
            appDbContext.OperationCopy.Update(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateDelete(OperationDeleteEntity operation)
    {
        try
        {
            appDbContext.OperationDelete.Update(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateExist(OperationExistEntity operation)
    {
        try
        {
            appDbContext.OperationExist.Update(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateMove(OperationMoveEntity operation)
    {
        try
        {
            appDbContext.OperationMove.Update(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateRead(OperationReadEntity operation)
    {
        try
        {
            appDbContext.OperationRead.Update(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool UpdateRename(OperationRenameEntity operation)
    {
        try
        {
            appDbContext.OperationRename.Update(operation);
            appDbContext.SaveChanges();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
