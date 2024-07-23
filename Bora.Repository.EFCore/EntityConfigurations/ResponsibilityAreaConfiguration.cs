using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Repository.EntityConfigurations
{
    internal sealed class ResponsibilityAreaConfiguration : IEntityTypeConfiguration<ResponsibilityArea>
    {
        public void Configure(EntityTypeBuilder<ResponsibilityArea> builder)
        {
            builder.ConfigureEntity();
            builder.Property(e => e.Title).IsRequired();
            builder.Property(e => e.Description).IsRequired();
        }
    }
}
