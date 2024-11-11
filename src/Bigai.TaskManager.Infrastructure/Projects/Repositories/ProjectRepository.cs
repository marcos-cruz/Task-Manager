using System.Linq.Expressions;

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
}