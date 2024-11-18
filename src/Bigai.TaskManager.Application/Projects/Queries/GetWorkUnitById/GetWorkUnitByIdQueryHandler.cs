using System.Net;

using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Notifications;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitById;

public class GetWorkUnitByIdQueryHandler : IRequestHandler<GetWorkUnitByIdQuery, WorkUnitDto?>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public GetWorkUnitByIdQueryHandler(IProjectRepository projectRepository, IBussinessNotificationsHandler notificationsHandler)
    {
        _projectRepository = projectRepository;
        _notificationsHandler = notificationsHandler;
    }

    public async Task<WorkUnitDto?> Handle(GetWorkUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var workUnit = await _projectRepository.GetWorkUnitByIdAsync(request.ProjectId, request.WorkUnitId, cancellationToken);

        if (workUnit == null)
        {
            _notificationsHandler.NotifyError(WorkUnitNotification.WorkUnitNotRegistered());
            _notificationsHandler.StatusCode = HttpStatusCode.NotFound;

            return null;
        }

        _notificationsHandler.StatusCode = HttpStatusCode.OK;

        return workUnit.AsDto();
    }
}