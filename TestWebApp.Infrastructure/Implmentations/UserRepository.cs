using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApp.Core.Entities;
using TestWebApp.Infrastructure.Interfaces;

namespace TestWebApp.Infrastructure.Implmentations
{
    public class UserRepository : EntityBaseRepository<User>, IUserRepository
    {
        public UserRepository(TestWebContext context)
            : base(context)
        { }
    }
}
