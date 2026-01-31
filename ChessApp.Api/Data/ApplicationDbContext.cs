using Microsoft.EntityFrameworkCore;
using ChessApp.API.Models;

namespace ChessApp.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // USERS
        // =========================
        modelBuilder.Entity<User>(eb =>
        {
            eb.ToTable("Users");

            eb.HasKey(u => u.Id);

            eb.Property(u => u.Id)
                .ValueGeneratedOnAdd();

            eb.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            eb.HasIndex(u => u.Email)
                .IsUnique();

            eb.Property(u => u.EmailVerified)
                .IsRequired()
                .HasDefaultValue(false);

            eb.Property(u => u.PasswordHash)
                .HasMaxLength(255);

            eb.Property(u => u.GoogleSubject)
                .HasMaxLength(255);

            eb.Property(u => u.FailedLoginCount)
                .IsRequired()
                .HasDefaultValue(0);

            eb.Property(u => u.LockoutUntilUtc);

            eb.Property(u => u.Provider)
                .IsRequired();
        });


        // =========================
        // REFRESH TOKENS
        // =========================
        modelBuilder.Entity<RefreshToken>(eb =>
        {
            eb.ToTable("RefreshTokens");

            eb.HasKey(x => x.Id);

            eb.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(255);

            eb.HasIndex(x => x.TokenHash)
                .IsUnique();

            eb.Property(x => x.ExpiresAtUtc)
                .IsRequired();

            eb.Property(x => x.Revoked)
                .IsRequired()
                .HasDefaultValue(false);

            eb.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.HasIndex(x => x.UserId);
        });
    }
}
