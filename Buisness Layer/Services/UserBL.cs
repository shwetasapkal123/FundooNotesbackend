using Buisness_Layer.Interfaces;
using Database_Layer;
using RepositoryLayer.Entity;
using RepositoryLayer.UserInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buisness_Layer.Services
{
    //userBL implements the IuserBL interface
    public class UserBL:IUserBL
    {
        private readonly IUserRL userRL;
        //constructor
        public UserBL(IUserRL userRL)
        {
            this.userRL = userRL;
        }
        public User AddUser(UserPostModel user)
        {
            //body of AddUser is provied here
            try
            {
               return userRL.AddUser(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string Login(UserLogin userLogin)
        {
            try
            {
                return userRL.Login(userLogin);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ForgetPassword(string email)
        {
            try
            {
                return userRL.ForgetPassword(email);
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public bool ChangePassword(string email, string password, string confirmPassword)
        {
            try
            {
                return userRL.ChangePassword(email, password, confirmPassword);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
