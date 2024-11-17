
using System.Net;

using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IProjectRepository _projectsRepository;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public CreateProjectCommandHandler(IProjectRepository projectsRepository, IBussinessNotificationsHandler notificationsHandler)
    {
        _projectsRepository = projectsRepository;
        _notificationsHandler = notificationsHandler;
    }

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = request.AsEntity();

        var projectId = await _projectsRepository.CreateAsync(project);

        _notificationsHandler.StatusCode = HttpStatusCode.Created;

        return projectId;
    }
}