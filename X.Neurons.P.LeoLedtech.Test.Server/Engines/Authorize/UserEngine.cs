using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Authorize;

namespace X.Neurons.P.LeoLedtech.Test.Server.Engines.Authorize
{
    public class UserEngine
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly List<User> _users;

        public UserEngine(IPasswordHasher<User> passwordHasher)
        {
            _passwordHasher = passwordHasher;
            //_users = GlobalStore.User;
            // 初始化靜態使用者資料
            _users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Username = "admin",
                    // 預設密碼為 "admin"，實際使用時需要先經過雜湊
                    PasswordHash = _passwordHasher.HashPassword(null, "admin"),
                    Role = "Admin"
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    // 預設密碼為 "user"
                    PasswordHash = _passwordHasher.HashPassword(null, "user"),
                    Role = "User"
                }
            };
        }

        public async Task<User> ValidateUserAsync(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);

            if (user == null) return null;

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success ? user : null;
        }
    }
}
