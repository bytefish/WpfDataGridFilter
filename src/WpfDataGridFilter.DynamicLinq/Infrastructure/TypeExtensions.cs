using System.Reflection;

namespace WpfDataGridFilter.DynamicLinq.Infrastructure
{
    public class TypeExtensions
    {
        public static bool HasProperty(Type type, string propertyPath)
        {
            foreach (var prop in propertyPath.Split('.'))
            {
                PropertyInfo? property = type.GetProperty(prop);

                if (property == null)
                {
                    return false;
                }

                type = property.PropertyType;
            }

            return true;
        }
    }
}
