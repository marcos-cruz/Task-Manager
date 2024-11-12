using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Services;

namespace Bigai.TaskManager.Infrastructure.Projects.Services;

public class ProjectAuthorizationService : IProjectAuthorizationService
{
    public ProjectAuthorizationService()
    {
    }

    public bool Authorize(Project? project, ResourceOperation resourceOperation)
    {
        bool authorized = project is null || project.WorkUnits.Any(w => w.Status == Status.Pending);

        return !authorized;
    }

    public bool AuthorizeLimit(Project? project, int MaximumTaskLimit = 20)
    {
        bool authorized = project is not null && project.WorkUnits.Count < MaximumTaskLimit;

        return authorized;
    }
}