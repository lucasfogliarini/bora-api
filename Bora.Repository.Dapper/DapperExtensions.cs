using Bora;
using Bora.Repository.Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class DapperExtensions
	{
		public static void AddDapperRepository(this IServiceCollection serviceCollection, string boraDatabaseConnString)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Adding {nameof(DapperRepository)} using SqlConnection with '{boraDatabaseConnString}'");
			serviceCollection.AddScoped<IDbConnection, SqlConnection>(provider =>
			{
				return new SqlConnection(boraDatabaseConnString);
			});
			Console.ResetColor();
			Console.WriteLine();

			serviceCollection.AddScoped<IRepository, DapperRepository>();
		}
	}
}
