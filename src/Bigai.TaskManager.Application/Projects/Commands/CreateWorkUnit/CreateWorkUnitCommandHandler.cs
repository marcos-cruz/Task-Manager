
using Bigai.TaskManager.Application.Projects.Mappers;
using Bigai.TaskManager.Application.Users;
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
        private readonly IUserContext _userContext;

        public CreateWorkUnitCommandHandler(IProjectRepository projectsRepository,
                                            IProjectAuthorizationService projectAuthorizationService,
                                            IBussinessNotificationsHandler notificationsHandler,
                                            IUserContext userContext)
        {
            _projectsRepository = projectsRepository;
            _projectAuthorizationService = projectAuthorizationService;
            _notificationsHandler = notificationsHandler;
            _userContext = userContext;
        }

        public async Task<int> Handle(CreateWorkUnitCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectsRepository.GetProjectByIdAsync(request.ProjectId);

            if (project == null)
            {
                _notificationsHandler.NotifyError(ProjectNotification.ProjectNotRegistered());

                return TaskManagerRoles.NotFound;
            }

            if (!_projectAuthorizationService.AuthorizeLimit(project))
            {
                _notificationsHandler.NotifyError(ProjectNotification.TaskLimitReached());

                return TaskManagerRoles.Forbidden;
            }

            var currentUser = _userContext.GetCurrentUser();
            var workUnit = request.AsEntity();
            workUnit.AssignToUser(currentUser!.UserId);

            var workUnitId = await _projectsRepository.CreateAsync(workUnit);

            return workUnitId;
        }
    }
}