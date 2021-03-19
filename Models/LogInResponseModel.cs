using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi1.Models
{
    public class LogInResponseModel
    {
        public bool Success { get; set; }

        public dynamic Result { get; set; }
    }
}
