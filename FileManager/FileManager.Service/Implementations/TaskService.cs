using FileManager.DAL;
using FileManager.DAL.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Domain.Response;
using FileManager.Domain.ViewModels.Task;
using FileManager.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Service.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IBaseRepository<TaskEntity> _taskRepository;
       
        public TaskService(IBaseRepository<TaskEntity> taskRepository)
        {
            _taskRepository = taskRepository;
            
        }
        public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
        {
            try
            {
/*                var task = await _taskRepository.GetAll()
                    .Where(x => x.TaskId == model.TaskId)
                                          .FirstOrDefaultAsync(x => x.TaskId == model.TaskId);
                if (task != null)
                {
                    return new BaseResponse<TaskEntity>()
                    {
                        Description = "Задача с таким именем уже есть",
                        StatusCode = StatusCode.TaskIsHasAlready
                    };
                }
*/
                var task = new TaskEntity()
                {
                    TaskId = model.TaskId,
                    Name = model.Name,
                    TimeBegin = model.TimeBegin,
                    TimeEnd = model.TimeEnd,
                    DayActive = model.DayActive,
                    Group = model.Group,
                    IsActive = model.IsActive,
                    SourceCatalog = model.SourceCatalog,
                    FileMask = model.FileMask,
                    Delay = model.Delay,
                    ArchiveCatalog = model.ArchiveCatalog,
                    BadArchiveCatalog = model.BadArchiveCatalog,
                    IsDeleteSource = model.IsDeleteSource,
                    MaxAmountFiles = model.MaxAmountFiles,
                    DublNameJr = model.DublNameJr,
                };

                await _taskRepository.Create(task);

                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача создалась",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception)
            {
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача не создалась",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }


        public async Task<IBaseResponse<TaskEntity>> Update(CreateTaskViewModel model)
        {
            try
            {

            
            var task = new TaskEntity()
            {
                TaskId = model.TaskId,
                Name = model.Name,
                TimeBegin = model.TimeBegin,
                TimeEnd = model.TimeEnd,
                DayActive = model.DayActive,
                Group = model.Group,
                IsActive = model.IsActive,
                SourceCatalog = model.SourceCatalog,
                FileMask = model.FileMask,
                Delay = model.Delay,
                ArchiveCatalog = model.ArchiveCatalog,
                BadArchiveCatalog = model.BadArchiveCatalog,
                IsDeleteSource = model.IsDeleteSource,
                MaxAmountFiles = model.MaxAmountFiles,
                DublNameJr = model.DublNameJr,
            };

            await _taskRepository.Update(task);

            return new BaseResponse<TaskEntity>()
            {
                Description = "Задача сохранена",
                StatusCode = StatusCode.Ok
            };
            }
            catch (Exception)
            {
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача не сохранена",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }



        public IQueryable<TaskEntity> GetAll()
        {
            return _taskRepository.GetAll();
        }

        public async Task<IBaseResponse<TaskEntity>> Delete(string idTask)
        {
            try
            {
                await _taskRepository.Delete(idTask);
                
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача удалена",
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception)
            {
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача не удалена",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
