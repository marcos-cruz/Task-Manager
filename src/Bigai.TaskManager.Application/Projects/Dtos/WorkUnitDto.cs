using Bigai.TaskManager.Domain.Projects.Enums;

namespace Bigai.TaskManager.Application.Projects.Dtos;

public record WorkUnitDto(int WorkUnitId,
                          int? ProjectId,
                          int? UserId,
                          string Title,
                          string Description,
                          DateTimeOffset DueDate,
                          Status Status,
                          Priority Priority);