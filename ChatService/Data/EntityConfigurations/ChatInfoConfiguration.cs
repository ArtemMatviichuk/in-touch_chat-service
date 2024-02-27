using ChatService.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Data.EntityConfigurations
{
    public class ChatInfoConfiguration : IEntityTypeConfiguration<ChatInfo>
    {
        public void Configure(EntityTypeBuilder<ChatInfo> builder)
        {
            builder.ToTable("ChatInfos");

            builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Description).HasMaxLength(4095);
            builder.Property(e => e.ChatImageName).HasMaxLength(255);

            builder.HasOne(e => e.Chat).WithOne(e => e.Info).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
