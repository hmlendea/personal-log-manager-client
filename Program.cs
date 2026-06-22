using Microsoft.AspNetCore.Builder;

namespace PersonalLogManagerClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            Startup.ConfigureServices(builder.Services, builder.Configuration);

            WebApplication app = builder.Build();

            Startup.Configure(app);

            app.Run();
        }
    }
}
