
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.RemoveProject;

public class RemoveProjectByIdCommandHandler : IRequestHandler<RemoveProjectByIdCommand, bool?>
{
    private readonly IProjectRepository _projectsRepository;
    private readonly IProjectAuthorizationService _projectAuthorizationService;

    public RemoveProjectByIdCommandHandler(IProjectRepository projectsRepository, IProjectAuthorizationService projectAuthorizationService)
    {
        _projectsRepository = projectsRepository;
        _projectAuthorizationService = projectAuthorizationService;
    }

    public async Task<bool?> Handle(RemoveProjectByIdCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetProjectByIdAsync(request.ProjectId);

        if (project is null)
        {
            return null;
        }

        if (!_projectAuthorizationService.Authorize(project, ResourceOperation.Remove))
        {
            return false;
        }

        await _projectsRepository.RemoveProjectAsync(project);

        return true;
    }
}