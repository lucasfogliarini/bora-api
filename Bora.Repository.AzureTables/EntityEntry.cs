namespace Bora.Repository.AzureTables
{
	public class EntityEntry(TableEntity tableEntity, EntityState entityState)
	{
		public TableEntity TableEntity { get; set; } = tableEntity;
		public EntityState EntityState { get; set; } = entityState;
	}

	public enum EntityState
	{
		Added,
		Deleted,
		Update,
		/// <summary>
		/// Entity does exist in the table, so invoking UpsertEntity will update using the given UpdateMode, which defaults to Merge if not given.
		///  Since UpdateMode.Replace was passed, the existing entity will be replaced and delete the "Brand" property.
		/// </summary>
		Upsert
	}
}
