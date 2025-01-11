using Campaign.Infrastructure.Utils.MathHelper;

namespace Campaign.Infrastructure.Utils.Currency;

public static class CurrencyHelper
{
    public static decimal ApplyCurrency(this decimal value, decimal currencyRate)
    {
        return (value / currencyRate).Round();
    }
}