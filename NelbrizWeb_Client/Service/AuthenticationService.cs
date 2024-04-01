using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Nelbriz_Common;
using Nelbriz_Models;
using NelbrizWeb_Client.Service.IService;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;

namespace NelbrizWeb_Client.Service
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly AuthenticationStateProvider _authStateProvider;  
        private readonly ILocalStorageService _localStorage;

        public AuthenticationService(HttpClient client, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
        {
            _client = client;
            _authStateProvider = authStateProvider; 
            _localStorage = localStorage;   
        }



        public async Task<SignInResponseDTO> Login(SignInRequestDTO signInResponse)
        {
            var content = JsonConvert.SerializeObject(signInResponse);  
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/account/signin", bodyContent);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SignInResponseDTO>(contentTemp);


            if(response.IsSuccessStatusCode)
            {
                await _localStorage.SetItemAsync(SD.Local_Token, result.Token);
                await _localStorage.SetItemAsync(SD.Local_UserDetails, result.userDTO);

                ((AuthStateProvider)_authStateProvider).NotifyUserLoggedIn(result.Token);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("berarer", result.Token);

                return new SignInResponseDTO() { IsAuthSuccessfull = true };
            }
            else
            {
                return result;
            }
        }




        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(SD.Local_Token);
            await _localStorage.RemoveItemAsync(SD.Local_UserDetails);


            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();

            _client.DefaultRequestHeaders.Authorization = null;
        }



        public async Task<SignUpResponseDTO> RegisterUser(SignUpRequestDTO signUpRequest)
        {
            var content = JsonConvert.SerializeObject(signUpRequest);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/account/signup", bodyContent);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SignUpResponseDTO>(contentTemp);


            if (response.IsSuccessStatusCode)
            {
                return new SignUpResponseDTO() { IsRegistrationSuccessfull = true };
            }
            else
            {
                return new SignUpResponseDTO() { IsRegistrationSuccessfull = false, Errors = result.Errors };
            }
        }


    }
}
