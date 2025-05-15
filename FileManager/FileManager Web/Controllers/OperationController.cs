using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using FileManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FileManager_Web.Controllers
{
    //[Authorize(Roles = "o.br.ДИТ")]
    public class OperationController(ILogger<OperationController> logger,
                                        IOperationService operationService,
                                        IStepService stepService,
                                        IAddresseeService addresseeService,
                                        AppDbContext appDbContext) 
                : Controller
    {
        [HttpGet]
        public IActionResult Operations(string stepId, string operationName)
        {
            TaskOperation? taskOperation;
            ViewBag.OperationName = operationName;
            ViewBag.StepId = stepId;
            ViewBag.AddresseeGroups = addresseeService.GetAllAddresseeGroups();
            switch (operationName)
            {
                case "Copy":
                    taskOperation = operationService.GetCopyByStepId(int.Parse(stepId));
                    return PartialView("OperationCopy", taskOperation);
                case "Exist":
                    taskOperation = operationService.GetExistByStepId(int.Parse(stepId));
                    return PartialView("OperationExist", taskOperation);
                case "Move":
                    taskOperation = operationService.GetMoveByStepId(int.Parse(stepId));
                    return PartialView("OperationMove", taskOperation);
                case "Read":
                    taskOperation = operationService.GetReadByStepId(int.Parse(stepId));
                    return PartialView("OperationRead", taskOperation);
                case "Rename":
                    taskOperation = operationService.GetRenameByStepId(int.Parse(stepId));
                    return PartialView("OperationRename", taskOperation);
                case "Delete":
                    taskOperation = operationService.GetDeleteByStepId(int.Parse(stepId));
                    return PartialView("OperationDelete", taskOperation);
                case "Clrbuf":
                    taskOperation = operationService.GetClrbufByStepId(int.Parse(stepId));
                    return PartialView("OperationClrbuf", taskOperation);
                default:
                    break;
            }
            return RedirectToAction("Tasks", "Task");
        }

        public IActionResult CreateOperationCopy(OperationCopyEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                operationService.CreateCopy(operationModel);
                OperationCopyEntity? operation = operationService.GetCopyByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationCopy(IFormCollection collection, string operationId)
        {
            OperationCopyEntity operation = appDbContext.OperationCopy.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

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

                appDbContext.OperationCopy.Update(operation);
                appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult CreateOperationMove(OperationMoveEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                operationService.CreateMove(operationModel);
                OperationMoveEntity? operation = operationService.GetMoveByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationMove(IFormCollection collection, string operationId)
        {
            OperationMoveEntity operation = appDbContext.OperationMove.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

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

                appDbContext.OperationMove.Update(operation);
                appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult CreateOperationDelete(OperationDeleteEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                operationService.CreateDelete(operationModel);
                OperationDeleteEntity? operation = operationService.GetDeleteByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationDelete(IFormCollection collection, string operationId)
        {
            OperationDeleteEntity operation = appDbContext.OperationDelete.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];


                appDbContext.OperationDelete.Update(operation);
                appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }


        public IActionResult CreateOperationRead(OperationReadEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                operationService.CreateRead(operationModel);
                OperationReadEntity? operation = operationService.GetReadByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationRead(IFormCollection collection, string operationId)
        {
            OperationReadEntity operation = appDbContext.OperationRead.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

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

                appDbContext.OperationRead.Update(operation);
                appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }


        public IActionResult CreateOperationExist(OperationExistEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                operationService.CreateExist(operationModel);
                OperationExistEntity? operation = operationService.GetExistByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationExist(IFormCollection collection, string operationId)
        {
            OperationExistEntity operation = appDbContext.OperationExist.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

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

                appDbContext.OperationExist.Update(operation);
                appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }


        public IActionResult CreateOperationRename(OperationRenameEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                operationService.CreateRename(operationModel);
                OperationRenameEntity? operation = operationService.GetRenameByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationRename(IFormCollection collection, string operationId)
        {
            OperationRenameEntity operation = appDbContext.OperationRename.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

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

                appDbContext.OperationRename.Update(operation);
                appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult CreateOperationClrbuf(OperationClrbufEntity operationModel, string stepId)
        {
            TaskStepEntity? taskStep = stepService.GetStepByStepId(int.Parse(stepId));
            if (taskStep == null)
            {
                return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
            }
            if (ModelState.IsValid)
            {
                operationModel.StepId = int.Parse(stepId);
                operationService.CreateClrbuf(operationModel);
                OperationClrbufEntity? operation = operationService.GetClrbufByStepId(operationModel.StepId);
                if (operation != null)
                {
                    taskStep.OperationId = operation.OperationId;
                    stepService.EditStep(taskStep);
                }
            }
            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }

        public IActionResult EditOperationClrbuf(IFormCollection collection, string operationId)
        {
            OperationClrbufEntity operation = appDbContext.OperationClrbuf.FirstOrDefault(x => x.OperationId == int.Parse(operationId));
            TaskStepEntity taskStep = appDbContext.TaskStep.FirstOrDefault(x => x.StepId == operation.StepId);

            if (ModelState.IsValid)
            {
                operation.InformSuccess = Convert.ToBoolean(collection["InformSuccess"].ToString().Split(',')[0]);
                if (operation.InformSuccess)
                {
                    operation.AddresseeGroupId = int.Parse(collection["AddresseeGroupId"]);
                }
                operation.AdditionalText = collection["AdditionalText"];


                appDbContext.OperationClrbuf.Update(operation);
                appDbContext.SaveChanges();

            }


            return RedirectToAction("StepDetails", "Step", new { taskId = taskStep.TaskId, stepNumber = taskStep.StepNumber });
        }


    }
}
