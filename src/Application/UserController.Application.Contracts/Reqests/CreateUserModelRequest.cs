namespace UserController.Application.Contracts.Reqests;

public class CreateUserModelRequest
{
    public CreateUserModelRequest(string name, string phoneNumber)
    {
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public string Name { get; set; }

    public string PhoneNumber { get; set; }
}