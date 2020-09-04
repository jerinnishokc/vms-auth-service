using System;
using System.Collections.Generic;

namespace VMS_Auth_Service.Models
{
    public partial class User
    {
        public int Id { get; set; }
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Type { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
