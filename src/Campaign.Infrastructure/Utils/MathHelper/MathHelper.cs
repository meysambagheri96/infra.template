namespace Campaign.Infrastructure.Utils.MathHelper;

public static class MathHelper
{
    public const int DECIAML_PLACE = 8;

    public static decimal Round(this decimal value)
    {
        return Math.Round(value, DECIAML_PLACE);
    }
		
    public static decimal RoundInternal(this decimal value, int place)
    {
        return Math.Round(value, place);
    }

    public static decimal Addition(params decimal[] values)
    {
        if (values.Length == 1)
            return values[0].Round();

        return
        (
            values.Take(1).First().Round()
            +
            Addition(values.Skip(1).ToArray()).Round()
        ).Round();
    }

    public static decimal Subtract(params decimal[] values)
    {
        if (values.Length == 1)
            return values[0].Round();

        return
        (
            values.Take(1).First().Round()
            -
            Subtract(values.Skip(1).ToArray()).Round()
        ).Round();
    }
}