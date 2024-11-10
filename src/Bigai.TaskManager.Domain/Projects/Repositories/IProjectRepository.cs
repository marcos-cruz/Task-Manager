using System.Linq.Expressions;

using Bigai.TaskManager.Domain.Projects.Models;

namespace Bigai.TaskManager.Domain.Projects.Repositories;

public interface IProjectRepository
{
    Task<IReadOnlyCollection<Project>> GetAllAsync(Expression<Func<Project, bool>> filter, CancellationToken cancellationToken = default);
}