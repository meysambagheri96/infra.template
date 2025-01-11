using System.Net;
using Extensions.Http.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Campaign.Infrastructure.Utils.Serializers.Helpers;

/// <summary>
/// Output formatter
/// </summary>
public class SystemTextJsonResultFilterAttribute : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            objectResult.Formatters.Insert(0, new SystemTextJsonOutputFormatter(CustomJsonSerializer.SystemTextJsonOptions));
            
            objectResult.Value = new ApiResult(
                (HttpStatusCode)objectResult.StatusCode,
                objectResult.Value);
        }

        base.OnResultExecuting(context);
    }
}