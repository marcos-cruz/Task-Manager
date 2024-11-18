namespace Bigai.TaskManager.Domain.Projects.Models;

public sealed class Project
{
    private readonly IList<WorkUnit> _workUnits = new List<WorkUnit>();

    public int Id { get; }
    public string Name { get; private set; } = default!;
    public IReadOnlyCollection<WorkUnit> WorkUnits => _workUnits.ToArray();

    public Project() { }

    private Project(string name)
    {
        Name = name;
    }

    public static Project Create(string name)
    {
        return new Project(name);
    }

    public void AddWorkUnit(WorkUnit workUnit)
    {
        _workUnits.Add(workUnit);
    }
}