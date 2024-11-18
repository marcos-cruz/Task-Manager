using Bigai.TaskManager.Application.Projects.Dtos;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetProjectById;

public class GetProjectByIdQuery : IRequest<ProjectDto?>
{
    public int ProjectId { get; }

    public GetProjectByIdQuery(int projectId)
    {
        ProjectId = projectId;
    }
}