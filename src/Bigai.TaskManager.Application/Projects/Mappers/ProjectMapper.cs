using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Domain.Projects.Models;

namespace Bigai.TaskManager.Application.Projects.Mappers;

public static class ProjectMapper
{
    public static ProjectDto AsDto(this Project project)
    {
        return new ProjectDto(project.Id, project.UserId, project.Name, project.Priority);
    }

}