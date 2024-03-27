using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Repository.EntityConfigurations
{
    internal sealed class ScenarioCofiguration : IEntityTypeConfiguration<Scenario>
    {
        public void Configure(EntityTypeBuilder<Scenario> builder)
        {
            builder.ConfigureEntity();
        }
    }
}
