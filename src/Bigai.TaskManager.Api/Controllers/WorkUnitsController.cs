using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitsProjectById;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Bigai.TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/tasks/")]
    public class WorkUnitsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkUnitsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets a list of work units associated with a project.
        /// </summary>
        /// <param name="projectId">Identifier of the project from which you want to obtain the work units.</param>
        /// <returns>List of work units associated with a project.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WorkUnitDto>>> GetWorkUnitsByProjectIdAsync([FromRoute] int projectId)
        {
            var workUnits = await _mediator.Send(new GetUnitWorksProjectByIdQuery(projectId));

            return workUnits is null ? NotFound() : Ok(workUnits);
        }

        /// <summary>
        /// Creates a new work unit for a project.
        /// </summary>
        /// <param name="command">Data to create the work unit.</param>
        /// <returns>The id of the created work unit.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateWorkUnitCommand command)
        {
            var workUnitId = await _mediator.Send(command);

            return Created();
            // return CreatedAtAction(nameof(GetProjectByIdAsync), new { workUnitId }, null);
        }

    }
}