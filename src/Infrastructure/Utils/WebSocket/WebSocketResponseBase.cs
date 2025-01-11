namespace Campaign.Infrastructure.Utils.WebSocket;

public abstract class WebSocketResponseBase<T> where T : class
{
    public virtual List<T> Proposals { get; set; }
    public bool Completed { get; set; }
    public Guid RequestId { get; set; }
}