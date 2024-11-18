using Bigai.TaskManager.Domain.Projects.Contracts;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetReportByRange;

public class GetReportByRangeQuery : IRequest<IEnumerable<IReportPeriod>>
{
    public DateTime InitialPeriod { get; }
    public DateTime FinalPeriod { get; }

    public GetReportByRangeQuery(DateTime initialPeriod, DateTime finalPeriod)
    {
        InitialPeriod = initialPeriod;
        FinalPeriod = finalPeriod;
    }
}