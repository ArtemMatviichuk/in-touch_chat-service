using ChatService.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Data.EntityConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasIndex(e => e.AuthenticationId).IsUnique();

            builder.Property(e => e.PublicId).IsRequired().HasMaxLength(128);
            builder.HasIndex(e => e.PublicId).IsUnique();
        }
    }
}