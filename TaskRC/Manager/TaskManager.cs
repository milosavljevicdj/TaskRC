using TaskRC.Models;
using Newtonsoft.Json;
using TaskRC.Interface;
using System.Drawing;
using System.Drawing.Imaging;


namespace TaskRC.Manager
{
    public class TaskManager : ITaskManager
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TaskManager(IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = new HttpClient();
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<Bitmap> GetPieChart(List<Employee> employees)
        {

            var groupedByName = employees.GroupBy(e => e.EmployeeName)
                .Select(g => new
                {
                    Name = g.Key,
                    TotalTime = g.Sum(e => (e.EndTimeUtc - e.StarTimeUtc).Hours)
                });

            double totalWorkTime = 0;

            foreach (var employee in groupedByName)
            {
                totalWorkTime += employee.TotalTime;
            }

            Bitmap chart = new Bitmap(700, 700);
            Graphics g = Graphics.FromImage(chart);

            g.Clear(Color.White);

            float startAngle = 0;

            float legendX = 400;
            float legendY = 20;
            int legendRectSize = 10;
            int legendSpacing = 5;
            int legendTextOffset = legendRectSize + legendSpacing;

            foreach (var employee in groupedByName)
            {
                Random random = new Random();

                int red = random.Next(0, 256);
                int green = random.Next(0, 256);
                int blue = random.Next(0, 256);
                Color randomColor = Color.FromArgb(red, green, blue);
                Brush brush = new SolidBrush(randomColor);

                float percentage = (float)employee.TotalTime / (float)totalWorkTime;
                float sweepAngle = percentage * 360f;

                RectangleF rect = new RectangleF(60, 10, 300, 300);
                g.FillPie(brush, rect, startAngle, sweepAngle);

                string percentageString = string.Format("{0:0.##}%", percentage * 100f);
                var font = new Font("Arial", 10);
                var textSize = g.MeasureString(percentageString, font);

                PointF textPoint = GetTextPoint(rect, startAngle, sweepAngle);
                textPoint.X -= textSize.Width / 2;
                textPoint.Y -= textSize.Height / 2;

                g.DrawString(percentageString, font, Brushes.White, textPoint);

                RectangleF legendRect = new RectangleF(legendX, legendY, legendRectSize, legendRectSize);
                g.FillRectangle(brush, legendRect);

                string legendText = $"{employee.Name} - {percentageString}";
                SizeF legendTextSize = g.MeasureString(legendText, new Font("Arial", 10));

                RectangleF legendTextRect;

                if (legendY + legendTextSize.Height + legendSpacing > chart.Height)
                {
                    legendY = 20;
                    legendX += legendRect.Width + legendTextOffset;
                }

                legendTextRect = new RectangleF(legendX + legendTextOffset, legendY, legendTextSize.Width, legendTextSize.Height);
                g.DrawString(legendText, new Font("Arial", 10), Brushes.Black, legendTextRect);

                legendY += legendTextSize.Height + legendSpacing;

                startAngle += sweepAngle;
            }

            string wwwrootpath = _webHostEnvironment.WebRootPath;
            string imagePath = Path.Combine(wwwrootpath, "chart", "chart.png");

            chart.Save(imagePath, ImageFormat.Png);

            return chart;
        }
        private PointF GetTextPoint(RectangleF rect, float startAngle, float sweepAngle)
        {
            double radians = (Math.PI / 180) * (startAngle + (sweepAngle / 2));
            float x = rect.X + (rect.Width / 2) + ((rect.Width / 2 - 40) * (float)Math.Cos(radians));
            float y = rect.Y + (rect.Height / 2) + ((rect.Height / 2 - 40) * (float)Math.Sin(radians));
            return new PointF(x, y);
        }
    }

}
