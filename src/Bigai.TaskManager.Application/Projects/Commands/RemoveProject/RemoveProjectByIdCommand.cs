using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.RemoveProject;

public class RemoveProjectByIdCommand : IRequest<bool?>
{
    public int ProjectId { get; }

    public RemoveProjectByIdCommand(int projectId)
    {
        ProjectId = projectId;
    }
}