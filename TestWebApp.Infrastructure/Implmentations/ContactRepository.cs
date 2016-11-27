using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWebApp.Core.Entities;
using TestWebApp.Infrastructure.Interfaces;

namespace TestWebApp.Infrastructure.Implmentations
{
    public class ContactRepository : EntityBaseRepository<Contact>, IContactRepository
    {
        public ContactRepository(TestWebContext context)
            : base(context)
        { }
    }
}
