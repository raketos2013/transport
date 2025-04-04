using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.DAL.Repositories.Interfaces
{
    public interface ITaskLogRepository
    {
        List<TaskLogEntity> GetLogsByTaskId(string taskId);
        bool AddTaskLog(TaskLogEntity taskLog);
    }
}
