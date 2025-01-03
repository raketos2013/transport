using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.ViewModels
{
    public class CopyTaskOperationsViewModel
    {
        public bool IsCopied { get; set; }
        public string OperationId { get; set; }
        public string NewOperationId { get; set; }

    }
}
