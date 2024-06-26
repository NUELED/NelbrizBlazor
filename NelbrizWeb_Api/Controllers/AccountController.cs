﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Nelbriz_Common;
using Nelbriz_DataAccess;
using Nelbriz_Models;
using NelbrizWeb_Api.Helper;
using NelbrizWeb_Api.Logging;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NelbrizWeb_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly APISettings _aPISettings;
        private readonly ILogger<AccountController>  _logger;
        private readonly ILogging _customLogger; //For testing Sake



        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
             RoleManager<IdentityRole> roleManager,
             IOptions<APISettings> options,
             ILogger<AccountController> logger,
             ILogging customLogger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _aPISettings = options.Value;
            _logger = logger;
            _customLogger = customLogger;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDTO signUpRequestDTO)
        {
            if(signUpRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new ApplicationUser
            {
                UserName = signUpRequestDTO.Email,
                Email = signUpRequestDTO.Email, 
                Name = signUpRequestDTO.Name,
                PhoneNumber = signUpRequestDTO.PhoneNumber, 
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, signUpRequestDTO.Password);

            if(!result.Succeeded)
            {
                return BadRequest(new SignUpResponseDTO()
                {
                    IsRegistrationSuccessfull = false,
                    Errors = result.Errors.Select(u => u.Description)
                }); 
            }

            _customLogger.Log("Adding a role to the newly created User", ""); // Just for testing
            _logger.LogInformation("Adding a role to the newly created User");
            var roleResult = await _userManager.AddToRoleAsync(user, SD.Role_Customer);
            if (!roleResult.Succeeded)
            {
                return BadRequest(new SignUpResponseDTO()
                {
                    IsRegistrationSuccessfull = false,
                    Errors = result.Errors.Select(u => u.Description)
                });
            }

            return StatusCode(201);
        }




        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO signInRequestDTO)
        {
            if (signInRequestDTO == null || !ModelState.IsValid)
            {
                return BadRequest();
            }



            var result = await _signInManager.PasswordSignInAsync(signInRequestDTO.UserName, signInRequestDTO.Password, false, false);
            if (result.Succeeded)
            {
                 var user = await _userManager.FindByNameAsync(signInRequestDTO.UserName);  
                if (user == null)
                {
                    return Unauthorized(new SignInResponseDTO()
                    {
                        IsAuthSuccessfull = false,
                        ErrorMessage = "Invalid Authentication"
                    });
                }
                //-----------------
                var signInCredentials = GetSigningCredentials();
                var myclaims = await  GetClaims(user);

                var tokenOptions = new JwtSecurityToken(
                     issuer: _aPISettings.ValidIssuer,
                     audience: _aPISettings.ValidAudience,
                     claims: myclaims,
                     expires:DateTime.Now.AddDays(7),
                     signingCredentials: signInCredentials
                    );

                var tokens = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

             
                _logger.LogInformation("returning the user details and the token.");
                return Ok(new SignInResponseDTO()
                {
                     IsAuthSuccessfull= true,
                     Token = tokens,
                     userDTO = new UserDTO()
                     {
                         Name= user.Name,
                         Id = user.Id,  
                         Email = user.Email,    
                         PhoneNumber = user.PhoneNumber,                          
                     }
                });

            }
            else
            {
                _customLogger.Log2("Oops........There was an issue authorizing at this point.", "error"); // Just for testing
                _logger.LogError("There was an issue authorizing at this point.");
                return Unauthorized(new SignInResponseDTO()
                {
                    IsAuthSuccessfull = false,
                    ErrorMessage = "Invalid Authentication"
                });
            }
         
           // return StatusCode(201);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_aPISettings.ValidKey));  
            
            return new SigningCredentials(secret,SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Email),  
                new Claim(ClaimTypes.Email, user.Email),  
                new Claim("Id", user.Id),  
            };

            var roles = await  _userManager.GetRolesAsync( await _userManager.FindByEmailAsync(user.Email) );   
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));    
            }

            return claims;
        }


    }
}
