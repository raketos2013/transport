using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FileManager_Web.Controllers
{
    [Authorize(Roles = "o.br.ДИТ")]
    public class OperationController : Controller
    {
        private readonly ILogger<OperationController> _logger;
        private readonly IOperationService _operationService;
        private readonly IStepService _stepService;
        private readonly IAddresseeService _addresseeService;
        private readonly AppDbContext _appDbContext;

        public OperationController(ILogger<OperationController> logger, 
                                    IOperationService operationService,
                                    IStepService stepService,
                                    IAddresseeService addresseeService,
                                    AppDbContext appDbContext)
        {
            _logger = logger;
            _operationService = operationService;
            _stepService = stepService;
            _addresseeService = addresseeService;
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public IActionResult Operations(string stepId, string operationName)
        {
            TaskOperation? taskOperation;
            ViewBag.OperationName = operationName;
            ViewBag.StepId = stepId;
            ViewBag.AddresseeGroups = _addresseeService.GetAllAddresseeGroups();
            switch (operationName)
            {
                case "Copy":
                    taskOperation = _operationService.GetCopyByStepId(int.Parse(stepId));
                    return PartialView("OperationCopy", taskOperation);
                case "Exist":
                    taskOperation = _operationService.GetExistByStepId(int.Parse(stepId));
                    return PartialView("OperationExist", taskOperation);
                case "Move":
                    taskOperation = _operationService.GetMoveByStepId(int.Parse(stepId));
                    return PartialView("OperationMove", taskOperation);
                case "Read":
                    taskOperation = _operationService.GetReadByStepId(int.Parse(stepId));
                    return PartialView("OperationRead", taskOperation);
                case "Rename":
                    taskOperation = _operationService.GetRenameByStepId(int.Parse(stepId));
                    return PartialView("OperationRename", taskOperation);
                case "Delete":
                    taskOperation = _operationService.GetDeleteByStepId(int.Parse(stepId));
                    return PartialView("OperationDelete", taskOperation);
                case "Clrbuf":
                    taskOperation = _operationService.GetClrbufByStepId(int.Parse(stepId));
                    return PartialView("OperationClrbuf", taskOperation);
                default:
                    break;
            }
            return RedirectToAction("Tasks", "Task");
        }

        public IActionResult CreateOperationCopy(OperationCopyEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = _stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                _operationService.CreateCopy(operationModel);
                OperationCopyEntity? operation = _operationService.GetCopyByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    _stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationCopy(IFormCollection collection, string operationId)
        {
            OperationCopyEntity operation = _appDbContext.OperationCopy.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];
                operation.FileInSource = (FileInSource)Enum.Parse(typeof(FileInSource), collection["FileInSource"]);
                operation.FileInDestination = (FileInDestination)Enum.Parse(typeof(FileInDestination), collection["FileInDestination"]);
                operation.FileInLog = Convert.ToBoolean(collection["FileInLog"].ToString().Split(',')[0]);
                operation.Sort = (SortFiles)Enum.Parse(typeof(SortFiles), collection["Sort"]);
                if (collection["FilesForProcessing"] == "")
                {
                    operation.FilesForProcessing = 0;
                }
                else
                {
                    operation.FilesForProcessing = int.Parse(collection["FilesForProcessing"]);
                }

                operation.FileAttribute = (AttributeFile)Enum.Parse(typeof(AttributeFile), collection["FileAttribute"]);

                _appDbContext.OperationCopy.Update(operation);
                _appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult CreateOperationMove(OperationMoveEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = _stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                _operationService.CreateMove(operationModel);
                OperationMoveEntity? operation = _operationService.GetMoveByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    _stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationMove(IFormCollection collection, string operationId)
        {
            OperationMoveEntity operation = _appDbContext.OperationMove.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];
                operation.FileInDestination = (FileInDestination)Enum.Parse(typeof(FileInDestination), collection["FileInDestination"]);
                operation.FileInLog = Convert.ToBoolean(collection["FileInLog"].ToString().Split(',')[0]);
                operation.Sort = (SortFiles)Enum.Parse(typeof(SortFiles), collection["Sort"]);
                if (collection["FilesForProcessing"] == "")
                {
                    operation.FilesForProcessing = 0;
                }
                else
                {
                    operation.FilesForProcessing = int.Parse(collection["FilesForProcessing"]);
                }

                operation.FileAttribute = (AttributeFile)Enum.Parse(typeof(AttributeFile), collection["FileAttribute"]);

                _appDbContext.OperationMove.Update(operation);
                _appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult CreateOperationDelete(OperationDeleteEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = _stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                _operationService.CreateDelete(operationModel);
                OperationDeleteEntity? operation = _operationService.GetDeleteByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    _stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationDelete(IFormCollection collection, string operationId)
        {
            OperationDeleteEntity operation = _appDbContext.OperationDelete.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];


                _appDbContext.OperationDelete.Update(operation);
                _appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }


        public IActionResult CreateOperationRead(OperationReadEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = _stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                _operationService.CreateRead(operationModel);
                OperationReadEntity? operation = _operationService.GetReadByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    _stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationRead(IFormCollection collection, string operationId)
        {
            OperationReadEntity operation = _appDbContext.OperationRead.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];
                operation.FileInSource = (FileInSource)Enum.Parse(typeof(FileInDestination), collection["FileInSource"]);
                operation.Encode = (Encode)Enum.Parse(typeof(Encode), collection["Encode"]);
                operation.SearchRegex = Convert.ToBoolean(collection["SearchRegex"].ToString().Split(',')[0]);
                operation.FindString = collection["FindString"];
                operation.ExpectedResult = (ExpectedResult)Enum.Parse(typeof(ExpectedResult), collection["ExpectedResult"]);
                operation.BreakTaskAfterError = Convert.ToBoolean(collection["BreakTaskAfterError"].ToString().Split(',')[0]);

                _appDbContext.OperationRead.Update(operation);
                _appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }


        public IActionResult CreateOperationExist(OperationExistEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = _stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                _operationService.CreateExist(operationModel);
                OperationExistEntity? operation = _operationService.GetExistByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    _stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationExist(IFormCollection collection, string operationId)
        {
            OperationExistEntity operation = _appDbContext.OperationExist.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];
                operation.ExpectedResult = (ExpectedResult)Enum.Parse(typeof(ExpectedResult), collection["ExpectedResult"]);
                operation.BreakTaskAfterError = Convert.ToBoolean(collection["BreakTaskAfterError"].ToString().Split(',')[0]);

                _appDbContext.OperationExist.Update(operation);
                _appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }


        public IActionResult CreateOperationRename(OperationRenameEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = _stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                _operationService.CreateRename(operationModel);
                OperationRenameEntity? operation = _operationService.GetRenameByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    _stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationRename(IFormCollection collection, string operationId)
        {
            OperationRenameEntity operation = _appDbContext.OperationRename.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];
                operation.OldPattern = collection["OldPattern"];
                operation.NewPattern = collection["NewPattern"];

                _appDbContext.OperationRename.Update(operation);
                _appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult CreateOperationClrbuf(OperationClrbufEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = _stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                _operationService.CreateClrbuf(operationModel);
                OperationClrbufEntity? operation = _operationService.GetClrbufByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    _stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationClrbuf(IFormCollection collection, string operationId)
        {
            OperationClrbufEntity operation = _appDbContext.OperationClrbuf.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = _appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];


                _appDbContext.OperationClrbuf.Update(operation);
                _appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }


    }
}
