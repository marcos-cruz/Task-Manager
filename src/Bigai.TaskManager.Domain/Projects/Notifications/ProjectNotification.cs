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

        public static BussinessNotification TaskLimitReached()
        {
            return new BussinessNotification()
            {
                Code = nameof(TaskLimitReached),
                Message = "Projeto já atingiu o limite máximo de tarefas permitido."
            };
        }

    }
}