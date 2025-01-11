namespace Campaign.Infrastructure.Utils.Specification;

public abstract class Specification<T> where T : class
{
    public abstract bool Match(T input);
}
