using ClassLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Interface
{
    public interface IUserAuthRepo : IRepo<UserAuthentication>
    {
        UserAuthentication GetAuthByUserName(string username);

        UserAuthentication? UpdatePassword(UserAuthentication entity, int id);
    }
}
