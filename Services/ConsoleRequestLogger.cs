using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SmartGearApp.Services
{
    public class ConsoleRequestLogger : IRequestLogger
    {
        public Task LogAsync(HttpRequest request)
        {
            Console.WriteLine($"[Request] {request.Method} {request.Path}");
            return Task.CompletedTask;
        }
    }
}
