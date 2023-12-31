using Azure.Data.Tables;
using Repository.AzureTables;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class AddAzureTablesRepositoryExtension
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
    }
}
