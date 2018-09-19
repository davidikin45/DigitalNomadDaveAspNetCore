using DND.Common.Infrastructure.Tasks;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DND.Common.Middleware
{
    public class RequestTasksMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TaskRunner _taskRunner;

        public RequestTasksMiddleware(RequestDelegate next, TaskRunner taskRunner)
        {
            _next = next;
            _taskRunner = taskRunner;
        }

        public async Task Invoke(HttpContext context)
        {
            _taskRunner.RunTasksOnEachRequest();

            // Call the next delegate/middleware in the pipeline
            try
            {
                await this._next(context);
            }
            catch
            {
                _taskRunner.RunTasksOnError();
                throw;
            }

            _taskRunner.RunTasksAfterEachRequest();
        }
    }
}
