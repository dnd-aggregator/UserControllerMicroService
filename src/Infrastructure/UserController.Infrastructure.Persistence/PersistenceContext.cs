using UserController.Application.Abstractions.Persistence;
using UserController.Application.Abstractions.Persistence.Repositories;

namespace UserController.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(IUserRepository userRepository)
    {
        UserRepository = userRepository;
    }

    public IUserRepository UserRepository { get; }
}