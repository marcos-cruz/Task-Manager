using System.ComponentModel.DataAnnotations;

using Bigai.TaskManager.Application.Projects.Commands.CreateProject;
using Bigai.TaskManager.Application.Projects.Commands.RemoveProject;
using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;
using Bigai.TaskManager.Application.Projects.Queries.GetProjectById;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bigai.TaskManager.Api.Controllers;

[ApiController]
[Route("api/projects/")]
[Authorize]
public class ProjectsController : MainController
{
    public ProjectsController(IMediator mediator, IBussinessNotificationsHandler bussinessNotificationsHandler) : base(bussinessNotificationsHandler, mediator)
    {
    }

    /// <summary>
    /// Gets all registered projects of a user.
    /// </summary>
    /// <param name="userId">Id that identifies the user.</param>
    /// <returns>List with all user projects.</returns>
    [HttpGet]
    [Route("users/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDto>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjectsByUserIdAsync([FromRoute][Required] int userId)
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto?>> GetProjectByIdAsync([FromRoute][Required] int projectId)
    {
        var project = await _mediator.Send(new GetProjectByIdQuery(projectId));

        return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : Ok(project);
    }

    /// <summary>
    /// Create a new project.
    /// </summary>
    /// <param name="command">Data to create the project.</param>
    /// <returns>The id of the created project.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateAsync([FromBody][Required] CreateProjectCommand command)
    {
        if (!ModelState.IsValid)
        {
            return GetResponse(ModelState);
        }

        var projectId = await _mediator.Send(command);

        return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : CreatedAtAction(nameof(GetProjectByIdAsync), new { projectId }, null);
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAsync([FromRoute][Required] int projectId)
    {
        await _mediator.Send(new RemoveProjectByIdCommand(projectId));

        return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : NoContent();
    }
}