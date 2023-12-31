using Azure;
using Azure.Data.Tables;
using Repository.AzureTables;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Repository.AzureTables
{
	public class AzureTablesRepository(TableServiceClient tableServiceClient) : IAzureTablesRepository
	{
		const string PARTITION_KEY = "1";
		protected List<EntityEntry> EntityEntries { get; set; } = [];
		private readonly TableServiceClient _tableServiceClient = tableServiceClient;

		public IQueryable<TEntity> Where<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class, ITableEntity
		{
			var tableClient = GetTableClient<TEntity>();
			string filter = new ExpressionTranslator().Translate(where.Body);
			Pageable<TEntity> entityPageable = tableClient.Query<TEntity>(filter: filter);
			return entityPageable.ToList().AsQueryable();
		}
		public TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : class, ITableEntity
		{
			return Where(where).FirstOrDefault();
		}
		public void Add<TEntity>(TEntity entity) where TEntity : class, ITableEntity
		{
			entity.PartitionKey = PARTITION_KEY;
			entity.RowKey = Guid.NewGuid().ToString();
			var entityEntry = new EntityEntry(entity, EntityState.Added);
			EntityEntries.Add(entityEntry);
		}
		public void Update<TEntity>(TEntity entity) where TEntity : class, ITableEntity
		{
			var entityState = entity.ETag == default ? EntityState.Upsert : EntityState.Update;
			var entityEntry = new EntityEntry(entity, entityState);
			EntityEntries.Add(entityEntry);
		}
		public void Remove<TEntity>(TEntity entity) where TEntity : class, ITableEntity
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
				foreach (EntityEntry entityEntry in EntityEntries.Where(e=>e.EntityState == EntityState.Added))
				{
					await tableClient.AddEntityAsync(entityEntry.TableEntity);
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
					await tableClient.UpsertEntityAsync(entityEntry.TableEntity, TableUpdateMode.Replace);
				}
			}

			var count = EntityEntries.Count;
			EntityEntries.Clear();
			return count;
		}
		public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, ITableEntity
		{
			foreach (var entity in entities)
			{
				Add(entity);
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
