using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Campaign.Infrastructure.Utils.Export;

public static class ExcelExporter
{
    public const string MIM_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    [Obsolete]
    public static string ToCsv<T>(this List<T> list) where T : class
    {
        if (list == null || !list.Any())
            return string.Empty;

        var csvBuilder = new StringBuilder();
        var properties = typeof(T).GetProperties();
        var headers = properties.Select(p =>
        {
            var displayNameAttr = p.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                .FirstOrDefault() as DisplayNameAttribute;
            return displayNameAttr?.DisplayName ?? p.Name;
        });

        csvBuilder.AppendLine(string.Join(",", headers));

        foreach (var item in list)
        {
            var row = item.GetType().GetProperties().Select(c => c.GetValue(item)?.ToString());
            csvBuilder.AppendLine(string.Join(",", row));
        }

        return csvBuilder.ToString();
    }

    public static string ExportListToExcel<T>(this List<T> list, string defaultPath, string fileName)
    {
        var filePath = GetUniqueFilePath(Path.Combine(defaultPath, "reports", fileName));

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Data");

            var properties = typeof(T).GetProperties();

            // Write header row
            for (int i = 0; i < properties.Length; i++)
            {
                var headerValue = GetHeaderValue<T>(properties[i]);

                worksheet.Cell(1, i + 1).Value = headerValue;
            }

            // Write data rows
            for (int row = 0; row < list.Count; row++)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var cellValue = GetCellValue(list[row], properties[col], col);

                    worksheet.Cell(row + 2, col + 1).Value = cellValue;
                }
            }

            // Save to file
            workbook.SaveAs(filePath);
        }

        return filePath;
    }

    private static string GetHeaderValue<T>(PropertyInfo propertyInfo)
    {
        var displayNameAttr = propertyInfo.CustomAttributes.FirstOrDefault()?.NamedArguments?.FirstOrDefault();

        var headerValue = displayNameAttr != null
            ? displayNameAttr.Value.TypedValue.Value?.ToString()
            : propertyInfo.Name;
        return headerValue;
    }

    private static string GetCellValue<T>(T cell, PropertyInfo propertyInfo, int col)
    {
        var cellValue = propertyInfo.GetValue(cell)?.ToString() ?? string.Empty;


        //Check if the property is Enum, return displayName attribute if exist
        if (propertyInfo.PropertyType.IsEnum)
        {
            var value = propertyInfo.GetValue(cell);
            if (value != null && propertyInfo.PropertyType.IsEnum)
            {
                var displayName = GetEnumDisplayName(value);
                cellValue = !string.IsNullOrWhiteSpace(displayName) ? displayName : cellValue;
            }
        }


        //Check if the property is DateTime return persian date
        if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
        {
            var value = propertyInfo.GetValue(cell);

            if (value != null)
            {
                var date = (DateTime)value;
                var persianDate = ConvertToPersianDate(date);
                cellValue = persianDate;
            }
        }

        return cellValue;
    }

    private static string ConvertToPersianDate(DateTime date)
    {
        var persianCalendar = new PersianCalendar();
        return $"{persianCalendar.GetYear(date):0000}/{persianCalendar.GetMonth(date):00}/{persianCalendar.GetDayOfMonth(date):00} {date:HH:mm:ss}";
    }

    private static string GetUniqueFilePath(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath) ?? ".";
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);

        int counter = 1;
        string uniqueFilePath = filePath;

        while (File.Exists(uniqueFilePath))
        {
            uniqueFilePath = Path.Combine(directory, $"{fileNameWithoutExtension} ({counter}){extension}");
            counter++;
        }

        return uniqueFilePath;
    }

    private static string GetEnumDisplayName(object enumValue)
    {
        var type = enumValue.GetType();
        if (!type.IsEnum) return enumValue.ToString();

        var memberInfo = type.GetMember(enumValue.ToString() ?? string.Empty);
        if (memberInfo.Length > 0)
        {
            var displayNameAttr = memberInfo[0].CustomAttributes.FirstOrDefault()?.NamedArguments?.FirstOrDefault();

            return displayNameAttr?.TypedValue.Value?.ToString();
        }

        return enumValue.ToString();
    }
}