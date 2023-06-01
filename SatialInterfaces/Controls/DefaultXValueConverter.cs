using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using SatialInterfaces.Helpers;

namespace SatialInterfaces.Controls.Chart;

public class DefaultXValueConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return BindingOperations.DoNothing;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (targetType != typeof(string) || value == null)
			return BindingOperations.DoNothing;

		return string.Format(CultureInfo.CurrentCulture, "{0:F3}", SystemHelper.GetValue(value, "X"));
	}
}

public class DefaultYValueConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return BindingOperations.DoNothing;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (targetType != typeof(string) || value == null)
			return BindingOperations.DoNothing;

		return string.Format(CultureInfo.CurrentCulture, "{0:F3}", SystemHelper.GetValue(value, "Y"));
	}
}