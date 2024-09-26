using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSecureCoding1.Models
{
    public class Students
    {
        [Key]
        public string Nim { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}