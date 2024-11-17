
using System.Net;

using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;

public class GetAllProjectsByUserIdQueryHandler : IRequestHandler<GetAllProjectsByUserIdQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IBussinessNotificationsHandler _notificationsHandler;

    public GetAllProjectsByUserIdQueryHandler(IProjectRepository projectRepository, IBussinessNotificationsHandler notificationsHandler)
    {
        _projectRepository = projectRepository;
        _notificationsHandler = notificationsHandler;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(GetAllProjectsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetProjectsByUserIdAsync(request.UserId, cancellationToken);

        var response = projects.Select(p => p.AsDto());

        _notificationsHandler.StatusCode = HttpStatusCode.OK;

        return response;
    }
}