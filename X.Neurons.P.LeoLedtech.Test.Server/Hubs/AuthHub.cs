using Microsoft.AspNetCore.SignalR;
using X.Neurons.P.LeoLedtech.Test.Server.Engines.Authorize;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Authorize;
using System.Security.Claims;

namespace X.Neurons.P.LeoLedtech.Test.Server.Hubs
{
    public class AuthHub : Hub
    {
        private readonly UserEngine _userService;
        private readonly JwtEngine _jwtService;

        public AuthHub(UserEngine userService, JwtEngine jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        public async Task<AuthResponse> Login(LoginRequest request)
        {
            try
            {
                var user = await _userService.ValidateUserAsync(request.Username, request.Password);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                var token = _jwtService.GenerateToken(user);

                // 將連線 ID 與使用者關聯
                await Groups.AddToGroupAsync(Context.ConnectionId, user.Id.ToString());

                return new AuthResponse
                {
                    Success = true,
                    Token = token,
                    ConnectionID = Context.ConnectionId,
                    User = new UserDto
                    {
                        Id = user.Id,
                        Username = user.Username,
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "An error occurred during login"
                };
            }
        }

        public async Task Logout()
        {
            // 移除使用者的群組關聯
            if (Context.User?.Identity?.IsAuthenticated ?? false)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId,
                    Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            }
        }

        public object ValidateToken(string token)
        {
            try
            {
                var isValid = _jwtService.ValidateToken(token);
                return new { isValid = isValid };
                //await Clients.Caller.SendAsync("TokenValidationResult", isValid);
            }
            catch
            {
                //await Clients.Caller.SendAsync("TokenValidationResult", false);
                return new { isValid = false };
            }
        }

        // 強制登出特定使用者
        public async Task ForceLogout(string userId)
        {
            await Clients.Group(userId).SendAsync("ForceLogout");
        }

        public async Task Heartbeat()
        {
            //Console.WriteLine("我應");
            await Clients.Caller.SendAsync("HeartbeatResponse");
        }
    }

    // DTOs
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public string ConnectionID { get; set; }
        public UserDto User { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
