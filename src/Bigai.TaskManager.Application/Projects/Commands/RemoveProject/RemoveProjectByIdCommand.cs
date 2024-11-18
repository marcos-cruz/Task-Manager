using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.RemoveProject;

public class RemoveProjectByIdCommand : IRequest<int>
{
    public int ProjectId { get; }

    public RemoveProjectByIdCommand(int projectId)
    {
        ProjectId = projectId;
    }
}