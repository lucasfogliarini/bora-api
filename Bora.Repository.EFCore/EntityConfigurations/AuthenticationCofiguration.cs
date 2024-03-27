using Bora.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bora.Repository.EntityConfigurations
{
    internal sealed class AuthenticationCofiguration : IEntityTypeConfiguration<Authentication>
    {
        public void Configure(EntityTypeBuilder<Authentication> builder)
        {
            builder.ConfigureEntity();
            builder.Property(e => e.Email).IsRequired();
            builder.Property(e => e.JwToken).IsRequired();
            builder.Property(e => e.ExpiresAt).IsRequired();
            builder.Property(e => e.Provider).IsRequired();
        }
    }
}
