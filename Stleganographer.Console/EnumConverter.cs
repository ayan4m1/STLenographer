using System.ComponentModel;
using System.Globalization;

namespace Stleganographer.Console
{
    public class EnumConverter<T> : TypeConverter where T : Enum
    {
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            Enum.TryParse(typeof(T), value as string, true, out var result);

            return result;
        }
    }
}
