using FileManager.Domain.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.ViewModels.Task
{
    public class CreateTaskViewModel
    {
        public string TaskId { get; set; }       
        public string Name { get; set; }
        public TimeOnly TimeBegin { get; set; }
        public TimeOnly TimeEnd { get; set; }
        public DayActive DayActive { get; set; }
        public int Group { get; set; }
        public bool IsActive { get; set; }
        public string SourceCatalog { get; set; }
        public string FileMask { get; set; }
        public TimeOnly Delay { get; set; }
        public string ArchiveCatalog { get; set; }
        public string BadArchiveCatalog { get; set; }
        public bool IsDeleteSource { get; set; }
        public int MaxAmountFiles { get; set; }
        public bool DublNameJr { get; set; }
    }
}
