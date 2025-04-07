using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.DAL.Repositories.Interfaces
{
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
    }
}
