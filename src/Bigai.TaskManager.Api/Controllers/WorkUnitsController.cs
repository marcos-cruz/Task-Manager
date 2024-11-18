using System.ComponentModel.DataAnnotations;

using Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit;
using Bigai.TaskManager.Application.Projects.Commands.RemoveWorkUnit;
using Bigai.TaskManager.Application.Projects.Commands.UpdateWorkUnit;
using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitById;
using Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitsProjectById;
using Bigai.TaskManager.Domain.Projects.Notifications;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bigai.TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/projects/{projectId}/tasks/")]
    [Authorize]
    public class WorkUnitsController : MainController
    {
        public WorkUnitsController(IMediator mediator, IBussinessNotificationsHandler bussinessNotificationsHandler) : base(bussinessNotificationsHandler, mediator)
        {
        }

        /// <summary>
        /// Gets a list of work units associated with a project.
        /// </summary>
        /// <param name="projectId">Identifier of the project from which you want to obtain the work units.</param>
        /// <returns>List of work units associated with a project.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<WorkUnitDto>>> GetWorkUnitsByProjectIdAsync([FromRoute][Required] int projectId)
        {
            var workUnits = await _mediator.Send(new GetUnitWorksProjectByIdQuery(projectId));

            return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : Ok(workUnits);
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WorkUnitDto?>> GetWorkUnitByIdAsync([FromRoute][Required] int workUnitId, [Required] int projectId)
        {
            var workUnit = await _mediator.Send(new GetWorkUnitByIdQuery(projectId, workUnitId));

            return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : Ok(workUnit);
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateAsync([FromBody][Required] CreateWorkUnitCommand command, [Required] int projectId)
        {
            if (!ModelState.IsValid)
            {
                return GetResponse(ModelState);
            }

            if (projectId != command.ProjectId)
            {
                ModelState.AddModelError(nameof(projectId), ProjectNotification.ProjectNotRegistered().Message);

                return GetResponse(ModelState);
            }

            var workUnitId = await _mediator.Send(command);

            return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : CreatedAtAction(nameof(GetWorkUnitByIdAsync), new { command.ProjectId, workUnitId }, null);
        }

        /// <summary>
        /// Updates a existing work unit.
        /// </summary>
        /// <param name="command">Data to update the work unit.</param>
        /// <param name="workUnitId">Identifier of the work unit from which you want to update.</param>
        /// <param name="projectId">Identifier of the project from which you want to update the work unit.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{workUnitId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateWorkUnitCommand command, [FromRoute][Required] int workUnitId, [Required] int projectId)
        {
            if (!ModelState.IsValid)
            {
                return GetResponse(ModelState);
            }

            if (projectId != command.ProjectId)
            {
                ModelState.AddModelError(nameof(projectId), ProjectNotification.ProjectNotRegistered().Message);

                return GetResponse(ModelState);
            }

            if (workUnitId != command.WorkUnitId)
            {
                ModelState.AddModelError(nameof(workUnitId), WorkUnitNotification.WorkUnitNotRegistered().Message);

                return GetResponse(ModelState);
            }

            await _mediator.Send(command);

            return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : NoContent();
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveAsync([FromRoute][Required] int workUnitId, [Required] int projectId)
        {
            await _mediator.Send(new RemoveWorkUnitByIdCommand(projectId, workUnitId));

            return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : NoContent();
        }
    }
}