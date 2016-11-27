using TestWebApp.Core.Entities;

namespace TestWebApp.Infrastructure.Interfaces
{
    public interface IUserRepository : IEntityBaseRepository<User> { }
    public interface IContactRepository : IEntityBaseRepository<Contact> { }
}
