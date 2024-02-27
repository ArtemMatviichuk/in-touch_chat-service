using ChatService.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Data.EntityConfigurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.Property(e => e.Content).IsRequired();

            builder.HasOne(e => e.Sender).WithMany().HasForeignKey(e => e.SenderId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e => e.Chat).WithMany().HasForeignKey(e => e.ChatId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
