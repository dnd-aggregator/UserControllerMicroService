namespace UserController.Application.Models;

public class UserModel
{
    public UserModel(string name, string phoneNumber)
    {
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public long Id { get; set; }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }
}