using System.Globalization;

namespace GpsGeoFence.Converters;

// ══════════════════════════════════════════════════════════
// BOOL → STRING
// Usage: Converter={StaticResource BoolToStringConverter}
//        ConverterParameter="TrueText|FalseText"
// ══════════════════════════════════════════════════════════
public class BoolToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        var parts = (parameter as string ?? "|").Split('|');
        var trueText  = parts.Length > 0 ? parts[0] : "True";
        var falseText = parts.Length > 1 ? parts[1] : "False";
        return value is true ? trueText : falseText;
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// ══════════════════════════════════════════════════════════
// BOOL → COLOR
// Usage: Converter={StaticResource BoolToColorConverter}
//        ConverterParameter="#TrueColor|#FalseColor"
// ══════════════════════════════════════════════════════════
public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        var parts      = (parameter as string ?? "#000000|#000000").Split('|');
        var trueColor  = parts.Length > 0 ? parts[0] : "#000000";
        var falseColor = parts.Length > 1 ? parts[1] : "#000000";
        return Color.FromArgb(value is true ? trueColor : falseColor);
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// ══════════════════════════════════════════════════════════
// INVERSE BOOL
// Usage: Converter={StaticResource InverseBoolConverter}
// ══════════════════════════════════════════════════════════
public class InverseBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => value is not true;

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => value is not true;
}

// ══════════════════════════════════════════════════════════
// STRING → BOOL (non-empty = true)
// Usage: Converter={StaticResource StringToBoolConverter}
// ══════════════════════════════════════════════════════════
public class StringToBoolConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => !string.IsNullOrWhiteSpace(value as string);

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// ══════════════════════════════════════════════════════════
// DOUBLE → DISTANCE TEXT
// Usage: Converter={StaticResource DistanceConverter}
// -1 → "--"  |  < 1000 → "250 m"  |  >= 1000 → "1.2 km"
// ══════════════════════════════════════════════════════════
public class DistanceConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is not double d || d < 0) return "--";
        return d >= 1000
            ? $"{d / 1000:F1} km"
            : $"{d:F0} m";
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// ══════════════════════════════════════════════════════════
// INT → PRIORITY TEXT
// Usage: Converter={StaticResource PriorityConverter}
// 1 → "⭐ Cao"  |  2 → "Trung bình"  |  else → "Thấp"
// ══════════════════════════════════════════════════════════
public class PriorityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => value switch
        {
            1 => "⭐ Cao",
            2 => "Trung bình",
            _ => "Thấp"
        };

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

// ══════════════════════════════════════════════════════════
// INT SECONDS → TIME STRING
// Usage: Converter={StaticResource SecondsToTimeConverter}
// 90 → "1:30"  |  3600 → "1:00:00"
// ══════════════════════════════════════════════════════════
public class SecondsToTimeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
    {
        if (value is not int secs || secs < 0) return "0:00";
        var ts = TimeSpan.FromSeconds(secs);
        return ts.TotalHours >= 1
            ? ts.ToString(@"h\:mm\:ss")
            : ts.ToString(@"m\:ss");
    }

    public object ConvertBack(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
