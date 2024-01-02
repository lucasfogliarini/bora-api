using Azure.Data.Tables;
using Repository.AzureTables;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AzureTablesExtensions
	{
		public static void AddAzureTablesRepository(this IServiceCollection serviceCollection, string storageConnectionString)
		{
			if (storageConnectionString == null)
			{
				throw new ArgumentNullException(storageConnectionString);
			}
			Console.WriteLine("Adding TableServiceClient ...");
			var tableServiceClient = new TableServiceClient(storageConnectionString);
			Console.WriteLine("Adding AzureTablesRepository ...");
			serviceCollection.AddSingleton(tableServiceClient);
			serviceCollection.AddScoped<IAzureTablesRepository, AzureTablesRepository>();
		}

		public static async void Seed<TEntity>(this IServiceProvider serviceProvider, IEnumerable<TEntity> entities) where TEntity : Entity
		{
			using var scope = serviceProvider.CreateScope();
			var azureTablesRepository = scope.ServiceProvider.GetService<IAzureTablesRepository>();
			azureTablesRepository!.UpdateRange(entities);

			await azureTablesRepository.CommitAsync();
		}
	}
}
