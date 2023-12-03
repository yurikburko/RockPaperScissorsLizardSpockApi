using Microsoft.Extensions.Configuration;

namespace RockPaperScissorsLizardSpockApi.Tests
{
    public class TestsConfiguration
    {
        private static IConfiguration _configuration;
        public static IConfiguration Configuration => _configuration ?? (_configuration = Read());

        public static T BindTo<T>()
            where T : class, new()
        {
            var result = new T();
            Configuration.Bind(result);
            return result;
        }

        private static IConfiguration Read()
        {
            return new ConfigurationBuilder()
                           .SetBasePath(Directory.GetCurrentDirectory())
                           .AddJsonFile("appsettings.json")
                           .AddEnvironmentVariables()
                           .Build();
        }

    }
}
