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

    bool DeleteCopy(OperationCopyEntity operation);
    bool DeleteMove(OperationMoveEntity operation);
    bool DeleteRead(OperationReadEntity operation);
    bool DeleteExist(OperationExistEntity operation);
    bool DeleteDelete(OperationDeleteEntity operation);
    bool DeleteRename(OperationRenameEntity operation);
    bool DeleteClrbuf(OperationClrbufEntity operation);

    Task<OperationCopyEntity> CreateCopy(OperationCopyEntity operation);
    Task<OperationMoveEntity> CreateMove(OperationMoveEntity operation);
    Task<OperationReadEntity> CreateRead(OperationReadEntity operation);
    Task<OperationExistEntity> CreateExist(OperationExistEntity operation);
    Task<OperationDeleteEntity> CreateDelete(OperationDeleteEntity operation);
    Task<OperationRenameEntity> CreateRename(OperationRenameEntity operation);
    Task<OperationClrbufEntity> CreateClrbuf(OperationClrbufEntity operation);

    OperationCopyEntity UpdateCopy(OperationCopyEntity operation);
    OperationMoveEntity UpdateMove(OperationMoveEntity operation);
    OperationReadEntity UpdateRead(OperationReadEntity operation);
    OperationExistEntity UpdateExist(OperationExistEntity operation);
    OperationDeleteEntity UpdateDelete(OperationDeleteEntity operation);
    OperationRenameEntity UpdateRename(OperationRenameEntity operation);
    OperationClrbufEntity UpdateClrbuf(OperationClrbufEntity operation);

    Task<OperationCopyEntity?> GetCopyByOperationId(int operationId);
    Task<OperationMoveEntity?> GetMoveByOperationId(int operationId);
    Task<OperationReadEntity?> GetReadByOperationId(int operationId);
    Task<OperationExistEntity?> GetExistByOperationId(int operationId);
    Task<OperationRenameEntity?> GetRenameByOperationId(int operationId);
    Task<OperationDeleteEntity?> GetDeleteByOperationId(int operationId);
    Task<OperationClrbufEntity?> GetClrbufByOperationId(int operationId);
}
