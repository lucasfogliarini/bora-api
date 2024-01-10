using Bora.Database;
using Bora.Repository;
using Microsoft.EntityFrameworkCore;

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

		public static async void Seed<TEntity>(this IServiceProvider serviceProvider, IEnumerable<TEntity> entities) where TEntity : Entity
		{
			using var scope = serviceProvider.CreateScope();
			var databaseRepository = scope.ServiceProvider.GetService<IRepository>();
			databaseRepository!.UpdateRange(entities);

			await databaseRepository.CommitAsync();
		}
	}
}
