using System.Diagnostics;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Campaign.Infrastructure.Utils.NET.CustomHttpLogger;

public class CustomLoggingScopeHttpMessageHandler : DelegatingHandler
{
	private readonly ILogger _logger;

	public CustomLoggingScopeHttpMessageHandler(ILogger logger)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
		CancellationToken cancellationToken)
	{
		if (request == null)
		{
			throw new ArgumentNullException(nameof(request));
		}

		//Check header
		//    if (!request.Headers.Any(a =>
		//            a.Key == CustomLoggingConstants.CustomLog_Key &&
		//            a.Value.Contains(CustomLoggingConstants.CustomLog_Enable)))
		//    {
		//        return await base.SendAsync(request, cancellationToken);
		//    }

		using (Log.BeginRequestPipelineScope(_logger, request))
		{
			Log.RequestPipelineStart(_logger, request);
			var response = await base.SendAsync(request, cancellationToken);
			var content = await response.Content.ReadAsStringAsync(cancellationToken);
			Log.RequestPipelineEnd(_logger, response, content, request.RequestUri);

			return response;
		}
	}

	private static class Log
	{
		private static class EventIds
		{
			public static readonly EventId PipelineStart = new EventId(100, "RequestPipelineStart");
			public static readonly EventId PipelineEnd = new EventId(101, "RequestPipelineEnd");
		}

		private static readonly Func<ILogger, HttpMethod, Uri, string, IDisposable> _beginRequestPipelineScope =
			LoggerMessage.DefineScope<HttpMethod, Uri, string>(
				"HTTP {HttpMethod} {Uri} {CorrelationId}");

		private static readonly Action<ILogger, HttpMethod, Uri, string, Exception> _requestPipelineStart =
			LoggerMessage.Define<HttpMethod, Uri, string>(
				LogLevel.Information,
				EventIds.PipelineStart,
				"Start processing HTTP request {HttpMethod} {Uri} [Correlation: {CorrelationId}]");

		private static readonly Action<ILogger, HttpStatusCode, Exception> _requestPipelineEnd =
			LoggerMessage.Define<HttpStatusCode>(
				LogLevel.Information,
				EventIds.PipelineEnd,
				"End processing HTTP request - {StatusCode}");

		public static IDisposable BeginRequestPipelineScope(ILogger logger, HttpRequestMessage request)
		{
			var correlationId = GetCorrelationIdFromRequest(request);
			return _beginRequestPipelineScope(logger, request.Method, request.RequestUri, correlationId);
		}

		public static void RequestPipelineStart(ILogger logger, HttpRequestMessage request)
		{
			var requestContent = request.Content?.ReadAsStringAsync().Result;
			var message =
				$"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} INF] Sending HTTP Request, " +
				$"Uri:{request.RequestUri}";

			if (requestContent != null)
				message += $", \nRequest Body: {requestContent}";

			WriteMessage(message);

			var correlationId = GetCorrelationIdFromRequest(request);
			_requestPipelineStart(logger, request.Method, request.RequestUri, correlationId, null);
		}

		public static void RequestPipelineEnd(ILogger logger, HttpResponseMessage response, string content,
			Uri requestRequestUri)
		{
			var message =
				$"[{DateTime.Now:yyyy-MM-dd HH:mm:ss} INF] Received HTTP Response, " +
				$"Status:{(int)response.StatusCode}, " +
				$"Uri:{requestRequestUri}";

			if (content != null)
				message += $", \nResponse Body: {content}";

			WriteMessage(message);

			_requestPipelineEnd(logger, response.StatusCode, null);
		}

		private static string GetCorrelationIdFromRequest(HttpRequestMessage request)
		{
			var correlationId = "Not set";

			if (request.Headers.TryGetValues("X-Correlation-ID", out var values))
			{
				correlationId = values.First();
			}

			return correlationId;
		}

		private static void WriteMessage(string message)
		{
			if (Debugger.IsAttached)
				Debug.WriteLine(message);

			Console.WriteLine(message);
		}
	}
}