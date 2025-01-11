using System.ComponentModel.DataAnnotations;

namespace Campaign.Infrastructure.Utils.Validations;

public class ValidSegmentDateTimeAttribute : ValidationAttribute
{
	public override bool IsValid(object value)
	{
		DateTime departureDate = Convert.ToDateTime(value);
		if (departureDate.Date > DateTime.Now.AddYears(1).Date || departureDate.Date < DateTime.Now.Date)
		{
			return false;
		}

		return true;
	}
}