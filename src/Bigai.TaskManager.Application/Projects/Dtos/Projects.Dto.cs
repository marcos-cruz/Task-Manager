using Bigai.TaskManager.Domain.Projects.Enums;

namespace Bigai.TaskManager.Application.Projects.Dtos
{
    public record ProjectDto(int ProjectId, int UserId, string? Name, Priority Priority);
}