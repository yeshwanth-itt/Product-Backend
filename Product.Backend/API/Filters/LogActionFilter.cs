using Microsoft.AspNetCore.Mvc.Filters;

namespace Product.Backend.API.Filters
{
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;

        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;
            _logger.LogInformation("Starting execution: {Controller}.{Action}", controller, action);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controller = context.Controller.GetType().Name;
            var action = context.ActionDescriptor.DisplayName;

            if (context.Exception == null)
            {
                _logger.LogInformation("Finished execution: {Controller}.{Action} - Success", controller, action);
            }
            else
            {
                _logger.LogError(context.Exception, "Finished execution: {Controller}.{Action} - Failed with error: {ErrorMessage}", controller, action, context.Exception.Message);
            }
        }
    }
}
