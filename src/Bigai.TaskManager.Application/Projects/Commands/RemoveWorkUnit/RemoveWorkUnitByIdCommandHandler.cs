using Bigai.TaskManager.Domain.Projects.Repositories;

using MediatR;

using Microsoft.AspNetCore.Http.Timeouts;

namespace Bigai.TaskManager.Application.Projects.Commands.RemoveWorkUnit;

public class RemoveWorkUnitByIdCommandHandler : IRequestHandler<RemoveWorkUnitByIdCommand, bool>
{
    private readonly IProjectRepository _projectsRepository;

    public RemoveWorkUnitByIdCommandHandler(IProjectRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public async Task<bool> Handle(RemoveWorkUnitByIdCommand request, CancellationToken cancellationToken)
    {
        var workUnit = await _projectsRepository.GetWorkUnitByIdAsync(request.ProjectId, request.WorkUnitId, cancellationToken);

        if (workUnit is null)
        {
            return false;
        }

        await _projectsRepository.RemoveWorkUnitAsync(workUnit);

        return true;
    }
}