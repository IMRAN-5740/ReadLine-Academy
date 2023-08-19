using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReadLineAcademy.Databases.Data;
using ReadLineAcademy.Databases.DbInitializer.Base;
using ReadLineAcademy.Models.AuthModels;
using ReadLineAcademy.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Databases.DbInitializer
{
    public class DbInitialize : IDbInitialize
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public DbInitialize(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        public async void Initialize()
        {
            //Migration if they are not applied
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception)
            {

                throw;
            }
            //Create Roles if they are not Created
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();
                //If Roles are not Created then we will Create admin user role as well.

                var isUser = _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin@readlineacademy.com",
                    Email = "admin@readlineacademy.com",
                    Name = "ReadLine Academy",
                    PhoneNumber = "+8801516003435",
                    StreetAddress = "Satana Baluya Darbasto,Gobindaganj Bangladesh",
                    State = "Rangpur",
                    PostalCode = "5740",
                    City = "Gobindaganj"
                }, "RA1234@").GetAwaiter().GetResult();
                _context.SaveChanges();
                ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(data => data.Email == "admin@readlineacademy.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;


        }
    }
}
