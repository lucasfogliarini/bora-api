using Bora;

namespace BoraApi
{
    public static class RepositoryExtensions
    {
        public static void AddRepository(this WebApplicationBuilder builder)
        {
            try
            {
                var efCoreProvider = builder.Environment.IsProduction() ? EFCoreProvider.SqlServer : EFCoreProvider.InMemory;
                var repositoryConnectionString = TryGetConnectionString(builder);
                builder.Services.AddEFCoreRepository(efCoreProvider, repositoryConnectionString);
                //builder.Services.AddDapperRepository(repositoryConnectionString);
            }
            catch (Exception)
            {
                builder.Services.AddEFCoreRepository(EFCoreProvider.InMemory);
            }
        }
        public static async Task SeedAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var databaseRepository = scope.ServiceProvider.GetService<IRepository>();
            await databaseRepository!.SeedAsync();
        }
        private static string? TryGetConnectionString(WebApplicationBuilder builder)
        {
            var boraRepositoryConnectionStringKey = "ConnectionStrings:BoraRepository";
            Console.WriteLine($"Trying to get a database connectionString '{boraRepositoryConnectionStringKey}' from Configuration.");
            var connectionString = builder.Configuration[boraRepositoryConnectionStringKey];
            if (connectionString == null)
                throw new Exception($"{boraRepositoryConnectionStringKey} was not found! From builder.Configuration[{boraRepositoryConnectionStringKey}]");

            Console.WriteLine();
            return connectionString;
        }

    }
}
