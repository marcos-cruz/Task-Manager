using Bigai.TaskManager.Domain.Projects.Contracts;

namespace Bigai.TaskManager.Application.Projects.Dtos;

public class PerformancePeriodDto : IReportPeriod
{
    public int? UserId { get; set; }
    public int TotalMonth { get; set; }
    public double AverageMonth { get; set; }
}