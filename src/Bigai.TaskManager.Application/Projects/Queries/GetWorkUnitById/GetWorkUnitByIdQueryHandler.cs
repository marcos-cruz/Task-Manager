using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Repositories;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitById;

public class GetWorkUnitByIdQueryHandler : IRequestHandler<GetWorkUnitByIdQuery, WorkUnitDto?>
{
    private readonly IProjectRepository _projectRepository;

    public GetWorkUnitByIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<WorkUnitDto?> Handle(GetWorkUnitByIdQuery request, CancellationToken cancellationToken)
    {
        var workUnit = await _projectRepository.GetWorkUnitByIdAsync(request.ProjectId, request.WorkUnitId, cancellationToken);

        var workUnitDto = workUnit is not null ? workUnit.AsDto() : null;

        return workUnitDto;
    }

}