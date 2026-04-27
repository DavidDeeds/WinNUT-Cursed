using System.Reflection;

namespace WinNUT_Client_Common;

[AttributeUsage(AttributeTargets.Field)]
public sealed class StringValueAttribute : Attribute
{
    public string Value { get; }

    public StringValueAttribute(string value) => Value = value;
}

public static class StringEnum
{
    public static string? GetStringValue(Enum value)
    {
        var type = value.GetType();
        var fi = type.GetField(value.ToString()!);
        if (fi == null)
        {
            return null;
        }

        var attrs = fi.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
        return attrs is { Length: > 0 } ? attrs[0].Value : null;
    }
}
