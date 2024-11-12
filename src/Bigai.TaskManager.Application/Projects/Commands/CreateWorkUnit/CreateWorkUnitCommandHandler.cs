
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Models;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit
{
    public class CreateWorkUnitCommandHandler : IRequestHandler<CreateWorkUnitCommand, int>
    {
        private readonly IProjectRepository _projectsRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;

        public CreateWorkUnitCommandHandler(IProjectRepository projectsRepository, IProjectAuthorizationService projectAuthorizationService)
        {
            _projectsRepository = projectsRepository;
            _projectAuthorizationService = projectAuthorizationService;
        }

        public async Task<int> Handle(CreateWorkUnitCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectsRepository.GetProjectByIdAsync(request.ProjectId);

            //
            // TODO: Implementar Domain Notification
            //
            // if (project == null)
            // {
            //     throw new NotFoundException(nameof(Project), request.ProjectId.ToString());
            // }

            if (!_projectAuthorizationService.AuthorizeLimit(project))
            {
                //throw new ForbidException();

                return 0;
            }

            var workUnit = request.AsEntity();

            var workUnitId = await _projectsRepository.CreateAsync(workUnit);

            return workUnitId;
        }
    }
}