namespace Bigai.TaskManager.Domain.Projects.Contracts;

public interface IReportPeriod
{
    public int? UserId { get; set; }
    public int TotalMonth { get; set; }
    public double AverageMonth { get; set; }
}