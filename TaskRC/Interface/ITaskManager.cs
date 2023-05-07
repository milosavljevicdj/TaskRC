using TaskRC.Models;
using System.Drawing;

namespace TaskRC.Interface
{
    public interface ITaskManager
    {
        Task<List<Employee>> GetEmployees();
        Task<Bitmap> GetPieChart(List<Employee> employees);
    }
}
