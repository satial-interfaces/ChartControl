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
	double X { get; set; }
	/// <summary>The Y position.</summary>
	double Y { get; set; }
}

/// <summary>This class represents a chart point.</summary>
[PseudoClasses(":pressed", ":selected")]
public class ChartPoint : Ellipse, IChartPoint
{
	/// <summary>X position property.</summary>
	public static readonly DirectProperty<ChartPoint, double> XProperty = AvaloniaProperty.RegisterDirect<ChartPoint, double>(nameof(X), o => o.X, (o, v) => o.X = v);
	/// <summary>Y position property.</summary>
	public static readonly DirectProperty<ChartPoint, double> YProperty = AvaloniaProperty.RegisterDirect<ChartPoint, double>(nameof(Y), o => o.Y, (o, v) => o.Y = v);
	/// <summary>Is selected property.</summary>
	public bool IsSelected { get => isSelected; set { isSelected = value; PseudoClasses.Set(":selected", isSelected); } }
	/// <inheritdoc />
	public int Index { get; set; }
	/// <inheritdoc />
	public double X { get => x; set => SetAndRaise(XProperty, ref x, value); }
	/// <inheritdoc />
	public double Y { get => y; set => SetAndRaise(YProperty, ref y, value); }

	/// <summary>Is selected.</summary>
	bool isSelected;
	/// <summary>X position.</summary>
	double x;
	/// <summary>Y position.</summary>
	double y;
}