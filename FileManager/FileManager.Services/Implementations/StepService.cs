using FileManager.DAL.Repositories.Interfaces;
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
		public StepService(IStepRepository stepRepository)
		{
			_stepRepository = stepRepository;
		}
	}
}
