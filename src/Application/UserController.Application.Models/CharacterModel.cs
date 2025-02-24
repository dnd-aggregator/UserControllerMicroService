namespace UserController.Application.Models;

public class CharacterModel
{
    public CharacterModel(
        string characterName,
        string characterDescription,
        int characterLevel,
        string race,
        string worldView,
        int speed,
        int defence,
        int health,
        int maxHealth,
        int strenth,
        int dexterity,
        int endurance,
        int intelligence,
        int wisdom,
        int bonus,
        IReadOnlyCollection<string> gear,
        IReadOnlyCollection<string> weapons,
        string personalityTraits,
        string ideals,
        string bonds,
        string flaws,
        string history,
        IReadOnlyCollection<string> activeSkills,
        IReadOnlyCollection<string> passiveSkills,
        long userId)
    {
        CharacterName = characterName;
        CharacterDescription = characterDescription;
        CharacterLevel = characterLevel;
        Race = race;
        WorldView = worldView;
        Speed = speed;
        Defence = defence;
        Health = health;
        MaxHealth = maxHealth;
        Strenth = strenth;
        Dexterity = dexterity;
        Endurance = endurance;
        Intelligence = intelligence;
        Wisdom = wisdom;
        Bonus = bonus;
        Gear = gear;
        Weapons = weapons;
        PersonalityTraits = personalityTraits;
        Ideals = ideals;
        Bonds = bonds;
        Flaws = flaws;
        History = history;
        ActiveSkills = activeSkills;
        PassiveSkills = passiveSkills;
        UserId = userId;
        Status = CharacterStatus.Draft;
    }

    public long CharacterId { get; set; }

    public long UserId { get; set; }

    public string CharacterName { get; set; }

    public string CharacterDescription { get; set; }

    public int CharacterLevel { get; set; }

    public string Race { get; set; }

    public string WorldView { get; set; }

    public int Speed { get; set; }

    public int Defence { get; set; }

    public int Health { get; set; }

    public int MaxHealth { get; set; }

    public int Strenth { get; set; }

    public int Dexterity { get; set; }

    public int Endurance { get; set; }

    public int Intelligence { get; set; }

    public int Wisdom { get; set; }

    public int Bonus { get; set; }

    public IReadOnlyCollection<string> Gear { get; set; }

    public IReadOnlyCollection<string> Weapons { get; set; }

    public string PersonalityTraits { get; set; }

    public string Ideals { get; set; }

    public string Bonds { get; set; }

    public string Flaws { get; set; }

    public string History { get; set; }

    public IReadOnlyCollection<string> ActiveSkills { get; set; }

    public IReadOnlyCollection<string> PassiveSkills { get; set; }

    public CharacterStatus Status { get; set; }
}