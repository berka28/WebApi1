using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi1.Models
{
    public class LogInResponseModel
    {
        public bool Success { get; set; }

        public LogInResponseResult Result { get; set; }
    }

    public class LogInResponseResult
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string AccessToken { get; set; }
    }
}
