using ClassLib.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLib.Interface
{
    public interface IUserRepo : IRepo<User>
    {
        UserAuthentication GetAuthByUserName(string username);
    }
}
