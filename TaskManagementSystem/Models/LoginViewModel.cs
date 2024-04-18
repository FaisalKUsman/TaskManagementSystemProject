using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagementSystem.Models
{
    public class LoginViewModel
    {
        public int Id { set; get; }
        [Required]
        public string UserName { set; get; }
        [Required]
        public string Password { set; get; }
    }
}
