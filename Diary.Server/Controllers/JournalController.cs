using Diary.Server.Data;
using Diary.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Diary.Server.Controllers
{
    public struct JournalFormData
    {
        public string Username { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string EncryptedKey { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class JournalController : ControllerBase
    {
        private readonly JournalService journalService;

        public JournalController(JournalService journalService)
        {
            this.journalService = journalService;
        }

        [HttpPost]
        [Authorize]
        public async Task<List<JournalEntry>> GetUserJournals([FromQuery] string username)
        {
            List<JournalEntry>? journals = await journalService.GetUserJournals(username);
            if (journals == null)
            {
                Response.StatusCode = 401;
                return [];
            }

            return journals;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task CreateUserJournal([FromBody] JournalFormData journalData)
        {
            bool result = await journalService.CreateJournal(journalData.Username, journalData.Title, journalData.Body, journalData.EncryptedKey);
            if (!result)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync("Unable to create journal.");
                return;
            }
        }
    }
}
