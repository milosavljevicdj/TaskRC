using TaskRC.Interface;
using TaskRC.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace TaskRC.Manager
{
    public class TaskManager :ITaskManager
    {
        private readonly HttpClient _httpClient;

        public TaskManager()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<Employee>> GetEmployees()
        {
            var secretKey = "vO17RnE8vuzXzPJo5eaLLjXjmRW07law99QTD90zat9FfOQJKKUcgQ==";
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://rc-vault-fap-live-1.azurewebsites.net/api/gettimeentries?code={secretKey}");
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            try
            {
                var employees = JsonConvert.DeserializeObject<List<Employee>>(json);

                return employees ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw;
            }
        }
    }
}
