using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace ConsoleAppDop1HW1Core;

class Program
{
    
    static void Main()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
        var config = builder.Build();
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        using (var db = new ApplicationContext(optionsBuilder.Options))
        {
            db.Database.EnsureCreated();
            InitializeTaskStatuses(db);
            bool running = true;
            while (running)
            {
                Console.WriteLine("1: Add Task, 2: View Tasks, 3: Exit");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTask(db);
                        break;
                    case "2":
                        ViewTasks(db);
                        break;
                    case "3":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }

    private static void InitializeTaskStatuses(ApplicationContext db)
    {
        if (!db.TaskStatuses.Any())
        {
            db.TaskStatuses.AddRange(
                new TaskStatus { Name = "Pending" },
                new TaskStatus { Name = "InProgress" },
                new TaskStatus { Name = "Completed" }
            );
            db.SaveChanges();
        };
    }

    private static void AddTask(ApplicationContext db)
    {
        Console.WriteLine("Enter task description:");
        string description = Console.ReadLine();

        var task = new Task { Description = description, StatusId = 1 };
        db.Tasks.Add(task);
        db.SaveChanges();

        Console.WriteLine("Task added successfully!");
    }

    private static void ViewTasks(ApplicationContext db)
    {
        var tasks = db.Tasks.Include(t => t.Status).ToList();
        foreach (var task in tasks)
        {
            Console.WriteLine($"Task {task.TaskId}: {task.Description}, Status: {task.Status.Name}");
        }
    }

    public class Task
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
        public int StatusId { get; set; }
        public TaskStatus Status { get; set; }
    }

    public class TaskStatus
    {
        [Key]
        public int TaskStatusId { get; set; }
        [Required]
        public string Name { get; set; }
    }
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<TaskStatus> TaskStatuses { get; set; }


    }
}
