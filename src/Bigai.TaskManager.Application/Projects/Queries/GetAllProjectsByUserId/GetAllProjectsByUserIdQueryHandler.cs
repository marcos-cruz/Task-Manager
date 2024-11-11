
using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Repositories;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;

public class GetAllProjectsByUserIdQueryHandler : IRequestHandler<GetAllProjectsByUserIdQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;

    public GetAllProjectsByUserIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectDto>> Handle(GetAllProjectsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetProjectsByUserIdAsync(request.UserId, cancellationToken);

        var response = projects.Select(p => p.AsDto());

        return response;
    }
}