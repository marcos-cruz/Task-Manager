using System.Net;

using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Notifications;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitsProjectById;

public class GetUnitWorksProjectByIdQueryHandler : IRequestHandler<GetUnitWorksProjectByIdQuery, IReadOnlyCollection<WorkUnitDto>?>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IBussinessNotificationsHandler _notificationsHandler;


    public GetUnitWorksProjectByIdQueryHandler(IProjectRepository projectRepository, IBussinessNotificationsHandler notificationsHandler)
    {
        _projectRepository = projectRepository;
        _notificationsHandler = notificationsHandler;
    }

    public async Task<IReadOnlyCollection<WorkUnitDto>?> Handle(GetUnitWorksProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectByIdAsync(request.ProjectId, cancellationToken);

        if (project == null)
        {
            _notificationsHandler.NotifyError(ProjectNotification.ProjectNotRegistered());
            _notificationsHandler.StatusCode = HttpStatusCode.NotFound;

            return null;
        }

        _notificationsHandler.StatusCode = HttpStatusCode.OK;

        return project.WorkUnits.AsDto();
    }
}