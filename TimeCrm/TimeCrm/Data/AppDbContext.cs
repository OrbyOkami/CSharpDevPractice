using Microsoft.EntityFrameworkCore;
using TimeCrm.Models;

namespace TimeCrm.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Unique project code
        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Code)
            .IsUnique();

        // Configure relationships
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TimeEntry>()
            .HasOne(te => te.Task)
            .WithMany(t => t.TimeEntries)
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        // Map DateOnly to PostgreSQL date type
        modelBuilder.Entity<TimeEntry>()
            .Property(te => te.Date)
            .HasColumnType("date");

        // Configure decimal precision
        modelBuilder.Entity<TimeEntry>()
            .Property(te => te.Hours)
            .HasPrecision(10, 2);
    }
}