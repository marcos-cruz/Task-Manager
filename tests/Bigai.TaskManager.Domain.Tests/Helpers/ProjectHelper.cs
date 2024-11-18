using Bigai.TaskManager.Domain.Projects.Enums;
using Bigai.TaskManager.Domain.Projects.Models;

namespace Bigai.TaskManager.Domain.Tests.Helpers;

public static class ProjectHelper
{
    public static IReadOnlyCollection<Project> GetProjects(int amount, int userId)
    {
        var projects = new List<Project>();
        if (amount > 0)
        {
            Random rnd = new Random();

            for (int i = 0; i < amount; i++)
            {
                var project = Project.Create($"Test Project {Guid.NewGuid()}");

                for (int j = 0; j < 7; j++)
                {
                    var priority = (Priority)rnd.Next(0, 2);
                    var dueDate = DateTime.Now.AddDays(rnd.Next(15, 45));

                    var workUnit = WorkUnit.Create("Work unit title", "Work unit description", dueDate, priority);
                    workUnit.AssignToUser(userId);
                    project.AddWorkUnit(workUnit);
                }

                projects.Add(project);
            }
        }

        return projects;
    }
}