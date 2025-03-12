namespace BankSystem.Services.User
{
    using System.Threading.Tasks;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class UserService : BaseService, IUserService
    {
        public UserService(BankSystemDbContext context)
            : base(context)
        {
        }

        public async Task<string> GetUserIdByUsernameAsync(string username)
        {
            var user = await this.Context
                .Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.UserName == username);

            _logger.LogInformation("User ID retrieved - " + user?.Id);

            return user?.Id;
        }

        public async Task<string> GetAccountOwnerFullnameAsync(string userId)
        {
            var user = await this.Context
                .Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.Id == userId);

            _logger.LogInformation("User name retrieved - " + user?.FullName)

            return user?.FullName;
        }
    }
}