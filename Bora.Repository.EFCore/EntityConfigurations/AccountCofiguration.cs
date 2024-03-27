using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Repository.EntityConfigurations
{
	internal sealed class AccountCofiguration : IEntityTypeConfiguration<Account>
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
        }
    }
}
