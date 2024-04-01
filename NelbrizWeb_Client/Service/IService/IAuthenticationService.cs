using Microsoft.AspNetCore.Identity;
using Nelbriz_Models;

namespace NelbrizWeb_Client.Service.IService
{
    public interface IAuthenticationService
    {
       Task<SignUpResponseDTO> RegisterUser(SignUpRequestDTO signUpRequestDTO);
       Task<SignInResponseDTO> Login(SignInRequestDTO signInResponseDTO);

        Task Logout();
    }
}
