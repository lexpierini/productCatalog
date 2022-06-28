using Microsoft.AspNetCore.Mvc.Filters;

namespace productCatalog.Filter
{
    public class ApiLoggingFilter : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;
        public ApiLoggingFilter(ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        // Executes before action
        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("### Executing -> OnActionExecuting");
            _logger.LogInformation("###################################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");
            _logger.LogInformation("###################################################");
        }

        // Executes after action
        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("### Executing -> OnActionExecuting");
            _logger.LogInformation("###################################################");
            _logger.LogInformation($"{DateTime.Now.ToLongTimeString()}");
            _logger.LogInformation("###################################################");
        }

    }
}
