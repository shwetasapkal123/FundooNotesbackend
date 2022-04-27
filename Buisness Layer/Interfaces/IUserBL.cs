using Database_Layer;
using RepositoryLayer.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buisness_Layer.Interfaces
{
    //Interface
    public interface IUserBL
    {
        //interface method(doesnot have body)
        public User AddUser(UserPostModel user);
        public string Login(UserLogin userLogin);
        public bool ForgetPassword(string email);
        // public bool ChangePassword(string email, string password, string confirmPassword);
       public bool ResetPassword(string password, string confirmpassword,string Email);
    }
}
