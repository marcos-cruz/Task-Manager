using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
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

    [HttpGet]
    [Route("{projectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto?>> GetProjectByIdAsync([FromRoute] int projectId)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery(projectId));
        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
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
}