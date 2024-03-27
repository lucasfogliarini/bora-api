using System.Linq.Expressions;
using Bora.Entities;

namespace Bora
{
	public interface IRepository
	{
		IQueryable<TEntity> Query<TEntity>() where TEntity : Entity;
		IQueryable<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>>? where) where TEntity : Entity;
		IQueryable<TEntity> All<TEntity>() where TEntity : Entity;
		bool Any<TEntity>(Expression<Func<TEntity, bool>>? where = null) where TEntity : Entity;
		TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity;
		void Add<TEntity>(TEntity entity) where TEntity : Entity;
		void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity;
		void Update<TEntity>(TEntity entity) where TEntity : Entity;
		void Remove<TEntity>(TEntity entity) where TEntity : Entity;
		Task<int> CommitAsync();
	}
}
