namespace Bigai.TaskManager.Domain.Projects.Notifications
{
    public class ProjectNotification
    {
        protected ProjectNotification()
        {
        }

        public static BussinessNotification ProjectNotRegistered()
        {
            return new BussinessNotification()
            {
                Code = nameof(ProjectNotRegistered),
                Message = "Projeto não cadastrado."
            };
        }

        public static BussinessNotification ProjectTaskLimitReached()
        {
            return new BussinessNotification()
            {
                Code = nameof(ProjectTaskLimitReached),
                Message = "Projeto já atingiu o limite máximo de tarefas permitido."
            };
        }

        public static BussinessNotification ProjectHasPendingWorkUnit(int projectId)
        {
            return new BussinessNotification()
            {
                Code = nameof(ProjectHasPendingWorkUnit),
                Message = $"O projeto {projectId} possuí tarefas pendentes. Sugerimos a conclusão do projeto ou remoção das tarefas primeiro."

            };
        }
    }
}