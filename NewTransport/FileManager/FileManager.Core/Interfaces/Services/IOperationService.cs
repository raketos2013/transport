using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Services;

public interface IOperationService
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

    Task<OperationCopyEntity> CreateCopy(OperationCopyEntity operation);
    Task<OperationMoveEntity> CreateMove(OperationMoveEntity operation);
    Task<OperationReadEntity> CreateRead(OperationReadEntity operation);
    Task<OperationExistEntity> CreateExist(OperationExistEntity operation);
    Task<OperationDeleteEntity> CreateDelete(OperationDeleteEntity operation);
    Task<OperationRenameEntity> CreateRename(OperationRenameEntity operation);
    Task<OperationClrbufEntity> CreateClrbuf(OperationClrbufEntity operation);

    Task<OperationCopyEntity> UpdateCopy(OperationCopyEntity operation);
    Task<OperationMoveEntity> UpdateMove(OperationMoveEntity operation);
    Task<OperationReadEntity> UpdateRead(OperationReadEntity operation);
    Task<OperationExistEntity> UpdateExist(OperationExistEntity operation);
    Task<OperationDeleteEntity> UpdateDelete(OperationDeleteEntity operation);
    Task<OperationRenameEntity> UpdateRename(OperationRenameEntity operation);
    Task<OperationClrbufEntity> UpdateClrbuf(OperationClrbufEntity operation);
}
