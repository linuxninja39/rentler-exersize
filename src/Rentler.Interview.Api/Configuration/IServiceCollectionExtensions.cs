namespace Rentler.Interview.Api.Configuration
{
    public static class IServiceCollectionExtensions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration config)
        {
            var appSettingsSection = config.GetSection("AppSettings");
            var connectionStringsSection = config.GetSection("ConnectionStrings");

            services.Configure<AppSettings>(appSettingsSection);
            services.Configure<ConnectionStrings>(connectionStringsSection);


        }
    }
}
