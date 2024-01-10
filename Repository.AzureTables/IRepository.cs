using System.Linq.Expressions;

namespace Repository.AzureTables
{
	public interface IRepository
	{
		Task<IQueryable<TEntity>> WhereAsync<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity;
		IQueryable<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>>? where) where TEntity : Entity;
		IQueryable<TEntity> All<TEntity>() where TEntity : Entity;
		TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity;
		void Add<TEntity>(TEntity entity) where TEntity : Entity;
		void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity;
		void Update<TEntity>(TEntity entity) where TEntity : Entity;
		void Remove<TEntity>(TEntity entity) where TEntity : Entity;
		Task<int> CommitAsync();
	}
}
