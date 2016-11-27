using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApp.Core.Entities;
using TestWebApp.Infrastructure.Identity;

namespace TestWebApp.Infrastructure
{
    public class TestDbInitializer
    {
        private static TestWebContext context;
        private static UserManager<ApplicationUser> _userManager;
        private static RoleManager<IdentityRole> _roleManager;
        public static void Initialize(IServiceProvider serviceProvider)
        {
            context = (TestWebContext)serviceProvider.GetService(typeof(TestWebContext));
            _userManager = (UserManager<ApplicationUser>)serviceProvider.GetService(typeof(UserManager<ApplicationUser>));
            _roleManager = (RoleManager<IdentityRole>)serviceProvider.GetService(typeof(RoleManager<IdentityRole>));

            InitializeTables();
            var task = SeedUserRoles();
        }

        public static async Task SeedUserRoles()
        {
            if (await _userManager.FindByEmailAsync("someone@someone.com") == null)
            {
                ApplicationUser administrator = new ApplicationUser
                {
                    UserName = "someone@someone.com",
                    Email = "someone@someone.com"
                };

                await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                await _userManager.CreateAsync(administrator, "Password123!");

                IdentityResult result = await _userManager.AddToRoleAsync(administrator, "Administrator");
            }
        }

        private static void InitializeTables()
        {
            //var _userManager = new UserStore<ApplicationUser>(context);

            //if(!context.Users.Any())
            //{

            //}

            //if (!context.TestUsers.Any())
            //{
            // User user_01 = new User { Id=1, Name = "Chris Sakellarios" };

            //    User user_02 = new User { Name = "Charlene Campbell", Profession = "Web Designer", Avatar = "avatar_03.jpg" };

            //    User user_03 = new User { Name = "Mattie Lyons", Profession = "Engineer", Avatar = "avatar_05.png" };

            //    User user_04 = new User { Name = "Kelly Alvarez", Profession = "Network Engineer", Avatar = "avatar_01.png" };

            //    User user_05 = new User { Name = "Charlie Cox", Profession = "Developer", Avatar = "avatar_03.jpg" };

            //    User user_06 = new User { Name = "Megan	Fox", Profession = "Hacker", Avatar = "avatar_05.png" };
            //if (_userManager.FindByIdAsync("1")==null)
            //    context.Users.Add(new ApplicationUser() { Id="1",UserName="User1" });
            //context.Users.Add(user_02);
            //    context.Users.Add(user_03); context.Users.Add(user_04);
            //    context.Users.Add(user_05); context.Users.Add(user_06);

            //    context.SaveChanges();
            //}

            //context.SaveChanges();
        }
    }
}
