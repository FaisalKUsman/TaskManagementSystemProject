using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagementSystem.Models
{
    public class TaskViewModel
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        public string Description { get; set; }
        
        public DateTime DueDate { get; set; }
        public string CreatedBy { get; set; }
        
        public string Status { get; set; }
        
    }
}
