using System.Net;
using Campaign.Infrastructure.Utils.Serializers;
using Domain;
using Extensions.Http.Mvc;
using Refit;

namespace Campaign.Infrastructure.Utils.ErrorHandling;

public class ApiErrorReceivedException : Exception
{
	public ApiErrorReceivedException(ApiException badRequest)
	{
		if (badRequest.StatusCode == HttpStatusCode.BadRequest || badRequest.StatusCode == HttpStatusCode.NotFound)
		{
			try
			{
				var tryDeserializeJson = badRequest.Content.TryDeserializeJson<Envelop>(out var apiErrorResponse);
				if (tryDeserializeJson)
				{
					var errorMessage = apiErrorResponse.Meta.ErrorMessage ??
					                   string.Join(",", apiErrorResponse.Meta.Errors.SelectMany(a => a.Errors));
					throw new DomainValidationException(errorMessage);
				}
				else
				{
					throw new DomainValidationException(badRequest.Content);
				}
			}
			catch (DomainValidationException)
			{
				throw;
			}
			catch (Exception)
			{
				throw new DomainValidationException(badRequest.Content);
			}
		}
		else
		{
			throw new Exception(badRequest.Content);
		}
	}
}