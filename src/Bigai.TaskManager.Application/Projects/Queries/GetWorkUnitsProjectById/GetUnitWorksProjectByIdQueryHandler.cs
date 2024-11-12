using Bigai.TaskManager.Application.Projects.Dtos;
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Repositories;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Queries.GetWorkUnitsProjectById;

public class GetUnitWorksProjectByIdQueryHandler : IRequestHandler<GetUnitWorksProjectByIdQuery, IReadOnlyCollection<WorkUnitDto>?>
{
    private readonly IProjectRepository _projectRepository;

    public GetUnitWorksProjectByIdQueryHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IReadOnlyCollection<WorkUnitDto>?> Handle(GetUnitWorksProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetProjectByIdAsync(request.ProjectId, cancellationToken);

        var workUnitsDto = project is not null ? project.WorkUnits.AsDto() : null;

        return workUnitsDto;
    }

}