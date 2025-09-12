using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IOperationRepository
{
    Task<OperationCopyEntity?> GetCopyByStepId(int stepId);
    Task<OperationMoveEntity?> GetMoveByStepId(int stepId);
    Task<OperationReadEntity?> GetReadByStepId(int stepId);
    Task<OperationExistEntity?> GetExistByStepId(int stepId);
    Task<OperationRenameEntity?> GetRenameByStepId(int stepId);
    Task<OperationDeleteEntity?> GetDeleteByStepId(int stepId);
    Task<OperationClrbufEntity?> GetClrbufByStepId(int stepId);

    Task<bool> DeleteCopy(OperationCopyEntity operation);
    Task<bool> DeleteMove(OperationMoveEntity operation);
    Task<bool> DeleteRead(OperationReadEntity operation);
    Task<bool> DeleteExist(OperationExistEntity operation);
    Task<bool> DeleteDelete(OperationDeleteEntity operation);
    Task<bool> DeleteRename(OperationRenameEntity operation);
    Task<bool> DeleteClrbuf(OperationClrbufEntity operation);

    Task<bool> CreateCopy(OperationCopyEntity operation);
    Task<bool> CreateMove(OperationMoveEntity operation);
    Task<bool> CreateRead(OperationReadEntity operation);
    Task<bool> CreateExist(OperationExistEntity operation);
    Task<bool> CreateDelete(OperationDeleteEntity operation);
    Task<bool> CreateRename(OperationRenameEntity operation);
    Task<bool> CreateClrbuf(OperationClrbufEntity operation);

    Task<bool> UpdateCopy(OperationCopyEntity operation);
    Task<bool>   UpdateMove(OperationMoveEntity operation);
    Task<bool> UpdateRead(OperationReadEntity operation);
    Task<bool>   UpdateExist(OperationExistEntity operation);
    Task<bool> UpdateDelete(OperationDeleteEntity operation);
    Task<bool> UpdateRename(OperationRenameEntity operation);
    Task<bool> UpdateClrbuf(OperationClrbufEntity operation);

    Task<OperationCopyEntity?> GetCopyByOperationId(int operationId);
    Task<OperationMoveEntity?> GetMoveByOperationId(int operationId);
    Task<OperationReadEntity?> GetReadByOperationId(int operationId);
    Task<OperationExistEntity?> GetExistByOperationId(int operationId);
    Task<OperationRenameEntity?> GetRenameByOperationId(int operationId);
    Task<OperationDeleteEntity?> GetDeleteByOperationId(int operationId);
    Task<OperationClrbufEntity?> GetClrbufByOperationId(int operationId);

}
