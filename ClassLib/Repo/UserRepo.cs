using ClassLib.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using ClassLib.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;

namespace ClassLib.Repo
{
    public class UserRepo : IUserRepo
    {
        private BorumBygDBContext _context;

        public UserRepo()
        {
            _context = new BorumBygDBContext();
        }

        public User Add(User entity)
        {
            _context.Users.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public User Delete(int id)
        {
            var user = GetById(id);
            _context.Users.Remove(user);
            _context.SaveChanges();
            return user;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }

        public User? GetById(int id)
        {
            return _context.Users.Find(id);
        }

        public UserAuthentication? GetAuthByUserName(string username)
        {
            return _context.UserAuthentications.FirstOrDefault(u => u.Username == username);
        }

        // TODO: See if Update method for UserRepo actually works, as it is not tested yet. We have used entry.CurrentValues.SetValues(entity) to update the entity, but we need to check if it works as expected. Does .Update work instead?

        public User Update(User entity, int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _context.Entry(user).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
            return entity;
        }
    }
}
