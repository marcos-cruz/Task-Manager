using Bigai.TaskManager.Application.Projects.Dtos;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitById;

public class GetWorkUnitByIdQuery : IRequest<WorkUnitDto?>
{
    public int WorkUnitId { get; }

    public GetWorkUnitByIdQuery(int workUnitId)
    {
        WorkUnitId = workUnitId;
    }
}