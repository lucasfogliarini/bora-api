using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Database.EntityConfigurations
{
    internal sealed class ScenarioCofiguration : IEntityTypeConfiguration<Scenario>
    {
        public void Configure(EntityTypeBuilder<Scenario> builder)
        {
            builder.ConfigureEntity();
        }
    }
}
