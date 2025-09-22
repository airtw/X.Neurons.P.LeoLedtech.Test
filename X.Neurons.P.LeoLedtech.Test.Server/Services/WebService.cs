using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using X.Neurons.P.LeoLedtech.Test.Server.Engines.Authorize;
using X.Neurons.P.LeoLedtech.Test.Server.Hubs;
using X.Neurons.P.LeoLedtech.Test.Server.Models.Authorize;

namespace X.Neurons.P.LeoLedtech.Test.Server.Services
{
    /// <summary>
    /// Web伺服器
    /// </summary>
    public class WebService
    {
        /// <summary>
        /// 初始化WebService
        /// </summary>
        public static void Initialization()
        {
            Task.Factory.StartNew(() => StartWebServer());
        }
        private static void StartWebServer()
        {
            var builder = WebApplication.CreateBuilder();

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(12500); // HTTP
            });

            builder.Services.AddControllers();

            #region JWT
            // Configure JWT authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
            // 添加 SignalR 服務
            builder.Services.AddSignalR();
            // Register services
            builder.Services.AddScoped<JwtEngine>();
            builder.Services.AddScoped<UserEngine>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            #endregion

            #region CORS
            builder.Services.AddCors(options =>
            {
                // CorsPolicy 是自訂的 Policy 名稱
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            #endregion

            var app = builder.Build();

            app.MapHub<AuthHub>("/authHub");
            app.MapHub<DashboardHub>("/dashboardHub");

            app.UseStaticFiles(); //靜態資料夾

            // Configure the HTTP request pipeline.

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.MapControllers();

            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}
