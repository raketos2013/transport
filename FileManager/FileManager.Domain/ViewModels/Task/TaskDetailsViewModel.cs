using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.ViewModels.Task
{
    public class TaskDetailsViewModel
    {
        public TaskEntity Task { get; set; }
        public IEnumerable<TaskStepEntity> TaskSteps { get; set; }

        public TaskDetailsViewModel()
        {
                
        }

        public TaskDetailsViewModel(TaskEntity task, IEnumerable<TaskStepEntity> taskSteps)
        {
            Task = task;
            TaskSteps = taskSteps;
        }
    }
}
