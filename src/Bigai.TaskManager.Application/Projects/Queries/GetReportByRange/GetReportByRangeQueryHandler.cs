using System.Net;

using Bigai.TaskManager.Domain.Projects.Contracts;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetReportByRange;

public class GetReportByRangeQueryHandler : IRequestHandler<GetReportByRangeQuery, IEnumerable<IReportPeriod>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public GetReportByRangeQueryHandler(IProjectRepository projectRepository, IBussinessNotificationsHandler notificationsHandler)
    {
        _projectRepository = projectRepository;
        _notificationsHandler = notificationsHandler;
    }

    public async Task<IEnumerable<IReportPeriod>> Handle(GetReportByRangeQuery request, CancellationToken cancellationToken)
    {
        var response = await _projectRepository.GetReportByRangeAsync(request.InitialPeriod, request.FinalPeriod, cancellationToken);

        _notificationsHandler.StatusCode = HttpStatusCode.OK;

        return response;
    }
}