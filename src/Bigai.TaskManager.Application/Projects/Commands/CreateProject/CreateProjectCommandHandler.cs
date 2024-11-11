
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Repositories;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectRepository _projectsRepository;

    public CreateProjectCommandHandler(IProjectRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = request.AsEntity();

        var projectId = await _projectsRepository.CreateAsync(project);

        return projectId;
    }
}