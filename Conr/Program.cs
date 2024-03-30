using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // Add this line to use JsonConvert
using System.IO; // Add this line to use File

namespace Conr
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ReadLine();
        }
        public class Task
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public bool Done { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? CompletedAt { get; set; }
            public int Priority { get; set; }
        }

        public class HomeController : Controller
        {
            private readonly ITaskService _taskService;

            public HomeController(ITaskService taskService)
            {
                _taskService = taskService;
            }

            public IActionResult Index()
            {
                var tasks = _taskService.GetAllTasks();
                return View(tasks);
            }

            [HttpPost]
            public IActionResult AddTask(Task task)
            {
                _taskService.AddTask(task);
                return RedirectToAction("Index");
            }
        }
        public class TaskController : Controller
        {
            private readonly ITaskService _taskService;

            public TaskController(ITaskService taskService)
            {
                _taskService = taskService;
            }

            public IActionResult Details(int id)
            {
                var task = _taskService.GetTaskById(id);
                return View(task);
            }

            [HttpPost]
            public IActionResult Edit(Task task)
            {
                _taskService.UpdateTask(task);
                return RedirectToAction("Details", new { id = task.Id });
            }

            [HttpPost]
            public IActionResult Delete(int id)
            {
                _taskService.DeleteTask(id);
                return RedirectToAction("Index", "Home");
            }
        }
        public interface ITaskService
        {
            List<Task> GetAllTasks();
            Task GetTaskById(int id);
            void AddTask(Task task);
            void UpdateTask(Task task);
            void DeleteTask(int id);
        }
        public class TaskService : ITaskService
        {
            private readonly string _filePath = "tasks.json";

            public List<Task> GetAllTasks()
            {
                var tasks = JsonConvert.DeserializeObject<List<Task>>(File.ReadAllText(_filePath));
                return tasks;
            }

            public Task GetTaskById(int id)
            {
                var tasks = GetAllTasks();
                return tasks.FirstOrDefault(t => t.Id == id);
            }

            public void AddTask(Task task)
            {
                var tasks = GetAllTasks();
                task.Id = tasks.Count() + 1;
                tasks.Add(task);
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(tasks));
            }

            public void UpdateTask(Task task)
            {
                var tasks = GetAllTasks();
                var taskToUpdate = tasks.FirstOrDefault(t => t.Id == task.Id);
                taskToUpdate.Title = task.Title;
                taskToUpdate.Description = task.Description;
                taskToUpdate.Done = task.Done;
                taskToUpdate.CompletedAt = task.CompletedAt;
                taskToUpdate.Priority = task.Priority;
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(tasks));
            }

            public void DeleteTask(int id)
            {
                var tasks = GetAllTasks();
                var taskToDelete = tasks.FirstOrDefault(t => t.Id == id);
                tasks.Remove(taskToDelete);
                File.WriteAllText(_filePath, JsonConvert.SerializeObject(tasks));
            }
        }
    }
}
