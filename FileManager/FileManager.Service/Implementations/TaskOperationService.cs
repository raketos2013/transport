using FileManager.DAL.Interfaces;
using FileManager.DAL.Repositories;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.Response;
using FileManager.Domain.ViewModels.Operation;
using FileManager.Domain.ViewModels.Task;
using FileManager.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Service.Implementations
{
    public class TaskOperationService: ITaskOperationService
    {
        private readonly TaskOperationRepository _taskOperationRepository;
        public TaskOperationService(TaskOperationRepository taskOperationRepository) 
        {
            _taskOperationRepository = taskOperationRepository;
        }



        public async Task<IBaseResponse<TaskOperationEntity>> Create(TaskOperationEntity model)
        {
            try
            {
                var oper = model;  
/*                var oper = new TaskOperationEntity()
                {
                    TaskId = model.taskEntity.TaskId,
//                    OperationId = model.operationEntity.OperationId,
                    Description = model.operationEntity.Description,
                    DestinationDerictory = model.operationEntity.DestinationDerictory,
                    NewMask = model.operationEntity.NewMask,
                    IsActive = model.operationEntity.IsActive,
                    DublDest = model.operationEntity.DublDest,
                    Sort = model.operationEntity.Sort,
                    ScanAttr = model.operationEntity.ScanAttr,
                    IsComplit = model.operationEntity.IsComplit,
                    AdditionalText = model.operationEntity.AdditionalText
                };
*/
                await _taskOperationRepository.Create(oper);

                return new BaseResponse<TaskOperationEntity>()
                {
                    Description = "Каталог назначения создался успешно",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception)
            {
                return new BaseResponse<TaskOperationEntity>()
                {
                    Description = "Каталог назначения не создался",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }





        public async Task<IBaseResponse<TaskOperationEntity>> Update(TaskOperationEntity model)
        {
            try
            {


                var oper = new TaskOperationEntity()
                {
                    TaskId = model.TaskId,
                    OperationId = model.OperationId,
                    Description = model.Description,
                    DestinationDirectory = model.DestinationDirectory,
                    NewMask = model.NewMask,
                    IsActive = model.IsActive,
                    DublDest = model.DublDest,
                    Prior = model.Prior,
                    ScanAttr = model.ScanAttr,
                    IsComplit = model.IsComplit,
                    AdditionalText = model.AdditionalText
                };

                await _taskOperationRepository.Update(oper);

                return new BaseResponse<TaskOperationEntity>()
                {
                    Description = "Каталог назначения сохранен",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception)
            {
                return new BaseResponse<TaskOperationEntity>()
                {
                    Description = "Каталог назначения не сохранен",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }


        public IQueryable<TaskOperationEntity> GetAllForTask(string taskId)
        {
            return _taskOperationRepository.GetAllForTask(taskId);
        }



        public async Task<IBaseResponse<TaskOperationEntity>> Delete(string idTask, string idOperation)
        {
            try
            {
                await _taskOperationRepository.Delete(idOperation, idTask);

                return new BaseResponse<TaskOperationEntity>()
                {
                    Description = "Каталог назначения удален",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception)
            {
                return new BaseResponse<TaskOperationEntity>()
                {
                    Description = "Каталог назначения не удален",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }










    }
}
