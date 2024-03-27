using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Repository.EntityConfigurations
{
    internal sealed class ContentCofiguration : IEntityTypeConfiguration<Content>
    {
        public void Configure(EntityTypeBuilder<Content> builder)
        {
            builder.ConfigureEntity();
            builder.HasIndex(e => new { e.Collection, e.Key, e.AccountId }).IsUnique();

            builder.Property(e => e.Key).IsRequired();
            builder.Property(e => e.Collection).IsRequired();
            builder.Property(e => e.Text).IsRequired();
            builder.Property(e => e.UpdatedAt);
        }
    }
}
