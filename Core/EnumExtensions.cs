using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Core;

public static class EnumExtensions
{
    public static string GetDisplayName<T>(this T Enum) where T : Enum
        => Enum.GetEnumAttribute<T, DisplayAttribute>()?.Name;

    public static TAttribute GetEnumAttribute<TEnum, TAttribute>(this TEnum Enum)
        where TEnum : Enum
        where TAttribute : Attribute
    {
        var MemberInfo = typeof(TEnum).GetMember(Enum.ToString());
        return MemberInfo[0].GetCustomAttribute<TAttribute>();
    }
}