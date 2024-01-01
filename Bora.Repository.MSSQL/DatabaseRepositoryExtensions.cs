using Azure.Data.Tables;
using Bora.Database;
using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.AzureTables;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DatabaseRepositoryExtensions
	{
		public static async void Migrate(this IServiceProvider serviceProvider)
		{
			using var scope = serviceProvider.CreateScope();
			var boraDbContext = scope.ServiceProvider.GetService<BoraDbContext>();
			if (boraDbContext!.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
			{
				boraDbContext.Database.Migrate();
			}
		}

		public static async void Seed<TEntity>(this IServiceProvider serviceProvider, IEnumerable<TEntity> entities) where TEntity : class, IEntity
		{
			using var scope = serviceProvider.CreateScope();
			var databaseRepository = scope.ServiceProvider.GetService<IDatabaseRepository>();
			databaseRepository!.AddRange(entities);

			await databaseRepository.CommitAsync();
		}
	}
}
