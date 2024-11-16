using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Domain.Projects.Contracts;
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

    public async Task<WorkUnit?> GetWorkUnitByIdAsync(int projectId, int workUnitId, CancellationToken cancellationToken = default)
    {
        var workUnit = await _taskManagerDbContext.WorkUnits.FirstOrDefaultAsync(r => r.Id == workUnitId && r.ProjectId == projectId, cancellationToken);

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

    public async Task<int> RemoveProjectAsync(Project project, CancellationToken cancellationToken = default)
    {
        _taskManagerDbContext.Projects.Remove(project);

        var deletedRows = await _taskManagerDbContext.SaveChangesAsync(cancellationToken);

        return deletedRows;
    }

    public async Task<int> RemoveWorkUnitAsync(WorkUnit workUnit, CancellationToken cancellationToken = default)
    {
        _taskManagerDbContext.WorkUnits.Remove(workUnit);

        var deletedRows = await _taskManagerDbContext.SaveChangesAsync(cancellationToken);

        return deletedRows;
    }

    public async Task<int> UpdateAsync(WorkUnit updatedWorkUnit, CancellationToken cancellationToken = default)
    {
        _taskManagerDbContext.WorkUnits.Update(updatedWorkUnit);

        var updatedRows = await _taskManagerDbContext.SaveChangesAsync(cancellationToken);

        return updatedRows;
    }

    public async Task<IReadOnlyCollection<IReportPeriod>> GetReportByRangeAsync(DateTime initialRange, DateTime finalRange, CancellationToken cancellationToken = default)
    {
        double period = (finalRange - initialRange).TotalDays == 0 ? 1 : (finalRange - initialRange).TotalDays;
        initialRange = initialRange.AddDays(-1);
        finalRange = finalRange.AddDays(1);

        var performancePeriod = await _taskManagerDbContext.WorkUnits
            .Where(workUnit => workUnit.Started > initialRange && workUnit.Finished < finalRange)
            .GroupBy(x => x.UserId)
            .Select(x => new PerformancePeriodDto
            {
                TotalMonth = x.Count(),
                UserId = x.Key,
                AverageMonth = x.Count() / period
            }).ToListAsync(cancellationToken);

        return performancePeriod;
    }

    public async Task<IReadOnlyCollection<IReportPeriod>> GetReportByProjectIdAsync(int projectId, DateTime initialRange, DateTime finalRange, CancellationToken cancellationToken = default)
    {
        double period = (finalRange - initialRange).TotalDays;
        initialRange = initialRange.AddDays(-1);
        finalRange = finalRange.AddDays(1);

        var performancePeriod = await _taskManagerDbContext.WorkUnits
            .Where(workUnit => workUnit.ProjectId == projectId && workUnit.Started >= initialRange && workUnit.Finished <= finalRange)
            .GroupBy(x => x.UserId)
            .Select(x => new PerformancePeriodDto
            {
                TotalMonth = x.Count(),
                UserId = x.Key,
                AverageMonth = x.Count() / period
            }).ToListAsync(cancellationToken);

        return performancePeriod;
    }
}