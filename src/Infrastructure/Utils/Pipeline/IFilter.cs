namespace Campaign.Infrastructure.Utils.Pipeline;

public interface IFilter<in T> where T : class

{
    Task Apply(T context);
}