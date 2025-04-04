using FileManager.DAL;
using FileManager.Domain.Entity;
using FileManager.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FileManager_Server
{
    class TaskLogger : ITaskLogger
    {
		private readonly AppDbContext _appDbContext;
		public TaskLogger(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		public void OperationLog(TaskStepEntity step)
		{
			if (step.OperationId == 0) 
			{
				return;
			}
			TaskLogEntity taskLog = new TaskLogEntity();
			taskLog.DateTimeLog = DateTime.Now;
			taskLog.OperationId = step.OperationId;
			taskLog.StepId = step.StepId;
			taskLog.TaskId = step.TaskId;
			TaskOperation operation;
			string text;
			switch (step.OperationName)
			{
				case OperationName.Copy:
					operation = _appDbContext.OperationCopy.FirstOrDefault();
					if (operation != null)
					{
						text = "Свойства операции - ";
						_appDbContext.TaskLog.Add(taskLog);
						text = "Сортировка - ";
						_appDbContext.TaskLog.Add(taskLog);
						text = "Контроль дублирования по журналу - ";
						_appDbContext.TaskLog.Add(taskLog);
						text = "Контроль Файл есть в назначении - ";
						_appDbContext.TaskLog.Add(taskLog);
					}
					break;
				case OperationName.Move:
					operation = _appDbContext.OperationMove.FirstOrDefault();
					break;
				case OperationName.Read:
					operation = _appDbContext.OperationRead.FirstOrDefault();
					if (operation != null)
					{
						text = "";
					}
					break;
				case OperationName.Exist:
					operation = _appDbContext.OperationExist.FirstOrDefault();
					break;
				case OperationName.Rename:
					operation = _appDbContext.OperationRename.FirstOrDefault();
					break;
				case OperationName.Delete:
					operation = _appDbContext.OperationDelete.FirstOrDefault();
					break;
			}
			

			
			_appDbContext.SaveChanges();
		}

		public void StepLog(TaskStepEntity step, string text, string filename = "")
		{
			TaskLogEntity taskLog = new TaskLogEntity();
			taskLog.DateTimeLog = DateTime.Now;
			taskLog.OperationId = step.OperationId;
			taskLog.ResultText = text;
			taskLog.StepId = step.StepId;
			taskLog.TaskId = step.TaskId;
			if(filename != "")
			{
				taskLog.FileName = filename;
			}
			taskLog.ResultOperation = ResultOperation.Success;

			_appDbContext.TaskLog.Add(taskLog);
			_appDbContext.SaveChanges();
		}

		public void TaskLog(string TaskId, string text)
		{
			TaskLogEntity taskLog = new TaskLogEntity();
			taskLog.DateTimeLog = DateTime.Now;
			taskLog.TaskId= TaskId;
			taskLog.ResultText = text;

			_appDbContext.TaskLog.Add(taskLog);
			_appDbContext.SaveChanges();
		}
	}
}
