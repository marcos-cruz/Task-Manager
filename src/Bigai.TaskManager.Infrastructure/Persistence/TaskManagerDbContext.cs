using Bigai.TaskManager.Domain.Projects.Models;

using Microsoft.EntityFrameworkCore;

namespace Bigai.TaskManager.Infrastructure.Persistence;

internal class TaskManagerDbContext : DbContext
{
    internal DbSet<Project> Projects { get; set; }
    internal DbSet<Domain.Projects.Models.Task> Tasks { get; set; }

    public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(project =>
        {
            project.HasKey(p => p.Id);
            project.HasMany(p => p.Tasks)
                   .WithOne()
                   .HasForeignKey(task => task.ProjectId);
            project.HasIndex(p => p.UserId);
        });

        modelBuilder.Entity<Domain.Projects.Models.Task>(task =>
        {
            task.HasKey(p => p.Id);
        });
    }
}