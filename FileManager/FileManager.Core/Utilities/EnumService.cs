using System.ComponentModel;
using System.Reflection;

namespace FileManager.Core.Utilities;

public static class EnumService
{
    /// <summary>
    /// Получить атрибут "Описание"
    /// </summary>
    /// <param name="value">Значение перечисления</param>
    /// <returns>Значение атрибута "Описание"</returns>
    public static string GetDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes =
            (DescriptionAttribute[])fi.GetCustomAttributes(
            typeof(DescriptionAttribute),
            false);

        if (attributes != null &&
            attributes.Length > 0)
            return attributes[0].Description;
        else
            return value.ToString();
    }
}
