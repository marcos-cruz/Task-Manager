
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Domain.Projects.Constants;
using Bigai.TaskManager.Domain.Projects.Notifications;
using Bigai.TaskManager.Domain.Projects.Repositories;
using Bigai.TaskManager.Domain.Projects.Services;

using MediatR;

namespace Bigai.TaskManager.Application.Projects.Commands.CreateWorkUnit
{
    public class CreateWorkUnitCommandHandler : IRequestHandler<CreateWorkUnitCommand, int>
    {
        private readonly IProjectRepository _projectsRepository;
        private readonly IProjectAuthorizationService _projectAuthorizationService;
        private readonly IBussinessNotificationsHandler _notificationsHandler;

        public CreateWorkUnitCommandHandler(IProjectRepository projectsRepository,
                                            IProjectAuthorizationService projectAuthorizationService,
                                            IBussinessNotificationsHandler notificationsHandler)
        {
            _projectsRepository = projectsRepository;
            _projectAuthorizationService = projectAuthorizationService;
            _notificationsHandler = notificationsHandler;
        }

        public async Task<int> Handle(CreateWorkUnitCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectsRepository.GetProjectByIdAsync(request.ProjectId);

            if (project == null)
            {
                _notificationsHandler.NotifyError(ProjectNotification.ProjectNotRegistered());

                return ProjectRoles.NotFound;
            }

            if (!_projectAuthorizationService.AuthorizeLimit(project))
            {
                _notificationsHandler.NotifyError(ProjectNotification.TaskLimitReached());

                return ProjectRoles.Forbidden;
            }

            var workUnit = request.AsEntity();

            var workUnitId = await _projectsRepository.CreateAsync(workUnit);

            return workUnitId;
        }
    }
}