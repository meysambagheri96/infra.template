using System.Net.WebSockets;
using System.Text;
using Campaign.Infrastructure.Utils.Serializers;

namespace Campaign.Infrastructure.Utils.WebSocket;

public class WebSocket
{
    private bool _isCompleted = false;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WebSocket> _logger;

    public WebSocket(ILogger<WebSocket> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<List<TResult>> ReceiveAvailableResponse<TResult, TWebSocketResult>(Guid requestId,
        Task socketRequest, CancellationToken cancellationToken)
        where TResult : class
        where TWebSocketResult : WebSocketResponseBase<TResult>
    {
        List<TResult> resultData = new();
        try
        {
            var baseAddress = _configuration["Urls:LiveBaseAddress"];
            Guard.NotNullOrEmpty(baseAddress, nameof(baseAddress));

            var socket = new ClientWebSocket();
            await socket.ConnectAsync(
                new Uri($"{baseAddress}/?userId={requestId.ToString().ToLower()}"), cancellationToken);
            _logger.LogWarning($"Socket opened for requestId:{requestId}");

            SendHeartBeat(socket, requestId, cancellationToken);
            var buffer = System.Net.WebSockets.WebSocket.CreateClientBuffer(4 * 1024, 4 * 1024);

            // send available request to FlightCatalog
            //await _flightCatalogClient.Available(query);
            await socketRequest;

            while (socket.State == WebSocketState.Open)
            {
                if (_isCompleted)
                    return resultData;

                if (cancellationToken.IsCancellationRequested)
                    return resultData;

                var stream = new MemoryStream();

                WebSocketReceiveResult result = null;
                
                do
                {
                    result = await socket.ReceiveAsync(buffer, cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        stream.Write(buffer.Array, 0, result.Count);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }
                } while (!result.EndOfMessage && !cancellationToken.IsCancellationRequested);
            
                // parse message
                var readJson = Encoding.UTF8.GetString(stream.ToArray());

                if (!string.IsNullOrWhiteSpace(readJson))
                {
                    var webSocketResponse = DeserializeWebSocketResponse<TResult, TWebSocketResult>(readJson);
                    if (webSocketResponse != null)
                    {
                        if (webSocketResponse is { Completed: true })
                            this._isCompleted = true;

                        if (webSocketResponse.Proposals?.Count > 0)
                            resultData.AddRange(webSocketResponse.Proposals);

                        if (webSocketResponse.Completed)
                        {
                            await CloseWebSocket(socket, WebSocketCloseStatus.NormalClosure, requestId, cancellationToken);
                            return resultData;
                        }
                    }
                }
            }

            return resultData;
        }
        catch (TaskCanceledException)
        {
            // available not completed (NormalClosure)
            return resultData;
        }
        catch (Exception ex)
        {
            _logger.LogError("Logger:{Logger}, Message:{Message}", nameof(WebSocket), ex.ToString());
            throw;
        }
        finally
        {
            _logger.LogWarning($"Response count: {resultData.Count} for requestId:{requestId}");
            _isCompleted = true;
        }
    }

    private TWebSocketResult DeserializeWebSocketResponse<TResult, TWebSocketResult>(string readJson)
        where TResult : class
        where TWebSocketResult : WebSocketResponseBase<TResult>
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(readJson))
            {
                // when heart bit sent, socket respond this: "0 requestId"
                if (readJson.StartsWith("0"))
                    return null;

                var webSocketResponse = readJson.DeserializeJson<TWebSocketResult>();
                return webSocketResponse;
            }
            else
            {
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical($"Deserialization Error: SocketResponse:{readJson}, Exception:{ex}");
            return null;
        }
    }

    private async void SendHeartBeat(ClientWebSocket socket, Guid requestId, CancellationToken cts)
    {
        try
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(4));
            int i = 1;

            do
            {
                if (cts.IsCancellationRequested)
                    return;

                if (_isCompleted)
                    return;

                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(new byte[] { 0 }, WebSocketMessageType.Binary, true, default);
                    _logger.LogWarning($"Heartbeat {i} sent for requestId:{requestId}");
                    i++;
                }

            } while (!cts.IsCancellationRequested && await timer.WaitForNextTickAsync(cts));
        }
        catch (Exception)
        {
            await CloseWebSocket(socket, WebSocketCloseStatus.Empty, requestId, cts);
        }
    }

    private async Task CloseWebSocket(ClientWebSocket socket, WebSocketCloseStatus status, Guid requestId, CancellationToken cts)
    {
        try
        {
            if (socket.State != WebSocketState.Closed)
            {
                await socket.CloseAsync(status, string.Empty, cts);
                _logger.LogWarning($"Socket closed for requestId:{requestId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex.ToString());
        }
    }
}