using System.ComponentModel;
using System.Data;

namespace Campaign.Infrastructure.Utils.Export;

public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(this List<T> iList)
    {
        DataTable dataTable = new DataTable();
        PropertyDescriptorCollection propertyDescriptorCollection =
            TypeDescriptor.GetProperties(typeof(T));
        for (int i = 0; i < propertyDescriptorCollection.Count; i++)
        {
            PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
            Type type = propertyDescriptor.PropertyType;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            dataTable.Columns.Add(propertyDescriptor.Name, typeof(string));
        }
        object[] values = new object[propertyDescriptorCollection.Count];
        foreach (T iListItem in iList)
        {
            for (int i = 0; i < values.Length; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];

                //if (propertyDescriptor.PropertyType.IsGenericType &&
                //    propertyDescriptor.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                //    propertyDescriptor.PropertyType.GenericTypeArguments[0].IsEnum)
                //{
                //    values[i] = propertyDescriptor.GetValue(iListItem)?.ToString();
                //}
                //else if (propertyDescriptor.PropertyType.IsEnum)
                //{
                //    values[i] = propertyDescriptor.GetValue(iListItem)?.ToString();
                //}
                //else
                //{
                //    values[i] = propertyDescriptor.GetValue(iListItem);
                //}

                try
                {
                    var value = propertyDescriptor.GetValue(iListItem);
                    if (value != null)
                        values[i] = value;
                    else
                        values[i] = "-";
                }
                catch
                {
                    values[i] = "-";
                }
            }
            dataTable.Rows.Add(values);
        }
        return dataTable;
    }
}