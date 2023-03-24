using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Shapes;
using Avalonia.LogicalTree;

namespace SatialInterfaces.Controls.Chart;

/// <summary>This interface represents a chart point.</summary>
public interface IChartPoint : ILogical
{
	/// <summary>Index property.</summary>
	int Index { get; set; }
	/// <summary>The X position.</summary>
	object? X { get; set; }
	/// <summary>The Y position.</summary>
	object? Y { get; set; }
	/// <summary>
	/// Converts the X-value to a double.
	/// </summary>
	/// <returns>The double or NaN otherwise.</returns>
	double XToDouble();
	/// <summary>
	/// Converts the Y-value to a double.
	/// </summary>
	/// <returns>The double or NaN otherwise.</returns>
	double YToDouble();
}

/// <summary>This class represents a chart point.</summary>
[PseudoClasses(":pressed", ":selected")]
public class ChartPoint : Ellipse, IChartPoint
{
	/// <summary>X position property.</summary>
	public static readonly DirectProperty<ChartPoint, object?> XProperty = AvaloniaProperty.RegisterDirect<ChartPoint, object?>(nameof(X), o => o.X, (o, v) => o.X = v);
	/// <summary>Y position property.</summary>
	public static readonly DirectProperty<ChartPoint, object?> YProperty = AvaloniaProperty.RegisterDirect<ChartPoint, object?>(nameof(Y), o => o.Y, (o, v) => o.Y = v);
	/// <summary>Is selected property.</summary>
	public bool IsSelected { get => isSelected; set { isSelected = value; PseudoClasses.Set(":selected", isSelected); } }
	/// <inheritdoc />
	public int Index { get; set; }
	/// <inheritdoc />
	public object? X
	{
		get => x; set
		{
			SetAndRaise(XProperty, ref x, value);
			cachedX = new double?();
		}
	}
	/// <inheritdoc />
	public object? Y
	{
		get => y; set
		{
			SetAndRaise(YProperty, ref y, value);
			cachedY = new double?();
		}
	}

	/// <summary>Is selected.</summary>
	bool isSelected;
	/// <summary>X position.</summary>
	object? x;
	/// <summary>Y position.</summary>
	object? y;

	/// <inheritdoc />
	public double XToDouble()
	{
		cachedX ??= ToDouble(x);
		return cachedX.Value;
	}

	/// <inheritdoc />
	public double YToDouble()
	{
		cachedY ??= ToDouble(y);
		return cachedY.Value;
	}

	/// <summary>
	/// Converts the given object to a double.
	/// </summary>
	/// <param name="value">Value to convert.</param>
	/// <returns>The double or NaN otherwise.</returns>
	static double ToDouble(object? value)
	{
		switch (value)
		{
			case null:
				return double.NaN;
			// Most common types
			case double d:
				return d;
			case float f:
				return f;
			case int i:
				return i;
			case long ul:
				return ul;
			case DateTime dt:
				return dt.Ticks;
			case byte b:
				return b;
			case sbyte sb:
				return sb;
			case char c:
				return c;
			case short s:
				return s;
			case ushort us:
				return us;
			case uint ui:
				return ui;
		}

		// Use the more expensive Converters
		var targetType = typeof(double);
		var converter = TypeDescriptor.GetConverter(targetType);
		try
		{
			if (converter.CanConvertFrom(value.GetType()))
				return (double)converter.ConvertFrom(value);
			return double.NaN;
		}
		catch (ArgumentException)
		{
			return double.NaN;
		}
		catch (NotSupportedException)
		{
			return double.NaN;
		}
	}

	/// <summary>
	/// Cached x value.
	/// </summary>
	double? cachedX;
	/// <summary>
	/// Cached y value.
	/// </summary>
	double? cachedY;
}