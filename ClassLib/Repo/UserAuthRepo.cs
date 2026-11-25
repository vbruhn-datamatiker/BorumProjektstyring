using ClassLib.Interface;
using ClassLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Repo
{
    public class UserAuthRepo : IUserAuthRepo
    {

        private readonly BorumBygDBContext _context;
        public UserAuthRepo()
        {
            _context = new BorumBygDBContext();
        }

        public UserAuthentication Add(UserAuthentication entity)
        {
            _context.UserAuthentications.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public UserAuthentication Delete(int id)
        {
            var user = GetById(id);
            _context.UserAuthentications.Remove(user);
            _context.SaveChanges();
            return user;
        }

        public List<UserAuthentication> GetAll()
        {
            return _context.UserAuthentications.ToList();
        }

        public UserAuthentication GetById(int id)
        {
            return _context.UserAuthentications.Find(id);
        }
        

        public UserAuthentication Update(UserAuthentication entity, int id)
        {
            var user = GetById(id);
            if (user != null)
            {
                _context.Entry(user).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
            return entity;
        }

        public UserAuthentication? UpdatePassword(UserAuthentication entity, int id)
        {
            var user = GetById(id);
            if (user == null)
            {
                return null;
            }

            user.Password = entity.Password;
            _context.Entry(user).Property(u => u.Password).IsModified = true;
            _context.SaveChanges();
            return user;
        }

        public UserAuthentication? GetAuthByUserName(string username)
        {
            return _context.UserAuthentications.FirstOrDefault(u => u.Username == username);
        }

    }
}
