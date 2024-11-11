using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Domain.Projects.Models;

namespace Bigai.TaskManager.Application.Projects.Mappers;

public static class ProjectMapper
{
    public static ProjectDto AsDto(this Project project)
    {
        var workUnits = project.WorkUnits.Select(w => w.AsDto()).ToArray();

        return new ProjectDto(project.Id, project.Name, workUnits);
    }

    public static Project AsEntity(this CreateProjectCommand command)
    {
        return Project.Create(command.Name);
    }
}