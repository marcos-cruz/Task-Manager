namespace Bigai.TaskManager.Application.Projects.Dtos;

public record ProjectDto(int ProjectId, string? Name, IReadOnlyCollection<WorkUnitDto> WorkUnits);