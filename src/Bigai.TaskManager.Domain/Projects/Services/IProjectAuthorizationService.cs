using Bigai.TaskManager.Domain.Projects.Constants;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;

namespace Bigai.TaskManager.Domain.Projects.Services;

public interface IProjectAuthorizationService
{
    bool Authorize(Project? project, ResourceOperation resourceOperation);
    bool AuthorizeLimit(Project? project, int MaximumTaskLimit = ProjectRoles.MaximumTaskLimit);
}