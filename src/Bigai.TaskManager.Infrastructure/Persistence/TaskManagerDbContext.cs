using Bigai.TaskManager.Domain.Projects.Models;

using Microsoft.EntityFrameworkCore;

namespace Bigai.TaskManager.Infrastructure.Persistence;

internal class TaskManagerDbContext : DbContext
{
    internal DbSet<Project> Projects { get; set; }
    internal DbSet<Domain.Projects.Models.WorkUnit> Tasks { get; set; }

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
                   .HasForeignKey(task => task.ProjectId);
        });

        modelBuilder.Entity<WorkUnit>(workUnit =>
        {
            workUnit.HasKey(p => p.Id);
            workUnit.HasIndex(p => p.UserId);
            workUnit.HasIndex(p => p.ProjectId);
        });
    }
}