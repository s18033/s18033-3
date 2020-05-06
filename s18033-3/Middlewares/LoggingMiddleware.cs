using Microsoft.AspNetCore.Http;
using s18033_3.Services;
using System.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace s18033_3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IStudentsDbService service)
        {
            httpContext.Request.EnableBuffering();

            if (httpContext.Request != null)
            {
                string path = httpContext.Request.Path;
                string method = httpContext.Request.Method;
                string queryString = httpContext.Request.QueryString.ToString();

                string bodyString = "";

                using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyString = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;

                    using (StreamWriter data = File.AppendText("./requestsLog.txt"))
                    {
                        data.WriteLine("===");
                        data.WriteLine(path + " " + method);
                        data.WriteLine(queryString);
                        data.WriteLine(bodyString);
                        data.WriteLine("===");
                        data.Close();
                    }
                }
            }

            await _next(httpContext);
        }
    }
}
