using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagementSystem.Models
{
    public class UserMaster
    {
        public int UserId { set; get; }
        [Required]
        public string UserName { set; get; }
        [Required]
        public string Password { set; get; }
    }
}
