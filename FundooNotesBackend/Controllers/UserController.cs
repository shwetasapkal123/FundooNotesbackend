using Buisness_Layer.Interfaces;
using Database_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FundooNotesBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class UserController : ControllerBase
    {
        //instance variable
        IUserBL userBL;
        FundooContext fundooContext;

        //Constructor
        public UserController(IUserBL userBL, FundooContext fundooContext)
        {
            this.userBL = userBL;
            this.fundooContext = fundooContext;
        }

        //Post method for creating new data
        [HttpPost("register")]
        public ActionResult RegisterdataAdd(UserPostModel user)
        {
            try
            {
                var getUserdata = fundooContext.Users.FirstOrDefault(u => u.email == user.email);
                if (getUserdata != null)
                {
                    return this.Ok(new { success = false, message = $"{user.email} is Already Exists" });
                }
                this.userBL.AddUser(user);
                return this.Ok(new { success = true, message = $"Registration Successful {user.email}" });
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("login/{email}/{password}")]
        public IActionResult Login(UserLogin userLogin)
        {
            try
            {
                var result = userBL.Login(userLogin);
                if (result != null)
                    return this.Ok(new { success = true, message = "Login Successful", Token = result });
                else
                    return this.BadRequest(new { success = false, message = "Login UnSuccessful", Token = result });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPost("ForgotPassword")]
        public IActionResult ForgotPassword(string email)
        {
            try
            {
                var result = userBL.ForgetPassword(email);
                if (result == true)
                    return this.Ok(new { success = true, message = "Reset link send Successfully on registered Email" + email });
                else
                    return this.BadRequest(new { success = false, message = "Reset link UnSuccessful for mail"+ email });
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        [Authorize]
        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword(string password,string confirmpassword)
        {
            try
            {
                //var email = User.FindFirst(ClaimTypes.Email).Value.ToString();
                //var userid = User.Claims.FirstOrDefault(x => x.Type.ToString().Equals("userId", StringComparison.InvariantCultureIgnoreCase));
                //int userId = Int32.Parse(userid.Value);
                //bool res = userBL.ChangePassword(email, password, confirmpassword);

                //if (!res)
                //{
                //    return this.BadRequest(new { success = false, message = "enter valid password" });

                //}
                //else
                //{
                //    return this.Ok(new { success = true, message = "reset password set successfully" });
                //}
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var UserEmailObject = claims.Where(p => p.Type == @"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").FirstOrDefault()?.Value;
                    this.userBL.ChangePassword(UserEmailObject, password, confirmpassword);
                    return Ok(new
                    {
                        success = true,
                        message = "Password Changed Sucessfully", email = $"{ UserEmailObject}" });
                }
                return Ok(new { success = false, message = "Password Changed Unsuccessful" });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
