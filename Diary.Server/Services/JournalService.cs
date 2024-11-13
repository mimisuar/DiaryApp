using Diary.Server.Data;
using System.Text;

namespace Diary.Server.Services
{
    public struct JournalEntryView
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public class JournalService
    {
        private readonly UserService userService;
        private readonly CryptoService cryptoService;
        private readonly ApplicationDbContext appDbContext;
        private readonly ILogger<JournalService> logger;

        public JournalService(UserService userService, CryptoService cryptoService, ApplicationDbContext appDbContext, ILogger<JournalService> logger)
        {
            this.userService = userService;
            this.cryptoService = cryptoService;
            this.appDbContext = appDbContext;
            this.logger = logger;
        }

        public async Task<List<JournalEntry>?> GetUserJournals(string username)
        {
            User? user = await userService.GetUserAsync(username);
            if (user == null)
            {
                logger.LogError($"Unable to find user with name ${username}");
                return null;
            }

            return appDbContext.Journal.Where(journal => journal.CreatorId == user.Id).ToList();
        }

        public async Task<bool> CreateJournal(string username, string title, string body, string base64UserKey)
        {
            User? user = await userService.GetUserAsync(username);
            if (user == null)
            {
                logger.LogError($"Unable to find user with name ${username}");
                return false;
            }

            byte[] decryptedUserKey = cryptoService.DecryptUserKeyFromClient(base64UserKey);

            JournalEntry journalEntry = new()
            {
                CreatedOn = DateTime.Now,
                Creator = user,
                Title = title,
                EncryptedBody = cryptoService.EncryptJournalBody(body, decryptedUserKey)
            };

            appDbContext.Add(journalEntry);
            await appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<JournalEntryView?> GetJournal(string username, int journalId, string base64UserKey)
        {
            User? user = await userService.GetUserAsync(username);
            if (user == null)
            {
                logger.LogError($"Unable to find user with name ${username}");
                return null;
            }

            JournalEntry entry;
            try
            {
                entry = appDbContext.Journal.Single(j => j.CreatorId == user.Id && j.Id == journalId);
            }
            catch
            {
                logger.LogError("Failed to find valid journal.");
                return null;
            }
            byte[] decryptedUserKey = cryptoService.DecryptUserKeyFromClient(base64UserKey);

            return new()
            {
                Body = cryptoService.DecryptJournalBody(entry.EncryptedBody, decryptedUserKey),
                Title = entry.Title,
                CreatedOn = entry.CreatedOn
            };
        }
    }
}
