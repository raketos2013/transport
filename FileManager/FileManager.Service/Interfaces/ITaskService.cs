using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Response;
using FileManager.Domain.ViewModels.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Service.Interfaces
{
    public interface ITaskService
    {
        Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel taskModel);

        IQueryable<TaskEntity> GetAll();

        Task<IBaseResponse<TaskEntity>> Delete(string idTask);

        Task<IBaseResponse<TaskEntity>> Update(CreateTaskViewModel taskModel);

    }
}
