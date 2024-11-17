using System.ComponentModel.DataAnnotations;

using Bigai.TaskManager.Application.Projects.Queries.GetReportByProjectId;
using Bigai.TaskManager.Application.Projects.Queries.GetReportByRange;
using Bigai.TaskManager.Domain.Projects.Constants;
using Bigai.TaskManager.Domain.Projects.Contracts;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bigai.TaskManager.Api.Controllers
{
    [ApiController]
    [Route("api/projects/performance/")]
    [Authorize(Roles = TaskManagerRoles.Manager)]
    public class ReportsController : MainController
    {
        public ReportsController(IMediator mediator, IBussinessNotificationsHandler bussinessNotificationsHandler) : base(bussinessNotificationsHandler, mediator)
        {
        }

        /// <summary>
        /// Get a performance report, such as the average number of tasks completed per user over the period.
        /// </summary>
        /// <param name="initialDate">Date representing the beginning of the period in dd/mm/yyyy format.</param>
        /// <param name="finalDate">Date representing the end of the period in dd/mm/yyyy format.</param>
        /// <returns>Report containing performance during the period.</returns>
        [HttpGet]
        [Route("range")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IReportPeriod>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<IReportPeriod>>> GetReportByRangeAsync([FromQuery][Required][StringLength(10, ErrorMessage = "Informe uma data no formato dd/mm/aaaa", MinimumLength = 10)] string initialDate,
                                                                                          [Required][StringLength(10, ErrorMessage = "Informe uma data no formato dd/mm/aaaa", MinimumLength = 10)] string finalDate)
        {
            if (!ModelState.IsValid)
            {
                return GetResponse(ModelState);
            }

            DateTime initialPeriod;
            DateTime finalPeriod;

            try
            {
                initialPeriod = Convert.ToDateTime(initialDate);
                finalPeriod = Convert.ToDateTime(finalDate);
            }
            catch (Exception)
            {
                ModelState.AddModelError("Período", "Informe as datas sempre no formato dd/mm/aaaa");

                return GetResponse(ModelState);
            }

            GetReportByRangeQuery query = new GetReportByRangeQuery(initialPeriod, finalPeriod);

            var response = await _mediator.Send(query);

            return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : Ok(response);
        }

        /// <summary>
        /// Get a performance report, such as the average number of tasks completed per user over the last 30 days.
        /// </summary>
        /// <param name="initialDate">Date representing the beginning of the period in dd/mm/yyyy format.</param>
        /// <returns>Report containing performance over a 30-day period.</returns>
        [HttpGet]
        [Route("period")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IReportPeriod>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<IReportPeriod>>> GetReportByPeriodAsync([FromQuery][Required][StringLength(10, ErrorMessage = "Informe uma data no formato dd/mm/aaaa", MinimumLength = 10)] string initialDate)
        {
            if (!ModelState.IsValid)
            {
                return GetResponse(ModelState);
            }

            DateTime initialPeriod;

            try
            {
                initialPeriod = Convert.ToDateTime(initialDate);
            }
            catch (Exception)
            {
                ModelState.AddModelError(nameof(initialDate), "Informe a data sempre no formato dd/mm/aaaa");

                return GetResponse(ModelState);
            }

            GetReportByRangeQuery query = new GetReportByRangeQuery(initialPeriod, initialPeriod.AddDays(30));

            var response = await _mediator.Send(query);

            return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : Ok(response);
        }

        /// <summary>
        /// Get a performance report by project identifier, such as the average number of tasks completed per user over the period.
        /// </summary>
        /// <param name="projectId">Identifier of the project from which you want to generate the report.</param>
        /// <param name="initialDate">Date representing the beginning of the period in dd/mm/yyyy format.</param>
        /// <param name="finalDate">Date representing the end of the period in dd/mm/yyyy format.</param>
        /// <returns>Report containing performance during the period.</returns>
        [HttpGet]
        [Route("{projectId}/project")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<IReportPeriod>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<IReportPeriod>>> GetReportByProjectIdAsync([FromRoute] int projectId, [FromQuery][Required][StringLength(10, ErrorMessage = "Informe uma data no formato dd/mm/aaaa", MinimumLength = 10)] string initialDate, [Required][StringLength(10, ErrorMessage = "Informe uma data no formato dd/mm/aaaa", MinimumLength = 10)] string finalDate)
        {
            DateTime initialPeriod;
            DateTime finalPeriod;

            try
            {
                initialPeriod = Convert.ToDateTime(initialDate);
                finalPeriod = Convert.ToDateTime(finalDate);
            }
            catch (Exception)
            {
                ModelState.AddModelError("Período", "Informe as datas sempre no formato dd/mm/aaaa");

                return GetResponse(ModelState);
            }

            GetReportByProjectIdQuery query = new GetReportByProjectIdQuery(projectId, initialPeriod, finalPeriod);

            var response = await _mediator.Send(query);

            return _bussinessNotificationsHandler.HasNotification() ? GetResponse() : Ok(response);
        }
    }
}