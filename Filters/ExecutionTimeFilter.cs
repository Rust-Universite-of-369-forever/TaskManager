using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TaskManager.Filters;

public class ExecutionTimeFilter : IActionFilter
{
    private readonly ILogger<ExecutionTimeFilter> _logger;
    private Stopwatch? _timer;

    public ExecutionTimeFilter(ILogger<ExecutionTimeFilter> logger)
        => _logger = logger;

    public void OnActionExecuting(ActionExecutingContext context)
        => _timer = Stopwatch.StartNew();

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _timer?.Stop();

        var login = context.HttpContext.Session.GetString("Login") ?? "Гость";
        _logger.LogInformation(
            "Действие {Action} пользователя {User} выполнено за {Time} мс",
            context.ActionDescriptor.DisplayName,
            login,
            _timer?.ElapsedMilliseconds);
    }
}
