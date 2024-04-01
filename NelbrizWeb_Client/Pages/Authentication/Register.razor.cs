using Microsoft.AspNetCore.Components;
using Nelbriz_Models;
using NelbrizWeb_Client.Service.IService;
using System;

namespace NelbrizWeb_Client.Pages.Authentication
{
    public partial class Register
    {


        private SignUpRequestDTO signUpRequest = new();
        public bool IsProcessing { get; set; } = false;
        public bool ShowRegistrationErrors { get; set; }
        public IEnumerable<string> Errors { get; set; }

        [Inject]
        public IAuthenticationService _authService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }


        private async Task RegisterUser()
        {
            ShowRegistrationErrors = false;
            IsProcessing = true;
            var result = await _authService.RegisterUser(signUpRequest);
            if (result.IsRegistrationSuccessfull)
            {
                //registration is successfull
                _navigationManager.NavigateTo("/login");
            }
            else
            {
                //registration is successfull
                Errors = result.Errors;
                ShowRegistrationErrors = true;

            }
            IsProcessing = false;
        }



    }
}
