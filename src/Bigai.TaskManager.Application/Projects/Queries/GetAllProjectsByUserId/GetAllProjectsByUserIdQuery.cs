using Bigai.TaskManager.Application.Projects.Dtos;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetAllProjectsByUserId;

public class GetAllProjectsByUserIdQuery : IRequest<IEnumerable<ProjectDto>>
{
    public int UserId { get; }

    public GetAllProjectsByUserIdQuery(int userId)
    {
        UserId = userId;
    }
}