using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiLifeCycle.Filters
{
  /// <summary>
  /// Фильтр для обработки запросов к устаревшему API.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class DeprecatedApi : Attribute, IResourceFilter
  {
    /// <summary>
    /// Дата в формате ISO 8601, с которой API считается устаревшим.
    /// </summary>
    public string? From { get; set; }

    /// <summary>
    /// Дата в формате ISO 8601, с которой API будет недоступен.
    /// </summary>
    public string? Sunset { get; set; }

    #region IResourceFilter

    /// <summary>
    /// <see cref="IResourceFilter.OnResourceExecuted(ResourceExecutedContext)"/>
    /// </summary>
    /// <param name="context"></param>
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
    }

    /// <summary>
    /// <see cref="IResourceFilter.OnResourceExecuting(ResourceExecutingContext)"/>
    /// </summary>
    /// <param name="context"></param>
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
      bool allowRequestExecute = IsRequestAllowed(context.HttpContext.Request);

      context.HttpContext.Response.Headers.Warning = $"299 - \"API {context.HttpContext.Request.Path} is deprecated.\"";

      if (DateTimeOffset.TryParse(From, out DateTimeOffset fromDate))
        context.HttpContext.Response.Headers["Deprecation"] = $"@{fromDate.ToUnixTimeSeconds()}";

      if (DateTimeOffset.TryParse(Sunset, out DateTimeOffset sunsetDate))
      {
        context.HttpContext.Response.Headers["Sunset"] = $"{sunsetDate.ToUniversalTime():r}";
        if (DateTimeOffset.Now > sunsetDate)
          allowRequestExecute = false;
      }

      if (!allowRequestExecute)
      {        
        context.Result = new JsonResult(new
        {
          Message = $"API {context.HttpContext.Request.Path} is deprecated. You should add X-Allow-Deprecated-Api header to use it.",
        });
        context.HttpContext.Response.StatusCode = 410;
      }
    }

    #endregion

    /// <summary>
    /// Возвращает признак доступности запроса к выполнению.
    /// </summary>
    /// <param name="request">HTTP запрос.</param>
    /// <returns>true - запрос можно выполнить, иначе false.</returns>
    protected static bool IsRequestAllowed(HttpRequest request)
    {
      string? allowedDeprecateApis = request.Headers["X-Allow-Deprecated-Api"];
      if (!String.IsNullOrEmpty(allowedDeprecateApis))
      {
        if (allowedDeprecateApis == "*")
          return true;

        if (allowedDeprecateApis.Split(' ').Contains(request.Path.Value, StringComparer.OrdinalIgnoreCase))
          return true;
      }

      return false;
    }
  }
}
