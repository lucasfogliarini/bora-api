using System.Linq.Expressions;
using Bora.Entities;

namespace Bora.Repository.AzureTables
{
	public interface IAzureTablesRepository
	{
		IQueryable<TEntity> Query<TEntity>() where TEntity : AzTableEntity;
		IQueryable<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>>? where) where TEntity : AzTableEntity;
		IQueryable<TEntity> All<TEntity>() where TEntity : AzTableEntity;
		bool Any<TEntity>(Expression<Func<TEntity, bool>>? where = null) where TEntity : AzTableEntity;
		TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : AzTableEntity;
		void Add<TEntity>(TEntity entity) where TEntity : AzTableEntity;
		void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : AzTableEntity;
		void Update<TEntity>(TEntity entity) where TEntity : AzTableEntity;
		void Remove<TEntity>(TEntity entity) where TEntity : AzTableEntity;
		Task<int> CommitAsync();
	}
}
