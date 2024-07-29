using System.Text;

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
        LogRequest(context);
        await _next(context);
        LogResponse(context);
    }

    private void LogRequest(HttpContext context)
    {
        var request = context.Request;
        _logger.LogInformation("HTTP {Method} {Path} мы получили следующий запрос.", request.Method, request.Path);
        if (request.Headers.ContainsKey("Authorization"))
        {
            var authHeader = request.Headers["Authorization"].ToString();
            _logger.LogInformation("Токен с которым прошло удаление {AuthHeader}", authHeader);
        }
        if (request.ContentLength > 0 && request.Body.CanSeek)
        {
            long originalPosition = request.Body.Position;
            request.Body.Position = 0;
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, leaveOpen: true))
            {
                var body = reader.ReadToEnd();
                _logger.LogInformation("Тело запроса: {Body}", body);
                request.Body.Position = originalPosition;
            }
        }
    }

    private void LogResponse(HttpContext context)
    {
        var response = context.Response;
        _logger.LogInformation("HTTP {Method} {Path} ответ выполнен со статусом {StatusCode}", context.Request.Method, context.Request.Path, response.StatusCode);
    }
}