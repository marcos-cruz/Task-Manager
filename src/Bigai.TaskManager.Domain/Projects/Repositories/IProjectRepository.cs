using Bigai.TaskManager.Domain.Projects.Models;

namespace Bigai.TaskManager.Domain.Projects.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyCollection<Project>> GetProjectsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(Project project, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(WorkUnit workUnit, CancellationToken cancellationToken = default);

    Task<int> UpdateAsync(WorkUnit updatedWorkUnit, CancellationToken cancellationToken = default);

    Task<Project?> GetProjectByIdAsync(int projectId, CancellationToken cancellationToken = default);

    Task<WorkUnit?> GetWorkUnitByIdAsync(int projectId, int workUnitId, CancellationToken cancellationToken = default);

    Task<int> RemoveProjectAsync(Project project, CancellationToken cancellationToken = default);

    Task<int> RemoveWorkUnitAsync(WorkUnit workUnit, CancellationToken cancellationToken = default);
}