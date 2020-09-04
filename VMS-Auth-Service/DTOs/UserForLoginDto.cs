using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VMS_Auth_Service.DTOs
{
    public class UserForLoginDto
    {
        public string Email { get; set; }
        public string Type { get; set; }
        public string Password { get; set; }
    }
}
