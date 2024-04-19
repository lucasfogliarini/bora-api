using Bora;
using Bora.Repository;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class EFCoreExtensions
	{
        public static void AddEFCoreRepository(this IServiceCollection serviceCollection, EFCoreProvider efCoreProvider, string? boraDatabaseConnString = null)
        {
            Console.WriteLine("Adding DbConext ...");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Using {efCoreProvider} Provider with {boraDatabaseConnString}");
            switch (efCoreProvider)
            {
                case EFCoreProvider.SqlServer:
                    serviceCollection.AddDbContext<BoraDbContext>(options => options.UseSqlServer(boraDatabaseConnString));
                    break;
                case EFCoreProvider.InMemory:
                    serviceCollection.AddDbContext<BoraDbContext>(options => options.UseInMemoryDatabase("boraDatabase"));
                    break;
            }

            Console.ResetColor();
            Console.WriteLine();

            serviceCollection.AddScoped<IRepository, EFCoreRepository>();
        }

        public static void Migrate(this IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var boraDbContext = scope.ServiceProvider.GetService<BoraDbContext>();
			if (boraDbContext != null && boraDbContext!.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
			{
				boraDbContext.Database.Migrate();
			}
		}
	}

    public enum EFCoreProvider
    {
        SqlServer,
        InMemory,
    }
}
