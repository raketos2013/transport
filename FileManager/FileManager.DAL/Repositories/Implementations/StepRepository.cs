using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public bool CreateStep(TaskStepEntity taskStep)
		{
			try
			{
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
			throw new NotImplementedException();
		}

		public List<TaskStepEntity> GetAllSteps()
		{
			return _appDbContext.TaskStep.ToList();
		}

		public List<TaskStepEntity> GetAllStepsByTaskId(string taskId)
		{
			return _appDbContext.TaskStep.Where(x => x.TaskId == taskId).ToList();
		}

		public TaskStepEntity GetStepByTaskId(string taskId, int stepNumber)
		{
			return _appDbContext.TaskStep.FirstOrDefault(x => x.TaskId == taskId &&
																x.StepNumber == stepNumber);
		}
	}
}
