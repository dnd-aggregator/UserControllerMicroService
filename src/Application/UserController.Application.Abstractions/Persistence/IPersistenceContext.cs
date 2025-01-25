using UserController.Application.Abstractions.Persistence.Repositories;

namespace UserController.Application.Abstractions.Persistence;

public interface IPersistenceContext
{
    public IUserRepository UserRepository { get; }

    public ICharacterRepository CharacterRepository { get; }
}