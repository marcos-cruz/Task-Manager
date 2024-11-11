using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Bigai.TaskManager.Api.Controllers;

[ApiController]
[Route("api/projects")]
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
    [Route("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProjectDto>))]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetByUserIdAsync([FromRoute] int userId)
    {
        var projects = await _mediator.Send(new GetAllProjectsByUserIdQuery(userId));

        return Ok(projects);
    }
}