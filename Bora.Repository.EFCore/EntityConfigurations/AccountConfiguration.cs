using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Repository.EntityConfigurations
{
	internal sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ConfigureEntity();
            builder.HasIndex(e => e.Username).IsUnique();
            builder.Property(e => e.Username).IsRequired();
            builder.Property(e => e.Name).IsRequired();
            builder.Property(e => e.Email).IsRequired();
            builder.Property(e => e.CalendarAuthorized).IsRequired();
            builder.Property(e => e.IsPartner).IsRequired();
            builder.Property(e => e.PartnerCommentsEnabled).IsRequired();
            builder.Property(e => e.PartnerCallsOpen).IsRequired();
            builder.Property(e => e.EventVisibility).IsRequired();
            builder.Property(e => e.OnlySelfOrganizer).IsRequired();
            builder.Property(e => e.CalendarAccessToken);
            builder.Property(e => e.CalendarRefreshAccessToken);

            builder
            .HasMany(a => a.Responsibilities)
            .WithMany(r => r.Accounts)
            .UsingEntity<Dictionary<string, object>>(
                nameof(AccountResponsibility),
                j => j.HasOne<Responsibility>().WithMany().HasForeignKey(nameof(AccountResponsibility.ResponsibilityId)),
                j => j.HasOne<Account>().WithMany().HasForeignKey(nameof(AccountResponsibility.AccountId))
            );
        }
    }
}
