using MarketplaceConsoleApp.Models;

namespace MarketplaceConsoleApp.Services;

public class UserService
{
    private List<User> _users = new List<User>();

    public User RegisterUser(string username, string password)
    {
        var user = new User(username, password);
        _users.Add(user);
        return user;
    }

    public User? LoginUser(string username, string password)
    {
        return _users.FirstOrDefault(u =>
            u.Username == username &&
            u.Password == password);
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }
}