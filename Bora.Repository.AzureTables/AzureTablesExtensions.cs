using Azure.Data.Tables;
using Bora;
using Bora.Entities;
using Bora.Repository.AzureTables;
using System.Linq.Expressions;

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
			serviceCollection.AddSingleton<IRepository, AzureTablesRepository>();
		}

		public static async void Seed<TEntity>(this IServiceProvider serviceProvider, IEnumerable<TEntity> entities) where TEntity : Entity
		{
			using var scope = serviceProvider.CreateScope();
			var azureTablesRepository = scope.ServiceProvider.GetService<IRepository>();
			azureTablesRepository!.UpdateRange(entities);

			await azureTablesRepository.CommitAsync();
		}
		public static Expression<Func<ITableEntity, bool>> ConvertToTableEntity<TEntity>(
			this Expression<Func<TEntity, bool>> where)
			where TEntity : Entity
		{
			var parameter = Expression.Parameter(typeof(ITableEntity), "x");

			var body = new ParameterReplacer(where.Parameters[0], parameter).Visit(where.Body);
			var lambda = Expression.Lambda<Func<ITableEntity, bool>>(body, parameter);

			return lambda;
		}

		private class ParameterReplacer : ExpressionVisitor
		{
			private readonly ParameterExpression _oldParameter;
			private readonly Expression _newExpression;

			public ParameterReplacer(ParameterExpression oldParameter, Expression newExpression)
			{
				_oldParameter = oldParameter;
				_newExpression = newExpression;
			}

			protected override Expression VisitParameter(ParameterExpression node)
			{
				return node == _oldParameter ? _newExpression : base.VisitParameter(node);
			}
		}
	}
}
