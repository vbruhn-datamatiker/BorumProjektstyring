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

        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
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
    }
}
