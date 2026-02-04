using Microsoft.EntityFrameworkCore;
using ChessApp.API.Models;

namespace ChessApp.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        // Chess domain
    public DbSet<RepertoireItem> RepertoireItems => Set<RepertoireItem>();
    public DbSet<Opening> Openings => Set<Opening>();
    public DbSet<OpeningNode> OpeningNodes => Set<OpeningNode>();
    public DbSet<TrainingNodeStats> TrainingNodeStats => Set<TrainingNodeStats>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =========================
        // USERS
        // =========================
        modelBuilder.Entity<User>(eb =>
        {
            eb.ToTable("Users");

            eb.Property(u => u.Id).ValueGeneratedOnAdd();

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

        
        // =========================
        // OPENINGS
        // =========================
        modelBuilder.Entity<Opening>(eb =>
        {
            eb.ToTable("Openings");
            eb.HasKey(x => x.Id);

            eb.Property(x => x.UserId).IsRequired();
            eb.Property(x => x.Name).IsRequired().HasMaxLength(120);
            eb.Property(x => x.Color).IsRequired();
            eb.Property(x => x.CreatedAtUtc).IsRequired();

            eb.HasOne(x => x.RootNode)
            .WithMany() 
            .HasForeignKey(x => x.RootNodeId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

            eb.HasIndex(x => new { x.UserId, x.Color, x.Name }).IsUnique();
        });

        // =========================
        // REPERTOIRE ITEMS (tree)
        // =========================
        modelBuilder.Entity<RepertoireItem>(eb =>
        {
            eb.ToTable("RepertoireItems");

            eb.HasKey(x => x.Id);

            eb.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            eb.Property(x => x.UserId)
                .IsRequired();

            eb.Property(x => x.Type)
                .IsRequired(); // Opening

            eb.Property(x => x.Color)
                .IsRequired(); // White / Black

            eb.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(120);

            eb.Property(x => x.SortOrder)
                .IsRequired()
                .HasDefaultValue(0);

            eb.Property(x => x.OpeningId)
                .IsRequired(false);

            eb.HasOne(x => x.Opening)
                .WithMany()
                .HasForeignKey(x => x.OpeningId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.HasIndex(x => new { x.UserId, x.Color, x.SortOrder });
            eb.HasIndex(x => new { x.UserId, x.Color, x.Name })
                .IsUnique();
        });

        // =========================
        // OPENING NODES (move tree)
        // =========================
        modelBuilder.Entity<OpeningNode>(eb =>
        {
            eb.ToTable("OpeningNodes");
            eb.HasKey(x => x.Id);

            eb.Property(x => x.Fen).IsRequired().HasMaxLength(120);
            eb.Property(x => x.MoveSan).HasMaxLength(20);
            eb.Property(x => x.LineType).IsRequired();
            eb.Property(x => x.Comment).HasMaxLength(2000);
            eb.Property(x => x.CreatedAtUtc).IsRequired();

            eb.HasOne(x => x.Opening)
            .WithMany() 
            .HasForeignKey(x => x.OpeningId)
            .OnDelete(DeleteBehavior.Cascade);

            eb.HasOne(x => x.ParentNode)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentNodeId)
            .OnDelete(DeleteBehavior.Cascade);

            eb.HasIndex(x => new { x.OpeningId, x.ParentNodeId });
            eb.HasIndex(x => new { x.OpeningId, x.Fen }).IsUnique();
        });
        // =========================
        // TRAINING NODE STATS
        // =========================
        modelBuilder.Entity<TrainingNodeStats>(eb =>
        {
            eb.ToTable("TrainingNodeStats");

            eb.HasKey(x => x.Id);

            eb.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            eb.Property(x => x.UserId)
                .IsRequired();

            eb.Property(x => x.OpeningNodeId)
                .IsRequired();

            eb.Property(x => x.TrainedCount)
                .IsRequired()
                .HasDefaultValue(0);

            eb.Property(x => x.FailedCount)
                .IsRequired()
                .HasDefaultValue(0);

            eb.Property(x => x.LastTrainedAtUtc);

            eb.Property(x => x.NextDueAtUtc);

            eb.HasIndex(x => new { x.UserId, x.OpeningNodeId })
                .IsUnique();

            eb.HasOne(x => x.OpeningNode)
                .WithMany() 
                .HasForeignKey(x => x.OpeningNodeId)
                .OnDelete(DeleteBehavior.Cascade);

            eb.HasIndex(x => x.UserId);
            eb.HasIndex(x => x.OpeningNodeId);
        });
    }
}
