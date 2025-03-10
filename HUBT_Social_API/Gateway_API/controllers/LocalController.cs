using Microsoft.AspNetCore.Mvc;
using System.Reflection;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Gateway_API.controllers
{
    [Route("")]
    [ApiController]
    public class LocalController(IConfiguration configuration) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;

        [HttpGet]
        public ContentResult Init()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Gateway_API.Templates.home.html"; // Thay bằng namespace của bạn

            string htmlContent;

            using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return new ContentResult
                    {
                        Content = "Error: home.html not found in resources.",
                        ContentType = "text/plain",
                        StatusCode = 500
                    };
                }

                using StreamReader reader = new StreamReader(stream);
                htmlContent = reader.ReadToEnd();
            }

            var services = _configuration.GetSection("Services").GetChildren();
            var statusHtml = "";

            foreach (var service in services)
            {
                string serviceName = service.Key;
                string? serviceUrl = service.Value;

                statusHtml += string.IsNullOrEmpty(serviceUrl)
                    ? $"<p>{serviceName} URL is not configured.</p>"
                    : $"<p>{serviceName}: <span data-url='{serviceUrl}' class='status'>Checking...</span></p>";
            }

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