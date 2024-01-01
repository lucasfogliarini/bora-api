using Azure.Data.Tables;
using System.Linq.Expressions;

namespace Repository.AzureTables
{
	public interface IAzureTablesRepository
	{
		IQueryable<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>>? where = null) where TEntity : class, ITableEntity;
		TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class, ITableEntity;
		void Add<TEntity>(TEntity entity) where TEntity : class, ITableEntity;
		void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, ITableEntity;
		void Update<TEntity>(TEntity entity) where TEntity : class, ITableEntity;
		void Remove<TEntity>(TEntity entity) where TEntity : class, ITableEntity;
		Task<int> CommitAsync();
	}

	
}
