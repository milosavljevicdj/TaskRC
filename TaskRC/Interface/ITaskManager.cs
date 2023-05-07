using TaskRC.Models;

namespace TaskRC.Interface
{
    public interface ITaskManager
    {
        Task<List<Employee>> GetEmployees();
    }
}
