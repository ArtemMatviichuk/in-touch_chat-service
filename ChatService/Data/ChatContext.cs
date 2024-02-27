using Microsoft.EntityFrameworkCore;

namespace ChatService.Data;
public class ChatContext : DbContext
{
    public ChatContext(DbContextOptions<ChatContext> opt)
        : base(opt)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}