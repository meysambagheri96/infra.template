namespace Campaign.Infrastructure.Utils.Pipeline;

public class Pipeline<T> where T : class
{
    private readonly List<IFilter<T>> _filters = new();

    public Pipeline<T> AddFilter(IFilter<T> filter)
    {
        Guard.NotNull(filter, nameof(filter));

        _filters.Add(filter);

        return this;
    }

    public virtual async Task Process(T context)
    {
        foreach (var filter in _filters)
        {
            await filter.Apply(context);
        }
    }
}