using Diary.Server.Data;
using System.Text;

namespace Diary.Server.Services
{
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

        public async Task<bool> CreateJournal(string username, string title, string body, string userKey)
        {
            User? user = await userService.GetUserAsync(username);
            if (user == null)
            {
                logger.LogError($"Unable to find user with name ${username}");
                return false;
            }

            string decryptedUserKey = cryptoService.DecryptText(Encoding.ASCII.GetBytes(userKey));
            byte[] userKeyBytes = Encoding.ASCII.GetBytes(decryptedUserKey);

            JournalEntry journalEntry = new()
            {
                CreatedOn = DateTime.Now,
                Creator = user,
                Title = title,
                EncryptedBody = cryptoService.EncryptText(body, userKeyBytes)
            };

            appDbContext.Add(journalEntry);
            await appDbContext.SaveChangesAsync();
            return true;
        }
    }
}
