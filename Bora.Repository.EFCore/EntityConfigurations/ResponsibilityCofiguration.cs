using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Repository.EntityConfigurations
{
    internal sealed class ResponsibilityCofiguration : IEntityTypeConfiguration<Responsibility>
    {
        public void Configure(EntityTypeBuilder<Responsibility> builder)
        {
            builder.ConfigureEntity();
            builder.Property(e => e.Title).IsRequired();
        }
    }
}
