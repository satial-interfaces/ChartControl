using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Shapes;

namespace SatialInterfaces.Controls;

/// <summary>This interface represents a chart point.</summary>
public interface IChartPoint : IControl
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
	public object? X { get => x; set => SetAndRaise(XProperty, ref x, value); }
	/// <inheritdoc />
	public object? Y { get => y; set => SetAndRaise(YProperty, ref y, value); }

	/// <summary>Is selected.</summary>
	bool isSelected;
	/// <summary>X position.</summary>
	object? x;
	/// <summary>Y position.</summary>
	object? y;

	/// <inheritdoc />
	public double XToDouble() => ToDouble(x);

	/// <inheritdoc />
	public double YToDouble() => ToDouble(y);

	/// <summary>
	/// Converts the given object to a double.
	/// </summary>
	/// <param name="value">Value to convert.</param>
	/// <returns>The double or NaN otherwise.</returns>
	static double ToDouble(object? value)
	{
		if (value == null) return double.NaN;
		// Most common types
		if (value is double d) return d;
		if (value is float f) return f;
		if (value is int i) return i;
		if (value is long ul) return ul;
		if (value is DateTime dt) return dt.Ticks;

		if (value is byte b) return b;
		if (value is sbyte sb) return sb;
		if (value is char c) return c;
		if (value is short s) return s;
		if (value is ushort us) return us;
		if (value is uint ui) return ui;

		// Use the more expensive Converters
		var targetType = typeof(double);
		var converter = TypeDescriptor.GetConverter(targetType);
		try
		{
			if (converter.CanConvertFrom(value.GetType()))
				return (double)converter.ConvertFrom(value);
			else
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
}