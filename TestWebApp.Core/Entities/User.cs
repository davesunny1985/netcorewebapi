using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Core.Entities
{
    public class User : IEntityBase
    {
        public User()
        {
            
        }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
