using Database_Layer;
using Microsoft.Extensions.Configuration;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLayer.Services
{
    public class NoteRL:INoteRL
    {
        // Created The User Repository Layer Class To Implement IUserRL Methods
        // Reference Object For FundooContext And IConfiguration
        FundooContext fundoo;
        private readonly IConfiguration Toolsettings;

        //Created Constructor To Initialize Fundoocontext For Each Instance
        public NoteRL(FundooContext fundoo, IConfiguration Toolsettings)
        {
            this.fundoo = fundoo;
            this.Toolsettings = Toolsettings;
        }
        public async Task AddNote(NotePostModel notePostModel,int userId)
        {
            try
            {
                var user = fundoo.Users.FirstOrDefault(u => u.Id == userId);
                Note note = new Note
                {
                    User = user
                };
                note.Title = notePostModel.Title;
                note.Description = notePostModel.Description;
                note.Title = notePostModel.Title;
                note.Title = notePostModel.Title;
                fundoo.Add(note);
                await fundoo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<User> GetAllUsers()
        {
            try
            {
                var result = fundoo.Users.ToList();
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task AddNote(NotePostModel notePostModel)
        {
            throw new NotImplementedException();
        }
    }
}
