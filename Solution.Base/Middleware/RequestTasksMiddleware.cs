using Microsoft.AspNetCore.Http;
using Solution.Base.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Middleware
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
            catch(Exception ex)
            {
                _taskRunner.RunTasksOnError();
                throw;
            }

            _taskRunner.RunTasksAfterEachRequest();
        }
    }
}
