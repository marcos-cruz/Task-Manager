using System.ComponentModel.DataAnnotations;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.CreateProject;

public class CreateProjectCommand : IRequest<int>
{
    [Required(AllowEmptyStrings = false)]
    [StringLength(100)]
    public string Name { get; set; } = default!;
}