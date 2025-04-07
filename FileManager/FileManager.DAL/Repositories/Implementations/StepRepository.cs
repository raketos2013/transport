using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using System.Threading.Tasks;


namespace FileManager.DAL.Repositories.Implementations
{
	public class StepRepository : IStepRepository
	{
		private readonly AppDbContext _appDbContext;

		public StepRepository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

        public bool ActivatedStep(int stepId)
		{
			try
			{
				TaskStepEntity? step = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId ==stepId);
				
				if (step != null)
				{
					TaskEntity? task = _appDbContext.Task.FirstOrDefault(x => x.TaskId == step.TaskId);
					if (task != null)
					{
						step.IsActive = !step.IsActive;
						_appDbContext.TaskStep.Update(step);
						_appDbContext.SaveChanges();
						task.LastModified = DateTime.Now;
						_appDbContext.Task.Update(task);
						_appDbContext.SaveChanges();
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
                foreach (var step in steps)
                {
                    if (step.StepNumber >= taskStep.StepNumber)
                    {
                        step.StepNumber++;
                    }
                }
                _appDbContext.TaskStep.UpdateRange(steps);
                _appDbContext.TaskStep.Add(taskStep);
				_appDbContext.SaveChanges();
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
				_appDbContext.TaskStep.Update(taskStep);
				_appDbContext.SaveChanges();
                return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public List<TaskStepEntity> GetAllSteps()
		{
			return _appDbContext.TaskStep.ToList();
		}

		public List<TaskStepEntity> GetAllStepsByTaskId(string taskId)
		{
			return _appDbContext.TaskStep.Where(x => x.TaskId == taskId).ToList();
		}

        public TaskStepEntity? GetStepByStepId(int stepId)
        {
			return _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == stepId);
        }

        public TaskStepEntity? GetStepByTaskId(string taskId, int stepNumber)
		{
			return _appDbContext.TaskStep.FirstOrDefault(x => x.TaskId == taskId &&
																x.StepNumber == stepNumber);
		}

        public bool UpdateRangeSteps(List<TaskStepEntity> steps)
        {
			try
			{
				_appDbContext.TaskStep.UpdateRange(steps);
				_appDbContext.SaveChanges();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
        }
    }
}
