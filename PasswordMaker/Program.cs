using ClassLib.Model;
using Microsoft.AspNetCore.Identity;

var user = new UserAuthentication();
var hasher = new PasswordHasher<UserAuthentication>();

var hash = hasher.HashPassword(user, "Test12345!");

Console.WriteLine("New password hash: \n" + hash);
