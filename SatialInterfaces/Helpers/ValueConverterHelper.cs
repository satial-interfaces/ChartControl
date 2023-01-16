using System.Globalization;
using Avalonia.Data.Converters;

namespace SatialInterfaces.Helpers;

/// <summary>This class contains value converter methods.</summary>
internal static class ValueConverterHelper
{
	/// <summary>
	/// Converts the given value to formatted text.
	/// </summary>
	/// <param name="valueConverter">Value converter to use.</param>
	/// <param name="value">Value to convert.</param>
	/// <returns>The string.</returns>
	public static string ConvertValueToText(IValueConverter? valueConverter, object? value)
	{
		var result = valueConverter != null
			? valueConverter.ConvertBack(value, typeof(string), null, CultureInfo.CurrentCulture)?.ToString()
			: value?.ToString();
		return result ?? "";
	}
}