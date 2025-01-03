using FileManager.Domain.Entity;
using FileManager.Domain.Response;
using FileManager.Domain.ViewModels.Operation;
using FileManager.Domain.ViewModels.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Service.Interfaces
{
    public interface ITaskOperationService
    {
        Task<IBaseResponse<TaskOperationEntity>> Create(TaskOperationEntity taskOperationModel);

        IQueryable<TaskOperationEntity> GetAllForTask(string idTask);

        Task<IBaseResponse<TaskOperationEntity>> Delete(string idTask, string idOperation);

        Task<IBaseResponse<TaskOperationEntity>> Update(TaskOperationEntity taskOperationModel);

    }
}
