using System.Net;

using Bigai.TaskManager.Domain.Projects.Contracts;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetReportByProjectId;

public class GetReportByProjectIdQueryHandler : IRequestHandler<GetReportByProjectIdQuery, IEnumerable<IReportPeriod>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public GetReportByProjectIdQueryHandler(IProjectRepository projectRepository, IBussinessNotificationsHandler notificationsHandler)
    {
        _projectRepository = projectRepository;
        _notificationsHandler = notificationsHandler;
    }

    public async Task<IEnumerable<IReportPeriod>> Handle(GetReportByProjectIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _projectRepository.GetReportByProjectIdAsync(request.ProjectId, request.InitialPeriod, request.FinalPeriod, cancellationToken);

        _notificationsHandler.StatusCode = HttpStatusCode.OK;

        return response;
    }
}