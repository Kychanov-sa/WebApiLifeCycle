using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiLifeCycle.Filters
{
  /// <summary>
  /// Фильтр для обработки запросов к экспериментальному API.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class ExperimentalApi : Attribute, IResourceFilter
  {
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

      context.HttpContext.Response.Headers.Warning = $"199 - \"API {context.HttpContext.Request.Path} is experimental\"";
      
      if (!allowRequestExecute)
      {
        context.Result = new BadRequestObjectResult(new
        {
          Message = $"API {context.HttpContext.Request.Path} is experimental. You should add X-Allow-Experimental-Api header to use it.",
        });
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
      string? allowedExperimentalApis = request.Headers["X-Allow-Experimental-Api"];
      if (!String.IsNullOrEmpty(allowedExperimentalApis))
      {
        if (allowedExperimentalApis == "*")
          return true;

        if (allowedExperimentalApis.Split(' ').Contains(request.Path.Value, StringComparer.OrdinalIgnoreCase))
          return true;
      }

      return false;
    }
  }
}
