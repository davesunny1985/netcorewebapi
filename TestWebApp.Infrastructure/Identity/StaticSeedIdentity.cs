using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Infrastructure.Identity
{
    public class StaticSeedIdentity
    {
        private TestWebContext _context;
        private UserManager<ApplicationUser> _mgr;


        public StaticSeedIdentity(TestWebContext context)
        {
            _context = context;
            //context = (TestWebContext)serviceProvider.GetService(typeof(TestWebContext));
        }

        public async void SeedUserRoles(string FirstName, string LastName, string UserName, string Email, string Password, string Role="admin")
        {
            var user = new ApplicationUser
            {
                UserName = UserName,
                NormalizedUserName = UserName.ToLower(),
                Email = Email,
                NormalizedEmail = Email.ToLower(),
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString(),
                //FirstName = FirstName,
               // LastName = LastName,

            };

            var roleStore = new RoleStore<IdentityRole>(_context);
            IdentityResult tResult=new IdentityResult();
            List<Task> tasks = new List<Task>();
            //bool chkUser = true;
            if (!_context.Roles.Any(r => r.Name == Role))
            {
                tasks.Add(roleStore.CreateAsync(new IdentityRole { Name = Role, NormalizedName = Role.ToLower() }));
                //chkUser = false;
            }
            
            if (!_context.Users.Any(u => u.UserName == user.UserName))
                {
                    var password = new PasswordHasher<ApplicationUser>();
                    var hashed = password.HashPassword(user, Password);
                    user.PasswordHash = hashed;
                    var userStore = new UserStore<ApplicationUser>(_context);
                tasks.Add(userStore.CreateAsync(user));
                tasks.Add(userStore.AddToRoleAsync(user, Role));
                }

            Task.WaitAll(tasks.ToArray());
            await _context.SaveChangesAsync();

            
        }
    }
}
