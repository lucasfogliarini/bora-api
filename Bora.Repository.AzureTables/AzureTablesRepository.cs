using Azure.Data.Tables;
using Bora.Entities;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;

namespace Bora.Repository.AzureTables
{
	public class AzureTablesRepository(TableServiceClient tableServiceClient) : IRepository
	{
		const string PARTITION_KEY = "1";
		protected List<EntityEntry> EntityEntries { get; set; } = [];
		private readonly TableServiceClient _tableServiceClient = tableServiceClient;

		public IQueryable<TEntity> Query<TEntity>() where TEntity : Entity
		{
			throw new NotImplementedException();
		}

		public async Task<IQueryable<TEntity>> WhereAsync<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity
		{
			var whereTable = where.ConvertToTableEntity(); 
			var tableClient = GetTableClient<TEntity>();
			List<TEntity> entities = [];
			await foreach (var entity in tableClient.QueryAsync(whereTable))
            {
				entities.Add(entity);
			}
			return entities.AsQueryable();
		}
		public IQueryable<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>>? where = null) where TEntity : Entity
		{
			var tableClient = GetTableClient<TEntity>();
			var filteredEntities = tableClient.Query(where).ToList();
			return filteredEntities.AsQueryable();
		}
		public IQueryable<TEntity> All<TEntity>() where TEntity : Entity
		{
			var tableClient = GetTableClient<TEntity>();
			var filteredEntities = tableClient.Query<TEntity>(filter: string.Empty).ToList();
			return filteredEntities.AsQueryable();
		}
		public TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : Entity
		{
			return Where(where).FirstOrDefault();
		}
		public void Add<TEntity>(TEntity entity) where TEntity : Entity
		{
			entity.PartitionKey = PARTITION_KEY;
			var entityEntry = new EntityEntry(entity, EntityState.Added);
			EntityEntries.Add(entityEntry);
		}
		public void Update<TEntity>(TEntity entity) where TEntity : Entity
		{
			entity.PartitionKey = PARTITION_KEY;
			var entityState = entity.ETag == default ? EntityState.Upsert : EntityState.Update;
			var entityEntry = new EntityEntry(entity, entityState);
			EntityEntries.Add(entityEntry);
		}
		public void Remove<TEntity>(TEntity entity) where TEntity : Entity
		{
			var entityEntry = new EntityEntry(entity, EntityState.Deleted);
			EntityEntries.Add(entityEntry);
		}
		public async Task<int> CommitAsync()
		{
			var entityEntriesByTable = EntityEntries.GroupBy(e => e.TableEntity.GetType().Name);
			foreach (var entityEntries in entityEntriesByTable)
			{
				var tableClient = _tableServiceClient.GetTableClient(entityEntries.Key);
				await tableClient.CreateIfNotExistsAsync();
				var addeds = EntityEntries.Where(e => e.EntityState == EntityState.Added);
				if (addeds.Any())
				{
					var lastId = tableClient.Query<Entity>().OrderByDescending(e=>e.Id).FirstOrDefault()?.Id;
					foreach (EntityEntry entityEntry in addeds)
					{
						lastId = entityEntry.TableEntity.IncrementId(lastId);
						await tableClient.AddEntityAsync(entityEntry.TableEntity);
					}
				}
				foreach (EntityEntry entityEntry in EntityEntries.Where(e => e.EntityState == EntityState.Deleted))
				{
					await tableClient.DeleteEntityAsync(entityEntry.TableEntity.PartitionKey, entityEntry.TableEntity.RowKey);
				}
				foreach (EntityEntry entityEntry in EntityEntries.Where(e => e.EntityState == EntityState.Update))
				{
					await tableClient.UpdateEntityAsync(entityEntry.TableEntity, entityEntry.TableEntity.ETag);
				}
				foreach (EntityEntry entityEntry in EntityEntries.Where(e => e.EntityState == EntityState.Upsert))
				{
					await tableClient.UpsertEntityAsync(entityEntry.TableEntity, TableUpdateMode.Merge);
				}
			}

			var count = EntityEntries.Count;
			EntityEntries.Clear();
			return count;
		}
		public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : Entity
		{
			foreach (var entity in entities)
			{
				Update(entity);
			}
		}
		private TableClient GetTableClient<TEntity>() where TEntity : class
		{
			var tableName = typeof(TEntity).Name;
			var tableClient = _tableServiceClient.GetTableClient(tableName);
			return tableClient;
		}

		
	}
}
