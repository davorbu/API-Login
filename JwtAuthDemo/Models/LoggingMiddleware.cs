using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace JwtAuthDemo.Models
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Provjeravamo da li je to POST zahtjev
            if (context.Request.Method == HttpMethods.Post)
            {
                // Omogućavamo buffering kako bismo mogli ponovo čitati stream
                context.Request.EnableBuffering();

                // Čitamo stream iz body-a
                var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
                await context.Request.Body.ReadAsync(buffer.AsMemory(0, buffer.Length));
                var bodyText = Encoding.UTF8.GetString(buffer);
                context.Request.Body.Seek(0, SeekOrigin.Begin); // Resetujemo stream na početak

                // Logiramo URL i body
                _logger.LogInformation($"URL: {context.Request.Path}, Body: {bodyText}");
            }

            // Nastavljamo sa sljedećim middleware-om u pipeline-u
            await _next(context);
        }
    }
}
