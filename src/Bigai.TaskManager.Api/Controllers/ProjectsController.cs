using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
using Bigai.TaskManager.Application.Projects.Commands.RemoveProject;
using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;
using Bigai.TaskManager.Application.Projects.Queries.GetProjectById;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Bigai.TaskManager.Api.Controllers;

[ApiController]
[Route("api/projects/")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all registered projects of a user.
    /// </summary>
    /// <param name="userId">Id that identifies the user.</param>
    /// <returns>List with all user projects.</returns>
    [HttpGet]
    [Route("users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDto>))]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByUserIdAsync([FromRoute] int userId)
    {
        var projects = await _mediator.Send(new GetAllProjectsByUserIdQuery(userId));

        return Ok(projects);
    }

    /// <summary>
    /// Gets a project by its identifier.
    /// </summary>
    /// <param name="projectId">Identifier of the project you want to obtain.</param>
    /// <returns>Project matching identifier.</returns>
    [HttpGet]
    [Route("{projectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto?>> GetProjectByIdAsync([FromRoute] int projectId)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery(projectId));

        return project is null ? NotFound() : Ok(project);
    }

    /// <summary>
    /// Create a new project.
    /// </summary>
    /// <param name="command">Data to create the project.</param>
    /// <returns>The id of the created project.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateProjectCommand command)
    {
        var projectId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetProjectByIdAsync), new { projectId }, null);
    }

    /// <summary>
    /// Remove a project.
    /// </summary>
    /// <param name="projectId">Project identifier that should be removed.</param>
    /// <returns>Operation status.</returns>
    [HttpDelete]
    [Route("{projectId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAsync([FromRoute] int projectId)
    {
        var removed = await _mediator.Send(new RemoveProjectByIdCommand(projectId));

        return removed is null ? NotFound() : RemovedResponse(projectId, removed.Value);
    }

    private IActionResult RemovedResponse(int projectId, bool removed)
    {
        return removed ? NoContent() : BadRequest($"O projeto {projectId} possuí tarefas pendentes. Sugerimos a conclusão do projeto ou remoção das tarefas primeiro.");
    }
}