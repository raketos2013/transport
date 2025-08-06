using FileManager.Core.Entities;

namespace FileManager.Core.Interfaces.Repositories;

public interface IOperationRepository
{
    OperationCopyEntity? GetCopyByStepId(int stepId);
    OperationMoveEntity? GetMoveByStepId(int stepId);
    OperationReadEntity? GetReadByStepId(int stepId);
    OperationExistEntity? GetExistByStepId(int stepId);
    OperationRenameEntity? GetRenameByStepId(int stepId);
    OperationDeleteEntity? GetDeleteByStepId(int stepId);
    OperationClrbufEntity? GetClrbufByStepId(int stepId);

    bool DeleteCopy(OperationCopyEntity operation);
    bool DeleteMove(OperationMoveEntity operation);
    bool DeleteRead(OperationReadEntity operation);
    bool DeleteExist(OperationExistEntity operation);
    bool DeleteDelete(OperationDeleteEntity operation);
    bool DeleteRename(OperationRenameEntity operation);
    bool DeleteClrbuf(OperationClrbufEntity operation);

    bool CreateCopy(OperationCopyEntity operation);
    bool CreateMove(OperationMoveEntity operation);
    bool CreateRead(OperationReadEntity operation);
    bool CreateExist(OperationExistEntity operation);
    bool CreateDelete(OperationDeleteEntity operation);
    bool CreateRename(OperationRenameEntity operation);
    bool CreateClrbuf(OperationClrbufEntity operation);

    bool UpdateCopy(OperationCopyEntity operation);
    bool UpdateMove(OperationMoveEntity operation);
    bool UpdateRead(OperationReadEntity operation);
    bool UpdateExist(OperationExistEntity operation);
    bool UpdateDelete(OperationDeleteEntity operation);
    bool UpdateRename(OperationRenameEntity operation);
    bool UpdateClrbuf(OperationClrbufEntity operation);

    OperationCopyEntity? GetCopyByOperationId(int operationId);
    OperationMoveEntity? GetMoveByOperationId(int operationId);
    OperationReadEntity? GetReadByOperationId(int operationId);
    OperationExistEntity? GetExistByOperationId(int operationId);
    OperationRenameEntity? GetRenameByOperationId(int operationId);
    OperationDeleteEntity? GetDeleteByOperationId(int operationId);
    OperationClrbufEntity? GetClrbufByOperationId(int operationId);

}
