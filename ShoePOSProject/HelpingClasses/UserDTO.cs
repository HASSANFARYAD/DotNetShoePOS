using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.HelpingClasses
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string EncryptedId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public string CreatedBy { get; set; }
        public int? isActive { get; set; }
    }
}