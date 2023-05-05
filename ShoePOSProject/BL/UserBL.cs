using ShoePOSProject.DL;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoePOSProject.BL
{
    public class UserBL
    {
        public List<User> GetActiveUsersList(DatabaseEntities de)
        {
            return new UserDL().GetActiveUsersList(de);
        }

        public List<User> GetUsersList(DatabaseEntities de)
        {
            return new UserDL().GetUsersList(de);
        }
        public User GetActiveUserById(int _Id, DatabaseEntities de)
        {
            return new UserDL().GeteActiveUserById(_Id, de);
        }

        public bool AddUser(User _user, DatabaseEntities de)
        {
            if (_user.Name == null || _user.Email == null || _user.Password == null || _user.Role == null)
            {
                return false;
            }
            else
            {
                return new UserDL().AddUser(_user, de);
            }
        }

        public bool UpdateUser(User _user, DatabaseEntities de)
        {
            if (_user.Name == "" || _user.Email == null || _user.Password == "" || _user.Role == null)
            {
                return false;
            }
            else
            {
                return new UserDL().UpdateUser(_user, de);
            }
        }

        public bool DeleteUser(int _id, DatabaseEntities de)
        {
            return new UserDL().DeleteUser(_id, de);
        }
    }
}