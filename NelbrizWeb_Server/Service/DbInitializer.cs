using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nelbriz_Common;
using Nelbriz_DataAccess.Data;
using NelbrizWeb_Server.Service.IService;

namespace NelbrizWeb_Server.Service
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
           _roleManager = roleManager;
           _userManager = userManager; 
            _db = db;   
        }

        public void Initialize()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                { 
                    _db.Database.Migrate(); 
                }
                if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                }
                else
                {
                    return;
                }

                IdentityUser user = new()
                {
                    UserName = "nelbriz@briz.com",
                    Email = "nelbriz@briz.com",
                    EmailConfirmed = true,
                };

                _userManager.CreateAsync(user, "Password1@").GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user,SD.Role_Customer).GetAwaiter().GetResult();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
