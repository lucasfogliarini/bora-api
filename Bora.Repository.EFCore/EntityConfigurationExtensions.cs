using Bora.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
	public static class EntityConfigurationExtensions
    {
        public static PropertyBuilder<TProperty> PricePrecision<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasPrecision(22, 4);
        }

        public static PropertyBuilder<TProperty> FeePrecision<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasPrecision(5, 4);
        }

        public static void ConfigureEntity<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : Entity
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt);
		}
    }
}
