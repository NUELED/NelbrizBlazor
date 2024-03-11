using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nelbriz_Models
{
    public class SignInResponseDTO
    {
        public bool IsAuthSuccessfull {  get; set; }    
        public string ErrorMessage { get; set; }
        public string Token { get; set; }
        public UserDTO userDTO { get; set; }    
    }
}
