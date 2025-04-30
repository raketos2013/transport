using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;


namespace FileManager.DAL.Repositories.Implementations
{
	public class StepRepository(AppDbContext appDbContext) : IStepRepository
	{
        public bool ActivatedStep(int stepId)
		{
			try
			{
				TaskStepEntity? step = appDbContext.TaskStep.FirstOrDefault(x => x.StepId ==stepId);
				
				if (step != null)
				{
					TaskEntity? task = appDbContext.Task.FirstOrDefault(x => x.TaskId == step.TaskId);
					if (task != null)
					{
						step.IsActive = !step.IsActive;
						appDbContext.TaskStep.Update(step);
						appDbContext.SaveChanges();
						task.LastModified = DateTime.Now;
						appDbContext.Task.Update(task);
						appDbContext.SaveChanges();
						return true;
					}
				}
				return false;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool CreateStep(TaskStepEntity taskStep)
		{
			try
			{

                List<TaskStepEntity> steps = GetAllStepsByTaskId(taskStep.TaskId).OrderBy(x => x.StepNumber).ToList();
				if (steps.Count != 0)
				{
                    foreach (var step in steps)
                    {
                        if (step.StepNumber >= taskStep.StepNumber)
                        {
                            step.StepNumber++;
                        }
                    }
                    appDbContext.TaskStep.UpdateRange(steps);
                }
                
                appDbContext.TaskStep.Add(taskStep);
				appDbContext.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool EditStep(TaskStepEntity taskStep)
		{
			try
			{
				appDbContext.TaskStep.Update(taskStep);
				appDbContext.SaveChanges();
                return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public List<TaskStepEntity> GetAllSteps()
		{
			return appDbContext.TaskStep.ToList();
		}

		public List<TaskStepEntity> GetAllStepsByTaskId(string taskId)
		{
			return appDbContext.TaskStep.Where(x => x.TaskId == taskId).ToList();
		}

        public TaskStepEntity? GetStepByStepId(int stepId)
        {
			return appDbContext.TaskStep.FirstOrDefault(x => x.StepId == stepId);
        }

        public TaskStepEntity? GetStepByTaskId(string taskId, int stepNumber)
		{
			return appDbContext.TaskStep.FirstOrDefault(x => x.TaskId == taskId &&
																x.StepNumber == stepNumber);
		}

        public bool UpdateRangeSteps(List<TaskStepEntity> steps)
        {
			try
			{
				appDbContext.TaskStep.UpdateRange(steps);
				appDbContext.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
        }
    }
}
