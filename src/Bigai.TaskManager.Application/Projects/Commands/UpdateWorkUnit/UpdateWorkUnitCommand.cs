using System.ComponentModel.DataAnnotations;

using Bigai.TaskManager.Domain.Projects.Enums;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.UpdateWorkUnit;

public class UpdateWorkUnitCommand : IRequest<int>
{
    [Required()]
    public int ProjectId { get; set; }

    [Required()]
    public int WorkUnitId { get; set; }

    [StringLength(100)]
    public string? Title { get; set; }

    [StringLength(300)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public Status? Status { get; set; }
}