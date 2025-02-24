namespace UserController.Application.Models;

public class UserModel
{
    private readonly List<CharacterModel> _characters;

    public UserModel(string name, string phoneNumber)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        _characters = new List<CharacterModel>();
    }

    public long Id { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }

    public void AddCharacter(CharacterModel character)
    {
        _characters.Add(character);
    }

    public IReadOnlyCollection<CharacterModel> Characters => _characters.AsReadOnly();
}