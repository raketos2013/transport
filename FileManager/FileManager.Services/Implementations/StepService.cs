using FileManager.DAL;
using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.Implementations
{
    public class StepService : IStepService
    {
		private readonly IStepRepository _stepRepository;
        private readonly ITaskService _taskService;
		public StepService(IStepRepository stepRepository, ITaskService taskService)
		{
			_stepRepository = stepRepository;
            _taskService = taskService;
		}

        public bool CreateStep(TaskStepEntity taskStep)
        {
            _stepRepository.CreateStep(taskStep);
            _taskService.UpdateLastModifiedTask(taskStep.TaskId);
            return _stepRepository.CreateStep(taskStep);
        }

        public bool EditStep(TaskStepEntity taskStep)
        {
            return _stepRepository.EditStep(taskStep);
        }

        public List<TaskStepEntity> GetAllSteps()
        {
            return _stepRepository.GetAllSteps();
        }

        public List<TaskStepEntity> GetAllStepsByTaskId(string taskId)
        {
            return _stepRepository.GetAllStepsByTaskId(taskId);
        }

        public TaskStepEntity? GetStepByTaskId(string taskId, int stepNumber)
        {
            return _stepRepository.GetStepByTaskId(taskId, stepNumber);
        }

        public bool ReplaceSteps(string taskId, string numberStep, string operation)
        {
            try
            {
                List<TaskStepEntity> steps = _stepRepository.GetAllStepsByTaskId(taskId).OrderBy(x => x.StepNumber).ToList();
                TaskStepEntity step1, step2, tmpStep;
                switch (operation)
                {
                    case "up":
                        if (int.Parse(numberStep) > 1)
                        {
                            step1 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep));
                            step2 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep) - 1);
                            step1.StepNumber = int.Parse(numberStep) - 1;
                            step2.StepNumber = int.Parse(numberStep);
                        }
                        break;
                    case "down":
                        if (int.Parse(numberStep) < steps.Count)
                        {
                            step1 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep));
                            step2 = steps.FirstOrDefault(x => x.StepNumber == int.Parse(numberStep) + 1);
                            step1.StepNumber = int.Parse(numberStep) + 1;
                            step2.StepNumber = int.Parse(numberStep);
                        }
                        break;
                    case "maxup":
                        if (int.Parse(numberStep) > 1)
                        {
                            for (int i = int.Parse(numberStep) - 1; i > 0; i--)
                            {
                                steps[i].StepNumber = i;
                                steps[i - 1].StepNumber = i + 1;
                                tmpStep = steps[i];
                                steps[i] = steps[i - 1];
                                steps[i - 1] = tmpStep;
                            }
                        }
                        break;
                    case "maxdown":
                        if (int.Parse(numberStep) < steps.Count)
                        {
                            for (int i = int.Parse(numberStep) - 1; i < steps.Count - 1; i++)
                            {
                                steps[i].StepNumber = i + 2;
                                steps[i + 1].StepNumber = i + 1;
                                tmpStep = steps[i];
                                steps[i] = steps[i + 1];
                                steps[i + 1] = tmpStep;
                            }
                        }
                        break;
                    default:
                        break;
                }
                
                return _stepRepository.UpdateRangeSteps(steps);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
