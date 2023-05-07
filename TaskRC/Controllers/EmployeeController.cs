using Microsoft.AspNetCore.Mvc;
using TaskRC.Interface;

namespace TaskRC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ITaskManager _taskManager;
        public EmployeeController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public async Task<IActionResult> GetAll()
        {
            var employeeList = await _taskManager.GetEmployees();

            var groupedByName = employeeList.GroupBy(e => e.EmployeeName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalTime = g.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).Hours)
                });

            return View(groupedByName);
        }
    }
}
