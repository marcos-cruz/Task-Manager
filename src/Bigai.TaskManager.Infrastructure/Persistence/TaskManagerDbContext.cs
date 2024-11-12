using Bigai.TaskManager.Domain.Projects.Models;

using Microsoft.EntityFrameworkCore;

namespace Bigai.TaskManager.Infrastructure.Persistence;

internal class TaskManagerDbContext : DbContext
{
    internal DbSet<Project> Projects { get; set; }
    internal DbSet<WorkUnit> WorkUnits { get; set; }

    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(project =>
        {
            project.HasKey(p => p.Id);
            project.HasMany(p => p.WorkUnits)
                   .WithOne()
                   .HasForeignKey(w => w.ProjectId);
        });

        modelBuilder.Entity<WorkUnit>(workUnit =>
        {
            workUnit.HasKey(w => w.Id);
            workUnit.HasIndex(w => w.UserId);
            workUnit.HasIndex(w => w.ProjectId);
        });
    }
}