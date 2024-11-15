
using System.Net;

using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Application.Users;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Notifications;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.UpdateWorkUnit;

public class UpdateWorkUnitCommandHandler : IRequestHandler<UpdateWorkUnitCommand, HttpStatusCode>
{
    private readonly IProjectRepository _projectsRepository;
    private readonly IBussinessNotificationsHandler _notificationsHandler;
    private readonly ISerializeService _serializeService;
    private readonly IUserContext _userContext;

    public UpdateWorkUnitCommandHandler(IProjectRepository projectsRepository,
                                        IBussinessNotificationsHandler notificationsHandler,
                                        ISerializeService serializeService,
                                        IUserContext userContext)
    {
        _projectsRepository = projectsRepository;
        _notificationsHandler = notificationsHandler;
        _serializeService = serializeService;
        _userContext = userContext;
    }

    public async Task<HttpStatusCode> Handle(UpdateWorkUnitCommand request, CancellationToken cancellationToken)
    {
        var existingWorkUnit = await _projectsRepository.GetWorkUnitByIdAsync(request.ProjectId, request.WorkUnitId);

        if (existingWorkUnit == null)
        {
            _notificationsHandler.NotifyError(WorkUnitNotification.WorkUnitNotRegistered());

            return HttpStatusCode.NotFound;
        }

        var changeRequest = request.AsEntity(existingWorkUnit);

        var changedValues = existingWorkUnit.GetDelta(changeRequest);

        if (changedValues is not null)
        {
            History history = History.Create(existingWorkUnit, changedValues, _serializeService);

            var currentUser = _userContext.GetCurrentUser();
            history.AssignToUser(currentUser!.UserId);

            existingWorkUnit.AddHistory(history);

            await _projectsRepository.UpdateAsync(existingWorkUnit);
        }

        return HttpStatusCode.NoContent;
    }
}