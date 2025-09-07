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

            // Add services to the container.

            builder.Services.AddControllers();

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

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.MapControllers();

            app.Run();
        }
    }
}
