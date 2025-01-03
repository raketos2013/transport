using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server
{
    public class JsonResponse
    {
        public int Code { get; set; } = 0;
        public string ExceptionName { get; set; } = "";
        public string[]? Message { get; set; }
        public string Path { get; set; } = "";
    }
}
