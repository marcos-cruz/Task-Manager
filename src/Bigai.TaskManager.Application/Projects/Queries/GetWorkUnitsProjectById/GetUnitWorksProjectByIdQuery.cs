using Bigai.TaskManager.Application.Projects.Dtos;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitsProjectById;

public class GetUnitWorksProjectByIdQuery : IRequest<IReadOnlyCollection<WorkUnitDto>?>
{
    public int ProjectId { get; }

    public GetUnitWorksProjectByIdQuery(int projectId)
    {
        ProjectId = projectId;
    }
}