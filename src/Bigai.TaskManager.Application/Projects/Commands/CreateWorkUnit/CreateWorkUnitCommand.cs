using System.ComponentModel.DataAnnotations;

using Bigai.TaskManager.Domain.Projects.Enums;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;

public class CreateWorkUnitCommand : IRequest<int>
{
    [Required()]
    public int ProjectId { get; set; }

    [Required(AllowEmptyStrings = false)]
    [StringLength(100)]
    public string Title { get; set; } = default!;

    [StringLength(300)]
    public string Description { get; set; } = default!;

    [Required()]
    public DateTime DueDate { get; set; } = default!;

    [Required()]
    public Priority Priority { get; set; }
}