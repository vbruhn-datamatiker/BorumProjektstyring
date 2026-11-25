using ClassLib.Interface;
using ClassLib.Model;
using ClassLib.Repo;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ClassLib.Service
{
    public class UserService
    {

        private readonly IUserAuthRepo _userRepo;

        public UserService(IUserAuthRepo userRepo)
        {
            _userRepo = userRepo;
        }


        public bool AuthenticateUser(string username, string password)
        {
            try
            {
                var user = _userRepo.GetAuthByUserName(username);
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] passwordCode = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    return user?.Password.SequenceEqual(passwordCode) ?? false; // Authentication successful                    
                }

            }
            catch (KeyNotFoundException ex)
            {
                // Handle exceptions, log errors, etc.
               
                return false; // Implement your authentication logic here
            }
        }


        // Method to change the password hash for a user - takes in a string username and a string newPassword, returns a UserAuthentication object or null if the user is not found
        // This method retrieves the user by username, hashes the new password using SHA256, updates the user's password
        public UserAuthentication? ChangePasswordHash(string username, string newPassword)
        {
            var user = _userRepo.GetAuthByUserName(username);
            if (user != null)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] newPasswordHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(newPassword));
                    user.Password = newPasswordHash;
                    return _userRepo.UpdatePassword(user, user.UserId);
                }
            }

            return null;
        }

    }
}
