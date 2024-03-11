using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelbriz_Models
{
    public class SignUpResponseDTO
    {
        public bool IsRegistrationSuccessfull {  get; set; }    
        public IEnumerable<string> Errors { get; set; }
    }
}
