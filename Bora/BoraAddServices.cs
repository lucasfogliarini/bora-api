using Bora.Accounts;
using Bora.Contents;
using Bora.Database;
using Bora.Events;
using Bora.Scenarios;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BoraAddServices
    {
        public static void AddServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEventService, EventService>();
            serviceCollection.AddTransient<IAccountService, AccountService>();
            serviceCollection.AddTransient<IScenarioService, ScenarioService>();
            serviceCollection.AddTransient<IContentService, ContentService>();
            serviceCollection.AddTransient<IAccountDataStore, AccountDataStore>();
        }

        public static void AddDatabase(this IServiceCollection serviceCollection, string boraDatabaseConnString)
        {
			Console.WriteLine("Adding DbConext ...");
            Console.ForegroundColor = ConsoleColor.Green;
			if (boraDatabaseConnString == null)
            {
				Console.WriteLine("Using InMemoryDatabase Provider");
				serviceCollection.AddDbContext<BoraDbContext>(options => options.UseInMemoryDatabase("boraDatabase"));
			}	
            else
            {
				Console.WriteLine($"Using SqlServer Provider with {boraDatabaseConnString}");
				serviceCollection.AddDbContext<BoraDbContext>(options => options.UseSqlServer(boraDatabaseConnString));
				Console.WriteLine($"For use InMemory Database, remove the connectionString from the appsettings.");
			}
			Console.ResetColor();
			Console.WriteLine();

			serviceCollection.AddScoped<IBoraDatabase, BoraDatabase>();
        }
    }
}
