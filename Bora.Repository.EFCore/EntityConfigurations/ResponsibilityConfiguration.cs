using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Repository.EntityConfigurations
{
    internal sealed class ResponsibilityConfiguration : IEntityTypeConfiguration<Responsibility>
    {
        public void Configure(EntityTypeBuilder<Responsibility> builder)
        {
            builder.ConfigureEntity();
            builder.Property(e => e.Title).IsRequired();

            builder
            .HasMany(r => r.Accounts)
            .WithMany(a => a.Responsibilities)
            .UsingEntity<Dictionary<string, object>>(
                nameof(AccountResponsibility),
                j => j.HasOne<Account>().WithMany().HasForeignKey(nameof(AccountResponsibility.AccountId)),
                j => j.HasOne<Responsibility>().WithMany().HasForeignKey(nameof(AccountResponsibility.ResponsibilityId))
            );
        }
    }
}
