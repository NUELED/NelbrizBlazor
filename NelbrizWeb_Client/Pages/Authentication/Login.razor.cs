using Microsoft.AspNetCore.Components;
using Nelbriz_Models;
using NelbrizWeb_Client.Service.IService;
using System.Web;

namespace NelbrizWeb_Client.Pages.Authentication
{
    public partial class Login
    {

        private SignInRequestDTO signInRequest = new();
        public bool IsProcessing { get; set; } = false;
        public bool ShowSignInErrors { get; set; }
        public string Errors { get; set; }
        public string ReturnUrl { get; set; }

        [Inject]
        public IAuthenticationService _authService { get; set; }
        [Inject]
        public NavigationManager _navigationManager { get; set; }



        private async Task LoginUser()
        {
            ShowSignInErrors = false;
            IsProcessing = true;
            var result = await _authService.Login(signInRequest);
           
            if (result.IsAuthSuccessfull)
            {
                //registration is successfull
                var absoluteUri = new Uri(_navigationManager.Uri);
                var querryParam = HttpUtility.ParseQueryString(absoluteUri.Query);
                ReturnUrl = querryParam["returnUrl"];
                if(string.IsNullOrEmpty(ReturnUrl)) 
                {
                    _navigationManager.NavigateTo("/");
                }
                else
                {
                    _navigationManager.NavigateTo("/" + ReturnUrl);
                }
               
            }
            else
            {
                //registration is successfull
                Errors = result.ErrorMessage;
                ShowSignInErrors = true;

            }
            IsProcessing = false;
        }



    }
}
