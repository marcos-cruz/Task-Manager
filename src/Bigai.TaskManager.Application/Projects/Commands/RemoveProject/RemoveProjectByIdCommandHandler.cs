
using System.Net;

using Bigai.TaskManager.Domain.Projects.Constants;
using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Notifications;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.RemoveProject;

public class RemoveProjectByIdCommandHandler : IRequestHandler<RemoveProjectByIdCommand, int>
{
    private readonly IProjectRepository _projectsRepository;
    private readonly IProjectAuthorizationService _projectAuthorizationService;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public RemoveProjectByIdCommandHandler(IProjectRepository projectsRepository,
                                           IProjectAuthorizationService projectAuthorizationService,
                                           IBussinessNotificationsHandler notificationsHandler)
    {
        _projectsRepository = projectsRepository;
        _projectAuthorizationService = projectAuthorizationService;
        _notificationsHandler = notificationsHandler;
    }

    public async Task<int> Handle(RemoveProjectByIdCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetProjectByIdAsync(request.ProjectId);

        if (project == null)
        {
            _notificationsHandler.NotifyError(ProjectNotification.ProjectNotRegistered());
            _notificationsHandler.StatusCode = HttpStatusCode.NotFound;

            return TaskManagerRoles.Error;
        }

        if (!_projectAuthorizationService.Authorize(project, ResourceOperation.Remove))
        {
            _notificationsHandler.NotifyError(ProjectNotification.ProjectHasPendingWorkUnit(request.ProjectId));
            _notificationsHandler.StatusCode = HttpStatusCode.BadRequest;

            return TaskManagerRoles.Error;
        }

        await _projectsRepository.RemoveProjectAsync(project, cancellationToken);

        _notificationsHandler.StatusCode = HttpStatusCode.NoContent;

        return TaskManagerRoles.Success;
    }
}