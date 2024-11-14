using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.RemoveWorkUnit;

public class RemoveWorkUnitByIdCommand : IRequest<bool>
{
    public int ProjectId { get; }
    public int WorkUnitId { get; }

    public RemoveWorkUnitByIdCommand(int projectId, int workUnitId)
    {
        ProjectId = projectId;
        WorkUnitId = workUnitId;
    }
}