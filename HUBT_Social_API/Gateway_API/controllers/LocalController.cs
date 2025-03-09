using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gateway_API.controllers
{
    [Route("")]
    [ApiController]
    public class LocalController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        // GET: api/<LocalController>
        [HttpGet]
        public ContentResult Init()
        {
            var htmlPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "home.html");
            var htmlContent = System.IO.File.ReadAllText(htmlPath);

            var services = _configuration.GetSection("Services").GetChildren();
            var statusHtml = "";

            foreach (var service in services)
            {
                string serviceName = service.Key;
                string? serviceUrl = service.Value;

                if (string.IsNullOrEmpty(serviceUrl))
                {
                    statusHtml += $"<p>{serviceName} URL is not configured.</p>";
                }
                else
                {
                    statusHtml += $"<p>{serviceName}: <span data-url='{serviceUrl}' class='status'>Checking...</span></p>";
                }
            }

            // Thay thế placeholder trong file HTML
            htmlContent = htmlContent.Replace("<!--SERVICES-->", statusHtml);

            return new ContentResult
            {
                Content = htmlContent,
                ContentType = "text/html",
                StatusCode = 200
            };
        }

    }
}
