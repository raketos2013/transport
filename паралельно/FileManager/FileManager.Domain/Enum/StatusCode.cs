using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager.Domain.Enum
{
    public enum StatusCode
    {
        TaskIsHasAlready = 1,

        Ok = 200,
        InternalServerError = 500
    }
}
