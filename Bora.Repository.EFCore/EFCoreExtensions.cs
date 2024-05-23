using Bora;
using Bora.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EFCoreExtensions
	{
        public static void AddEFCoreRepository(this IServiceCollection serviceCollection, EFCoreProvider efCoreProvider, string? boraDatabaseConnString = null)
        {
            int retries = 3;
            int waitInSeconds = 1;
            for (int i = 1; i <= retries; i++)
            {
                try
                {
                    Console.WriteLine("Adding BoraDbContext ...");
                    Console.WriteLine($"Using {efCoreProvider} Provider with {boraDatabaseConnString}");

                    TryConnect(efCoreProvider, boraDatabaseConnString);

                    serviceCollection.AddBoraDbContext(efCoreProvider, boraDatabaseConnString);

                    serviceCollection.AddScoped<IRepository, EFCoreRepository>();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Successful connecting to the provider!");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error connecting to the provider! ({ex.Message})");
                    Thread.Sleep(waitInSeconds * 1000);
                    if(retries == 3)
                        throw;

                }
                finally
                {
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }
        }

        private static void AddBoraDbContext(this IServiceCollection serviceCollection, EFCoreProvider efCoreProvider, string? boraDatabaseConnString = null)
        {
            switch (efCoreProvider)
            {
                case EFCoreProvider.SqlServer:
                    serviceCollection.AddDbContext<BoraDbContext>(options => options.UseSqlServer(boraDatabaseConnString));
                    break;
                case EFCoreProvider.InMemory:
                    serviceCollection.AddDbContext<BoraDbContext>(options => options.UseInMemoryDatabase("boraDatabase"));
                    break;
            }
        }

        public static void TryConnect(EFCoreProvider efCoreProvider, string? boraDatabaseConnString = null)
        {
            switch (efCoreProvider)
            {
                case EFCoreProvider.SqlServer:
                    new SqlConnection(boraDatabaseConnString).Open();
                    break;
            }
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
