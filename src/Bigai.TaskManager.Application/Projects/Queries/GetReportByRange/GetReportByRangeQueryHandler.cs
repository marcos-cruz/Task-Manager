using Bigai.TaskManager.Domain.Projects.Contracts;
using Bigai.TaskManager.Domain.Projects.Repositories;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetReportByRange;

public class GetReportByRangeQueryHandler : IRequestHandler<GetReportByRangeQuery, IEnumerable<IReportPeriod>>
{
    private readonly IProjectRepository _projectRepository;

    public GetReportByRangeQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<IReportPeriod>> Handle(GetReportByRangeQuery request, CancellationToken cancellationToken)
    {
        var response = await _projectRepository.GetReportByRangeAsync(request.InitialPeriod, request.FinalPeriod, cancellationToken);

        return response;
    }
}