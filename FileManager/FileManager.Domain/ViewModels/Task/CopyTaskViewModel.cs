using FileManager.Domain.ViewModels.Step;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.ViewModels.Task
{
    public class CopyTaskViewModel
    {
        public string TaskId { get; set; }
        public List<CopyStepViewModel> CopySteps { get; set; }

    }
}
