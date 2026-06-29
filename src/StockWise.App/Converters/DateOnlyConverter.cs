using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace StockWise.App.Converters;

public class DateOnlyToDateTimeOffsetConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateOnly dateOnly)
            return new DateTimeOffset(dateOnly.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dto)
            return DateOnly.FromDateTime(dto.DateTime);
        return null;
    }
}
