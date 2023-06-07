using Microsoft.AspNetCore.Mvc.Filters;

namespace JourneyDrawing.Front.Infrastructure
{
    /// <summary>
    /// Global Log Action Filter to log begin and End
    /// </summary>
    public class GlobalLogActionFilter :IAsyncActionFilter
    {
        private readonly ILogger<GlobalLogActionFilter> _logger;
        
        public GlobalLogActionFilter(ILogger<GlobalLogActionFilter> logger)
        {
            _logger = logger;             
        }

        /******************************************************************/
        /// <summary>
        /// OnActionExecutionAsync
        /// </summary>
        /// <param name="context">ActionExecutingContext</param>
        /// <param name="next">ActionExecutionDelegate</param>
        /// <returns>Task</returns>
       /******************************************************************/
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation($"WebApp backend:Entering Request {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} routed to {context.Controller.GetType().Name} {context.ActionDescriptor.ToString()} ");
            // execute any code before the action executes
            var result = await next();
            // execute any code after the action executes
            _logger.LogInformation($"WebApp backend:Exiting Request {context.HttpContext.Request.Method} {context.HttpContext.Request.Path} routed to {context.Controller.GetType().Name}  {context.ActionDescriptor.ToString()} ");
        }
    }
}
