using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using Bora.Entities;
using Dapper;
using static Dapper.SqlMapper;

namespace Bora.Repository.Dapper
{
    public class DapperRepository(IDbConnection dbConnection) : IRepository
	{
		public IQueryable<TEntity> Query<TEntity>() where TEntity : Entity
		{
			return dbConnection.Query<TEntity>("SELECT * FROM " + typeof(TEntity).Name).AsQueryable();
		}
		public IQueryable<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>>? where) where TEntity : Entity
		{
			if (where == null)
				return Query<TEntity>();

			var query = Query<TEntity>();
			return query.Where(where);
		}
		public IQueryable<TEntity> All<TEntity>() where TEntity : Entity
		{
			return Query<TEntity>();
		}
		public bool Any<TEntity>(Expression<Func<TEntity, bool>>? where = null) where TEntity : Entity
		{
			var query = Query<TEntity>();
			return where != null ? query.Any(where) : query.Any();
		}
		public TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity
		{
			var query = Query<TEntity>();
			return query.FirstOrDefault(where);
		}
		public void Remove<TEntity>(TEntity entity) where TEntity : Entity
		{
			dbConnection.Execute($"DELETE FROM {typeof(TEntity).Name} WHERE Id = @Id", entity.Id);
		}		
		public void Update<TEntity>(TEntity entity) where TEntity : Entity
		{
			var setValuesCollection = entity.GetType().GetProperties()
			.Where(prop => prop.Name != "Id" && prop.GetValue(entity) != null)
			.Select(p=> ToSetValue(p, entity));
			string setValues = string.Join(",", setValuesCollection);
			dbConnection.Execute($"UPDATE {typeof(TEntity).Name} SET {setValues} WHERE Id = {entity.Id}");
		}
		private string ToSetValue(PropertyInfo property, Entity entity)
        {
            if (property.PropertyType.IsEnum)
            {
                int enumIntValue = (int)property.GetValue(entity);
                return $"{property.Name} = {enumIntValue}";
            }
            //else if (property.PropertyType == typeof(bool))
            //{
            //    bool boolValue = (bool)property.GetValue(entity);
            //    int boolIntValue = boolValue ? 1 : 0;
            //    return $"{property.Name} = {boolValue}";
            //}
            else
            {
                return $"{property.Name} = '{property.GetValue(entity)}'";
            }
        }
        //TODO
        public void Add<TEntity>(TEntity entity) where TEntity : Entity
		{
			throw new NotImplementedException();
			dbConnection.Execute($"INSERT INTO {typeof(TEntity).Name} VALUES (@Id, @Name, @OtherProperties)", entity);
		}
		public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
		{
			throw new NotImplementedException();
			foreach (var entity in entities)
			{
				dbConnection.Execute($"UPDATE {typeof(TEntity).Name} SET Name = @Name, OtherProperties = @OtherProperties WHERE Id = @Id", entity);
			}
		}		
		public async Task<int> CommitAsync()
		{
			throw new NotImplementedException();
		}
        public Task SeedAsync()
        {
            throw new NotImplementedException();
        }
    }
}
