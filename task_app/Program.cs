using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

// Abstract class representing a Task
abstract class TaskItem
{
    public string Title { get; set; }
    public abstract void Display();
}

// Concrete class inheriting TaskItem
class BasicTask : TaskItem
{
    public bool IsCompleted { get; private set; }

    public void CompleteTask() => IsCompleted = true;

    public override void Display()
    {
        Console.WriteLine($"[ {(IsCompleted ? "✔" : "✗")} ] {Title}");
    }
}

// Interface for Task Manager operations
interface ITaskManager
{
    void AddTask(string title);
    void ListTasks();
    void CompleteTask(string title);
    void DeleteTask(string title);
}

// Prototype Pattern - Cloning tasks
interface IPrototype<T>
{
    T Clone();
}

class ClonableTask : BasicTask, IPrototype<ClonableTask>
{
    public ClonableTask(string title)
    {
        Title = title;
    }

    public ClonableTask Clone()
    {
        return new ClonableTask(this.Title);
    }
}

// Singleton TaskManager implementing ITaskManager
class TaskManager : ITaskManager
{
    private static TaskManager _instance;
    private static readonly object _lock = new object();
    private List<TaskItem> tasks = new List<TaskItem>();

    private TaskManager() { }

    public static TaskManager GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new TaskManager();
                }
            }
        }
        return _instance;
    }

    public void AddTask(string title)
    {
        tasks.Add(new ClonableTask(title));
    }

    public void ListTasks()
    {
        foreach (var task in tasks)
        {
            task.Display();
        }
    }

    public void CompleteTask(string title)
    {
        foreach (var task in tasks)
        {
            if (task.Title == title && task is BasicTask basicTask)
            {
                basicTask.CompleteTask();
            }
        }
    }

    public void DeleteTask(string title)
    {
        tasks.RemoveAll(t => t.Title == title);
    }
}

// Multithreading example - Auto-save
class BackgroundWorker
{
    public static async Task PerformBackgroundTask()
    {
        await Task.Run(() =>
        {
            while (true)
            {
                Thread.Sleep(10000);
                Console.WriteLine("[Background Task]: Checking system status...");
            }
        });
    }
}

// Main Program
class Program
{
    static async Task Main()
    {
        var taskManager = TaskManager.GetInstance();
        _ = BackgroundWorker.PerformBackgroundTask(); // Start auto-save in the background

        while (true)
        {
            Console.WriteLine("1. Add Task\n2. List Tasks\n3. Complete Task\n4. Delete Task\n5. Exit");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Enter task title: ");
                    taskManager.AddTask(Console.ReadLine());
                    break;
                case "2":
                    taskManager.ListTasks();
                    break;
                case "3":
                    Console.Write("Enter task title to complete: ");
                    taskManager.CompleteTask(Console.ReadLine());
                    break;
                case "4":
                    Console.Write("Enter task title to delete: ");
                    taskManager.DeleteTask(Console.ReadLine());
                    break;
                case "5":
                    return;
            }
        }
    }
}