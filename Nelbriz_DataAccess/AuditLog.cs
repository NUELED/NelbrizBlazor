using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelbriz_DataAccess
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string EntityName { get; set; }
        [Required]
        public string Action { get; set; }    

        public DateTime TimeStamp { get; set; }
        [Required]
        public  string Changes { get; set; }    
    }
}
