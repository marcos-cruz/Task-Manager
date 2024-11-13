using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.RemoveWorkUnit;

public class RemoveWorkUnitByIdCommand : IRequest<bool>
{
    public int WorkUnitId { get; }

    public RemoveWorkUnitByIdCommand(int workUnitId)
    {
        WorkUnitId = workUnitId;
    }
}