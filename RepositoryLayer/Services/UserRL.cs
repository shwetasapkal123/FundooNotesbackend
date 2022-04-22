using Database_Layer;
using Experimental.System.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Services;
using RepositoryLayer.UserInterface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.UserClass
{
    public class UserRL : IUserRL
    {
        FundooContext fundoo;
        private readonly IConfiguration Toolsettings;
        private static string Key = "47c53aa7571c33d2f98d02a4313c4ba1ea15e45c18794eb564b21c19591805ff";
        //Constructor
        public UserRL (FundooContext fundoo, IConfiguration Toolsettings)
        {
            this.fundoo = fundoo;
            this.Toolsettings = Toolsettings;
        }
        public User AddUser(UserPostModel user)
        {
            // throw new NotImplementedException();
            try
            {
                User userobj = new User();
                userobj.Id = new User().Id;
                userobj.firstName = user.firstName;
                userobj.lastName = user.lastName;
                userobj.email = user.email;
                userobj.password = EncryptPassword(user.password);
                userobj.address = user.address;
                userobj.registeredDate = DateTime.Now;

                fundoo.Users.Add(userobj);
                int result= fundoo.SaveChanges();
                if(result>0)
                {
                    return userobj;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public string LoginUser(string email, string password)
        //{
        //    //throw new NotImplementedException();
        //    try
        //    {
        //        var result=fundoo.Users.FirstOrDefault(u=>u.email==email && u.password==password);
        //        if (result==null)
        //        {
        //            return null;
        //        }
        //        return GenJWTToken(email,result.Id);
        //        //string password=password;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
        //public static string GenJWTToken(string email, string Id)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenKey = Encoding.ASCII.GetBytes("THIS_IS_MY_KEY_TO_GENERATE_TOKEN");
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim("email", email),
        //            new Claim("userId",Id.ToString())
        //        }),
        //        Expires = DateTime.UtcNow.AddHours(1),
        //        SigningCredentials =
        //        new SigningCredentials(
        //            new SymmetricSecurityKey(tokenKey),
        //            SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}

        public string EncryptPassword(string Password)
        {
            try
            {
                byte[] encode = new byte[Password.Length];
                encode = Encoding.UTF8.GetBytes(Password);
                string encryptPass = Convert.ToBase64String(encode);
                return encryptPass;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string Login(UserLogin userLogin)
        {
            try
            {
                if (string.IsNullOrEmpty(userLogin.email) || string.IsNullOrEmpty(userLogin.password))
                {
                    return null;
                }

                var user = fundoo.Users.Where(x => x.email == userLogin.email && x.password == userLogin.password).FirstOrDefault();
                string dcryptPass = this.EncryptPassword(userLogin.password);

                if (user != null)
                {
                    string token = GenerateSecurityToken(user.email, user.Id);
                    return token;
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GenerateSecurityToken(string Email, int Id)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Toolsettings["Jwt:secretkey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,Email),
                new Claim("Id",Id.ToString())
            };
            var token = new JwtSecurityToken(Toolsettings["Jwt:Issuer"],
              Toolsettings["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddHours(1),
              signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ForgetPassword(string email)
        {
            try
            {
                var res=fundoo.Users.FirstOrDefault(x => x.email == email);
                if (res == null)
                    return false;

                //Add message queue
                MessageQueue queue;
                if (MessageQueue.Exists(@".\Private$\FundooQueue"))
                {
                    queue = new MessageQueue(@".\Private$\FundooQueue");
                }
                else
                {
                    queue = MessageQueue.Create(@".\Private$\FundooQueue");
                }
                Message message=new Message();
                message.Formatter=new BinaryMessageFormatter();
                message.Body = GenerateSecurityToken(email, res.Id);
                queue.Send(message);
                Message msg=queue.Receive();
                msg.Formatter=new BinaryMessageFormatter();
                EmailService.SendMail(email,msg.Body.ToString());
                queue.ReceiveCompleted += new ReceiveCompletedEventHandler(msmqQueue_ReceiveCompleted);
                queue.BeginReceive();
                queue.Close();
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        //GENERATE TOKEN WITH EMAIL
        public string GenerateToken(string email)
        {
            if (email == null)
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes("THIS_IS_MY_KEY_TO_GENERATE_TOKEN");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Email",email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void msmqQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                MessageQueue queue = (MessageQueue)sender;
                Message msg = queue.EndReceive(e.AsyncResult);
                EmailService.SendMail(e.Message.ToString(), GenerateToken(e.Message.ToString()));
                queue.BeginReceive();
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode ==
                    MessageQueueErrorCode.AccessDenied)
                {
                    Console.WriteLine("Access is denied. " +
                        "Queue might be a system queue.");
                }
                // Handle other sources of MessageQueueException.
            }
        }

        public bool ChangePassword(string email,string password,string confirmPassword)
        {
            try
            {
                if (password.Equals(confirmPassword))
                {
                    var user = fundoo.Users.Where(x => x.email == email).FirstOrDefault();
                    user.password = confirmPassword;
                    fundoo.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
