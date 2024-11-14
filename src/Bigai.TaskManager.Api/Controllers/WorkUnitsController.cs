using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Application.Projects.Commands.RemoveWorkUnit;
using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitById;
using Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitsProjectById;
using Bigai.TaskManager.Domain.Projects.Notifications;

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
        /// Gets a work unit by its identifier.
        /// </summary>
        /// <param name="workUnitId">Identifier of the work unit you want to obtain.</param>
        /// <param name="projectId">Identifier of the project from which you want to obtain the work unit.</param>
        /// <returns>Work unit matching identifier.</returns>
        [HttpGet]
        [Route("{workUnitId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkUnitDto?>> GetWorkUnitByIdAsync([FromRoute] int workUnitId, int projectId)
        {
            var workUnit = await _mediator.Send(new GetWorkUnitByIdQuery(projectId, workUnitId));

            return workUnit is null ? NotFound() : Ok(workUnit);
        }

        /// <summary>
        /// Creates a new work unit for a project.
        /// </summary>
        /// <param name="command">Data to create the work unit.</param>
        /// <param name="projectId">Identifier of the project from which you want to create the work unit.</param>
        /// <returns>The id of the created work unit.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateWorkUnitCommand command, int projectId)
        {
            if (projectId != command.ProjectId)
            {
                ModelState.AddModelError(nameof(projectId), ProjectNotification.ProjectNotRegistered().Message);

                return BadRequest();
            }

            var workUnitId = await _mediator.Send(command);

            return createResponse(command, workUnitId);
        }

        /// <summary>
        /// Remove a work unit.
        /// </summary>
        /// <param name="workUnitId">Work unit identifier that should be removed.</param>
        /// <param name="projectId">Identifier of the project from which you want to remove the work unit.</param>
        /// <returns>Operation status.</returns>
        [HttpDelete]
        [Route("{workUnitId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveAsync([FromRoute] int workUnitId, int projectId)
        {
            var removed = await _mediator.Send(new RemoveWorkUnitByIdCommand(projectId, workUnitId));

            return removed ? NoContent() : NotFound();
        }

        private IActionResult createResponse(CreateWorkUnitCommand command, int workUnitId)
        {
            return workUnitId * -1 == StatusCodes.Status404NotFound ? NotFound() : CreatedAtAction(nameof(GetWorkUnitByIdAsync), new { command.ProjectId, workUnitId }, null);
        }
    }
}