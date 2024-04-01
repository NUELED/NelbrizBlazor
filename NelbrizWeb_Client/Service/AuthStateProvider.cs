using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Nelbriz_Common;
using NelbrizWeb_Client.Helper;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace NelbrizWeb_Client.Service
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;   
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(SD.Local_Token);
             if(token == null)
             { 
               return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                // the code below is an hardcoded sample of authentication state.

                 //  return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new[]
                 //  {
                 //     new Claim(ClaimTypes.Name, "nelbriz@gmail.com")
                 //  }, "jwtAuthType")));
             }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token),"jwtAuthType")));
        }



        public void NotifyUserLoggedIn(string token)
        {
            var authenticateduser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticateduser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {        
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity() )));
            NotifyAuthenticationStateChanged(authState);
        }


    }
}
