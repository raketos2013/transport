using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager_Server.Operations
{
    public interface IStepOperation
    {
        void SetNext(IStepOperation nextStep);
        void Execute(List<string>? bufferFiles);
    }
}
