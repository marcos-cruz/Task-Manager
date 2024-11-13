using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Bigai.TaskManager.Infrastructure.Projects.Repositories;

internal class ProjectRepository : IProjectRepository
{
    private readonly TaskManagerDbContext _taskManagerDbContext;

    public ProjectRepository(TaskManagerDbContext taskManagerDbContext)
    {
        _taskManagerDbContext = taskManagerDbContext;
    }

    public async Task<IReadOnlyCollection<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _taskManagerDbContext.Projects.Where(p => p.WorkUnits.Any(w => w.UserId == userId))
                                                   .ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetProjectByIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var project = await _taskManagerDbContext.Projects.Include(t => t.WorkUnits)
                                                          .FirstOrDefaultAsync(r => r.Id == projectId, cancellationToken);

        return project;
    }

    public async Task<WorkUnit?> GetWorkUnitByIdAsync(int workUnitId, CancellationToken cancellationToken = default)
    {
        var workUnit = await _taskManagerDbContext.WorkUnits.FirstOrDefaultAsync(r => r.Id == workUnitId, cancellationToken);

        return workUnit;
    }

    public async Task<int> CreateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _taskManagerDbContext.Projects.Add(project);

        await _taskManagerDbContext.SaveChangesAsync(cancellationToken);

        return project.Id;
    }

    public async Task<int> CreateAsync(WorkUnit workUnit, CancellationToken cancellationToken = default)
    {
        _taskManagerDbContext.WorkUnits.Add(workUnit);

        await _taskManagerDbContext.SaveChangesAsync(cancellationToken);

        return workUnit.Id;
    }

    public async Task RemoveProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        _taskManagerDbContext.Projects.Remove(project);

        await _taskManagerDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveWorkUnitAsync(WorkUnit workUnit, CancellationToken cancellationToken = default)
    {
        _taskManagerDbContext.WorkUnits.Remove(workUnit);

        await _taskManagerDbContext.SaveChangesAsync(cancellationToken);
    }
}