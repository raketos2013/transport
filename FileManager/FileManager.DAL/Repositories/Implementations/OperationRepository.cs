using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.DAL.Repositories.Implementations
{
    public class OperationRepository : IOperationRepository
    {
        private readonly AppDbContext _appDbContext;

        public OperationRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public bool CreateClrbuf(OperationClrbufEntity operation)
        {
            try
            {
                _appDbContext.OperationClrbuf.Add(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationCopy.Add(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationDelete.Add(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationExist.Add(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationMove.Add(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationRead.Add(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationRename.Add(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationClrbuf.Remove(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationCopy.Remove(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationDelete.Remove(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationExist.Remove(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationMove.Remove(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationRead.Remove(operation);
                _appDbContext.SaveChanges();
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
                _appDbContext.OperationRename.Remove(operation);
                _appDbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public OperationClrbufEntity? GetClrbufByStepId(int stepId)
        {
            return _appDbContext.OperationClrbuf.FirstOrDefault(x => x.StepId == stepId);
        }

        public OperationCopyEntity? GetCopyByStepId(int stepId)
        {
            return _appDbContext.OperationCopy.FirstOrDefault(x => x.StepId == stepId);
        }

        public OperationDeleteEntity? GetDeleteByStepId(int stepId)
        {
            return _appDbContext.OperationDelete.FirstOrDefault(x => x.StepId == stepId);
        }

        public OperationExistEntity? GetExistByStepId(int stepId)
        {
            return _appDbContext.OperationExist.FirstOrDefault(x => x.StepId == stepId);
        }

        public OperationMoveEntity? GetMoveByStepId(int stepId)
        {
            return _appDbContext.OperationMove.FirstOrDefault(x => x.StepId == stepId);
        }

        public OperationReadEntity? GetReadByStepId(int stepId)
        {
            return _appDbContext.OperationRead.FirstOrDefault(x => x.StepId == stepId);
        }

        public OperationRenameEntity? GetRenameByStepId(int stepId)
        {
            return _appDbContext.OperationRename.FirstOrDefault(x => x.StepId == stepId);
        }
    }
}
