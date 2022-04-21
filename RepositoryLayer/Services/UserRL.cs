using Database_Layer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
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
                //string dcryptPass = this.EncryptPassword(userLogin.password);

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

        public string GenerateSecurityToken(string Email, int Id)
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

    }
}
