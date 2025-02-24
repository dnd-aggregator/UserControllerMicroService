using UserController.Application.Abstractions.Persistence;
using UserController.Application.Abstractions.Persistence.Repositories;

namespace UserController.Infrastructure.Persistence;

public class PersistenceContext : IPersistenceContext
{
    public PersistenceContext(IUserRepository userRepository, ICharacterRepository characterRepository)
    {
        UserRepository = userRepository;
        CharacterRepository = characterRepository;
    }

    public ICharacterRepository CharacterRepository { get; }

    public IUserRepository UserRepository { get; }
}