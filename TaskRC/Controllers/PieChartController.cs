using Microsoft.AspNetCore.Mvc;
using TaskRC.Interface;

namespace TaskRC.Controllers
{
    public class PieChartController : Controller
    {
        private readonly ITaskManager _taskManager;
        public PieChartController(ITaskManager taskManager)
        {
            _taskManager = taskManager;
        }

        public async Task<IActionResult> MakePieChart()
        {
            var employees = await _taskManager.GetEmployees();

            var chart = await _taskManager.GetPieChart(employees);

            return View(chart);
        }
    }
}
