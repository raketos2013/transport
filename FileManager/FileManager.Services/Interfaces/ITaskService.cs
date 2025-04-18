﻿using FileManager.Domain.Entity;
using FileManager.Domain.ViewModels.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Services.Interfaces
{
    public interface ITaskService
    {
		List<TaskEntity> GetAllTasks();
		TaskEntity GetTaskById(string idTask);
		bool CreateTask(TaskEntity task);
		bool EditTask(TaskEntity task);
		bool DeleteTask(string idTask);
		List<TaskEntity> GetTasksByGroup(string nameGroup);
		List<TaskGroupEntity> GetAllGroups();
		bool UpdateLastModifiedTask(string idTask);
		bool CreateTaskGroup(string name);
		bool DeleteTaskGroup(int idGroup);
		bool ActivatedTask(string idTask);
		bool CopyTask(string idTask, string newIdTask, string isCopySteps,
                                        CopyStepViewModel[] copyStep);

		bool CreateTaskStatuse(string idTask);
	}
}
