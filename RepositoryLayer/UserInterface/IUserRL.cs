using Database_Layer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.UserInterface
{
    public interface IUserRL
    {
        public void AddUser(UserPostModel user);
    }
}
