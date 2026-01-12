using Microsoft.EntityFrameworkCore;

namespace FunChatTelegramBot;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<UserChatCounter> Counters { get; set; }
    public DbSet<ChatRunLog> RunLogs { get; set; }
    public string Path { get; set; }

    public AppDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        Path = System.IO.Path.Combine(path, "FunChatTelegramBot.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={Path}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserId);
            entity.Property(u => u.UserId).IsRequired();
            entity.Property(u => u.FirstName)
                .HasMaxLength(100);
            entity.Property(u => u.LastName)
                .HasMaxLength(100);
        });
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(c => c.ChatId);
            entity.Property(c => c.ChatId).IsRequired();
        });
        modelBuilder.Entity<UserChatCounter>(entity =>
        {
            entity.HasKey(ucc => ucc.Id);
            entity.HasIndex(ucc => new {ucc.ChatId, ucc.UserId})
                .IsUnique();
            entity.HasOne(ucc => ucc.Chat)
                .WithMany(c => c.UserCounters)
                .HasForeignKey(ucc => ucc.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ucc => ucc.User)
                .WithMany(u => u.ChatCounters)
                .HasForeignKey(ucc => ucc.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ChatRunLog>(entity =>
        {
            entity.HasKey(r => r.ChatId);
            entity.Property(r => r.LastRunAt)
                .HasConversion<DateOnlyConverter>();
        });
    }
}