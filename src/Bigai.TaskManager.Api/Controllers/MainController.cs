using System.Net;

using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Bigai.TaskManager.Api.Controllers;

[Produces("application/json")]
[ApiController]
public abstract class MainController : ControllerBase
{
    protected readonly IBussinessNotificationsHandler _bussinessNotificationsHandler;
    protected readonly IMediator _mediator;

    protected MainController(IBussinessNotificationsHandler bussinessNotificationsHandler,
                             IMediator mediator)
    {
        _bussinessNotificationsHandler = bussinessNotificationsHandler ?? throw new ArgumentNullException(nameof(bussinessNotificationsHandler));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    protected ObjectResult GetResponse(ModelStateDictionary modelState)
    {
        _bussinessNotificationsHandler.StatusCode = HttpStatusCode.BadRequest;

        var errors = modelState.Where(m => m.Value!.Errors.Count() > 0);

        foreach (var error in errors)
        {
            _bussinessNotificationsHandler.NotifyError(error.Key, error.Value!.Errors[0].ErrorMessage);
        }

        return StatusCode((int)_bussinessNotificationsHandler.StatusCode, GetProblemDetails());
    }

    protected ObjectResult GetResponse()
    {
        return StatusCode((int)_bussinessNotificationsHandler.StatusCode, GetProblemDetails());
    }

    private IReadOnlyCollection<ProblemDetails> GetProblemDetails()
    {
        var problemDetails = new List<ProblemDetails>();

        _bussinessNotificationsHandler.GetNotifications()
                                      .ToList()
                                      .ForEach(notification =>
                                      {
                                          var problemDetail = new ProblemDetails()
                                          {
                                              Title = notification.Code,
                                              Status = (int)_bussinessNotificationsHandler.StatusCode,
                                              Detail = notification.Message,
                                          };

                                          problemDetails.Add(problemDetail);
                                      });

        return problemDetails;
    }
}