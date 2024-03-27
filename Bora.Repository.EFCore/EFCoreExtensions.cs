using Bora;
using Bora.Entities;
using Bora.Repository;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class EFCoreExtensions
	{
		public static void AddEFCoreRepository(this IServiceCollection serviceCollection, string boraDatabaseConnString)
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

			serviceCollection.AddSingleton<IRepository, EFCoreRepository>();
		}

		public static void Migrate(this IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var boraDbContext = scope.ServiceProvider.GetService<BoraDbContext>();
			if (boraDbContext!.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
			{
				boraDbContext.Database.Migrate();
			}
		}

		public static async Task SeedAsync<TEntity>(this IServiceProvider serviceProvider, IEnumerable<TEntity> entities) where TEntity : Entity
		{
			using var scope = serviceProvider.CreateScope();
			var databaseRepository = scope.ServiceProvider.GetService<IRepository>();
			databaseRepository!.UpdateRange(entities);

			await databaseRepository.CommitAsync();
		}
	}
}
