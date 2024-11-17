using Bigai.TaskManager.Domain.Projects.Contracts;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetReportByProjectId;

public class GetReportByProjectIdQuery : IRequest<IEnumerable<IReportPeriod>>
{
    public int ProjectId { get; }
    public DateTime InitialPeriod { get; }
    public DateTime FinalPeriod { get; }

    public GetReportByProjectIdQuery(int projectId, DateTime initialPeriod, DateTime finalPeriod)
    {
        ProjectId = projectId;
        InitialPeriod = initialPeriod;
        FinalPeriod = finalPeriod;
    }
}