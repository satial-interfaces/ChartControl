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
}