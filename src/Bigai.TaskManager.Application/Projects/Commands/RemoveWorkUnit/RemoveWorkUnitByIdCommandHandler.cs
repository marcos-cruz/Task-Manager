using System.Net;

using Bigai.TaskManager.Domain.Projects.Constants;
using Bigai.TaskManager.Domain.Projects.Notifications;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.RemoveWorkUnit;

public class RemoveWorkUnitByIdCommandHandler : IRequestHandler<RemoveWorkUnitByIdCommand, int>
{
    private readonly IProjectRepository _projectsRepository;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public RemoveWorkUnitByIdCommandHandler(IProjectRepository projectsRepository, IBussinessNotificationsHandler notificationsHandler)
    {
        _projectsRepository = projectsRepository;
        _notificationsHandler = notificationsHandler;
    }

    public async Task<int> Handle(RemoveWorkUnitByIdCommand request, CancellationToken cancellationToken)
    {
        var workUnit = await _projectsRepository.GetWorkUnitByIdAsync(request.ProjectId, request.WorkUnitId, cancellationToken);

        if (workUnit is null)
        {
            _notificationsHandler.NotifyError(WorkUnitNotification.WorkUnitNotRegistered());
            _notificationsHandler.StatusCode = HttpStatusCode.NotFound;

            return TaskManagerRoles.Error;
        }

        await _projectsRepository.RemoveWorkUnitAsync(workUnit);

        _notificationsHandler.StatusCode = HttpStatusCode.NoContent;

        return TaskManagerRoles.Success;
    }
}