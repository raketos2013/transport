using FileManager.Domain.Entity;

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
