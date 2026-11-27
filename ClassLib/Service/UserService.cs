using ClassLib.Interface;
using ClassLib.Model;
using ClassLib.Security;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace ClassLib.Service;

public class UserService
{
    private readonly IUserAuthRepo _userRepo;
    private readonly PasswordHasher<UserAuthentication> _passwordHasher = new();

    public UserService(IUserAuthRepo userRepo)
    {
        _userRepo = userRepo;
    }

    public bool AuthenticateUser(string username, string password)
    {
        var user = _userRepo.GetAuthByUserName(username);

        if (user is null)
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            Encoding.UTF8.GetString(user.Password),
            password);

        return result != PasswordVerificationResult.Failed;
    }

    public bool ChangePassword(string username, string newPassword, out string error)
    {
        if (!PasswordPolicy.IsValid(newPassword, out error))
        {
            return false;
        }

        var user = _userRepo.GetAuthByUserName(username);

        if (user is null)
        {
            error = "Brugeren blev ikke fundet.";
            return false;
        }

        user.Password = CreatePasswordHash(user, newPassword);

        if (_userRepo.UpdatePassword(user, user.UserId) is null)
        {
            error = "Adgangskoden kunne ikke ændres.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    private byte[] CreatePasswordHash(UserAuthentication user, string password)
    {
        var hash = _passwordHasher.HashPassword(user, password);
        return Encoding.UTF8.GetBytes(hash);
    }
}
