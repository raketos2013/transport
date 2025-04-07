using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.Implementations
{
    public class OperationService : IOperationService
    {
        private readonly IOperationRepository _operationRepository;

        public OperationService(IOperationRepository operationRepository)
        {
            _operationRepository = operationRepository;
        }

        public bool CreateClrbuf(OperationClrbufEntity operation)
        {
            return _operationRepository.CreateClrbuf(operation);
        }

        public bool CreateCopy(OperationCopyEntity operation)
        {
            return _operationRepository.CreateCopy(operation);
        }

        public bool CreateDelete(OperationDeleteEntity operation)
        {
            return _operationRepository.CreateDelete(operation);
        }

        public bool CreateExist(OperationExistEntity operation)
        {
            return _operationRepository.CreateExist(operation);
        }

        public bool CreateMove(OperationMoveEntity operation)
        {
            return _operationRepository.CreateMove(operation);
        }

        public bool CreateRead(OperationReadEntity operation)
        {
            return _operationRepository.CreateRead(operation);
        }

        public bool CreateRename(OperationRenameEntity operation)
        {
            return _operationRepository.CreateRename(operation);
        }

        public bool DeleteClrbuf(OperationClrbufEntity operation)
        {
            return _operationRepository.DeleteClrbuf(operation);
        }

        public bool DeleteCopy(OperationCopyEntity operation)
        {
            return _operationRepository.DeleteCopy(operation);
        }

        public bool DeleteDelete(OperationDeleteEntity operation)
        {
            return _operationRepository.DeleteDelete(operation);
        }

        public bool DeleteExist(OperationExistEntity operation)
        {
            return _operationRepository.DeleteExist(operation);
        }

        public bool DeleteMove(OperationMoveEntity operation)
        {
            return _operationRepository.DeleteMove(operation);
        }

        public bool DeleteRead(OperationReadEntity operation)
        {
            return _operationRepository.DeleteRead(operation);
        }

        public bool DeleteRename(OperationRenameEntity operation)
        {
            return _operationRepository.DeleteRename(operation);
        }

        public OperationClrbufEntity? GetClrbufByStepId(int stepId)
        {
            return _operationRepository.GetClrbufByStepId(stepId);
        }

        public OperationCopyEntity? GetCopyByStepId(int stepId)
        {
            return _operationRepository.GetCopyByStepId(stepId);
        }

        public OperationDeleteEntity? GetDeleteByStepId(int stepId)
        {
            return _operationRepository.GetDeleteByStepId(stepId);
        }

        public OperationExistEntity? GetExistByStepId(int stepId)
        {
            return _operationRepository.GetExistByStepId(stepId);
        }

        public OperationMoveEntity? GetMoveByStepId(int stepId)
        {
            return _operationRepository.GetMoveByStepId(stepId);
        }

        public OperationReadEntity? GetReadByStepId(int stepId)
        {
            return _operationRepository.GetReadByStepId(stepId);
        }

        public OperationRenameEntity? GetRenameByStepId(int stepId)
        {
            return _operationRepository.GetRenameByStepId(stepId);
        }
    }
}
