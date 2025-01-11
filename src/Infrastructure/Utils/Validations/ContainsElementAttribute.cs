using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Campaign.Infrastructure.Utils.Validations;

public class ContainsElementAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is IList list)
        {
            return list.Count > 0;
        }
        return false;
    }
}