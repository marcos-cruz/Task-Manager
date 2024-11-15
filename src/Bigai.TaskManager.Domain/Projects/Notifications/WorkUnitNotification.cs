namespace Bigai.TaskManager.Domain.Projects.Notifications
{
    public class WorkUnitNotification
    {
        protected WorkUnitNotification()
        {
        }

        public static BussinessNotification WorkUnitNotRegistered()
        {
            return new BussinessNotification()
            {
                Code = nameof(WorkUnitNotRegistered),
                Message = "Tarefa não cadastrada."
            };
        }

    }
}