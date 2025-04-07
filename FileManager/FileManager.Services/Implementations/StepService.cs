using FileManager.DAL;
using FileManager.DAL.Repositories.Interfaces;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
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
        private readonly IOperationRepository _operationRepository;
		public StepService(IStepRepository stepRepository, 
                            ITaskService taskService, 
                            IOperationRepository operationRepository)
		{
			_stepRepository = stepRepository;
            _taskService = taskService;
            _operationRepository = operationRepository;
		}

		public bool ActivatedStep(int stepId)
		{
			return _stepRepository.ActivatedStep(stepId);
		}

		public bool CreateStep(TaskStepEntity taskStep)
        {
            _stepRepository.CreateStep(taskStep);
            _taskService.UpdateLastModifiedTask(taskStep.TaskId);
            return _stepRepository.CreateStep(taskStep);
        }

        public bool EditStep(TaskStepEntity taskStep)
        {
            TaskStepEntity? step = _stepRepository.GetStepByTaskId(taskStep.TaskId, taskStep.StepNumber);
            if (step == null)
            {
                return false;
            }
            step.Description = taskStep.Description;
            step.Source = taskStep.Source;
            step.Destination = taskStep.Destination;
            step.FileMask = taskStep.FileMask;
            step.IsActive = taskStep.IsActive;
            step.IsBreak = taskStep.IsBreak;
            if (taskStep.OperationName != step.OperationName)
            {
                step.OperationName = taskStep.OperationName;
                switch (step.OperationName)
                {
                    case OperationName.Copy:
                        OperationCopyEntity? copy = _operationRepository.GetCopyByStepId(step.StepId);
                        if (copy != null)
                        {
                            _operationRepository.DeleteCopy(copy);
                        }
                        break;
                    case OperationName.Move:
                        OperationMoveEntity? move = _operationRepository.GetMoveByStepId(step.StepId);
                        if (move != null)
                        {
                            _operationRepository.DeleteMove(move);
                        }
                        break;
                    case OperationName.Read:
                        OperationReadEntity? read = _operationRepository.GetReadByStepId(step.StepId);
                        if (read != null)
                        {
                            _operationRepository.DeleteRead(read);
                        }
                        break;
                    case OperationName.Exist:
                        OperationExistEntity? exist = _operationRepository.GetExistByStepId(step.StepId);
                        if (exist != null)
                        {
                            _operationRepository.DeleteExist(exist);
                        }
                        break;
                    case OperationName.Rename:
                        OperationRenameEntity? rename = _operationRepository.GetRenameByStepId(step.StepId);
                        if (rename != null)
                        {
                            _operationRepository.DeleteRename(rename);
                        }
                        break;
                    case OperationName.Delete:
                        OperationDeleteEntity? delete = _operationRepository.GetDeleteByStepId(step.StepId);
                        if (delete != null)
                        {
                            _operationRepository.DeleteDelete(delete);
                        }
                        break;
                    case OperationName.Clrbuf:
                        OperationClrbufEntity? clrbuf = _operationRepository.GetClrbufByStepId(step.StepId);
                        if (clrbuf != null)
                        {
                            _operationRepository.DeleteClrbuf(clrbuf);
                        }
                        break;
                    default:
                        break;
                }
            }
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

        public TaskStepEntity? GetStepByStepId(int stepId)
        {
            return _stepRepository.GetStepByStepId(stepId);
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
