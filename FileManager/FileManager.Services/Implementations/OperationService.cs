using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;


namespace FileManager.Services.Implementations
{
    public class OperationService(IOperationRepository operationRepository) 
                : IOperationService
    {

        public bool CreateClrbuf(OperationClrbufEntity operation)
        {
            return operationRepository.CreateClrbuf(operation);
        }

        public bool CreateCopy(OperationCopyEntity operation)
        {
            return operationRepository.CreateCopy(operation);
        }

        public bool CreateDelete(OperationDeleteEntity operation)
        {
            return operationRepository.CreateDelete(operation);
        }

        public bool CreateExist(OperationExistEntity operation)
        {
            return operationRepository.CreateExist(operation);
        }

        public bool CreateMove(OperationMoveEntity operation)
        {
            return operationRepository.CreateMove(operation);
        }

        public bool CreateRead(OperationReadEntity operation)
        {
            return operationRepository.CreateRead(operation);
        }

        public bool CreateRename(OperationRenameEntity operation)
        {
            return operationRepository.CreateRename(operation);
        }

        public bool DeleteClrbuf(OperationClrbufEntity operation)
        {
            return operationRepository.DeleteClrbuf(operation);
        }

        public bool DeleteCopy(OperationCopyEntity operation)
        {
            return operationRepository.DeleteCopy(operation);
        }

        public bool DeleteDelete(OperationDeleteEntity operation)
        {
            return operationRepository.DeleteDelete(operation);
        }

        public bool DeleteExist(OperationExistEntity operation)
        {
            return operationRepository.DeleteExist(operation);
        }

        public bool DeleteMove(OperationMoveEntity operation)
        {
            return operationRepository.DeleteMove(operation);
        }

        public bool DeleteRead(OperationReadEntity operation)
        {
            return operationRepository.DeleteRead(operation);
        }

        public bool DeleteRename(OperationRenameEntity operation)
        {
            return operationRepository.DeleteRename(operation);
        }

        public OperationClrbufEntity? GetClrbufByStepId(int stepId)
        {
            return operationRepository.GetClrbufByStepId(stepId);
        }

        public OperationCopyEntity? GetCopyByStepId(int stepId)
        {
            return operationRepository.GetCopyByStepId(stepId);
        }

        public OperationDeleteEntity? GetDeleteByStepId(int stepId)
        {
            return operationRepository.GetDeleteByStepId(stepId);
        }

        public OperationExistEntity? GetExistByStepId(int stepId)
        {
            return operationRepository.GetExistByStepId(stepId);
        }

        public OperationMoveEntity? GetMoveByStepId(int stepId)
        {
            return operationRepository.GetMoveByStepId(stepId);
        }

        public OperationReadEntity? GetReadByStepId(int stepId)
        {
            return operationRepository.GetReadByStepId(stepId);
        }

        public OperationRenameEntity? GetRenameByStepId(int stepId)
        {
            return operationRepository.GetRenameByStepId(stepId);
        }
    }
}
