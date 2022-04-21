using Buisness_Layer.Interfaces;
using Database_Layer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using System;
using System.Linq;

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

        [HttpPost("login")]
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
    }
}
