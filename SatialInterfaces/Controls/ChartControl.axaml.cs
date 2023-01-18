using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using SatialInterfaces.Helpers;

namespace SatialInterfaces.Controls;

internal class MarginConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		value is double d ? new Thickness(0.0, 0.0, 0.0, d) : BindingOperations.DoNothing;

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
		BindingOperations.DoNothing;
}

internal class VerticalControlMarginConverter : IMultiValueConverter
{
	public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
	{
		if (values.Count < 6 ||
		values[0] is not double fontSize ||
		values[1] is not string text ||
		values[2] is not TextBlock textBlock ||
		values[3] is not double parentWidth ||
		values[4] is not double parentHeight ||
		values[5] is not Thickness parentMargin)
		{
			return BindingOperations.DoNothing;
		}
		// textBlock.Measure(Size.Infinity);
		var d = 0.0 - fontSize / 2.0 + parentWidth / 2.0;
		// d = d + ((parentHeight - parentMargin.Bottom - textBlock.DesiredSize.Width) / 2);
		return new Thickness(0.0, 0.0, 0.0, d);
	}
}

/// <summary>This class represents a chart control (line with markers).</summary>
public class ChartControl : ContentControl, IStyleable
{
	/// <summary>Items property</summary>
	public static readonly DirectProperty<ChartControl, IEnumerable> ItemsProperty = AvaloniaProperty.RegisterDirect<ChartControl, IEnumerable>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
	/// <summary>Item template property.</summary>
	public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<ChartControl, IDataTemplate?>(nameof(ItemTemplate));
	/// <summary>Grid stroke property.</summary>
	public static readonly StyledProperty<IBrush?> GridStrokeProperty = AvaloniaProperty.Register<ChartControl, IBrush?>(nameof(GridStroke));
	/// <summary>Grid stroke thickness property.</summary>
	public static readonly StyledProperty<double> GridStrokeThicknessProperty = AvaloniaProperty.Register<ChartControl, double>(nameof(GridStrokeThickness));
	/// <summary>Line stroke property.</summary>
	public static readonly StyledProperty<IBrush?> LineStrokeProperty = AvaloniaProperty.Register<ChartControl, IBrush?>(nameof(LineStroke));
	/// <summary>Line stroke thickness property.</summary>
	public static readonly StyledProperty<double> LineStrokeThicknessProperty = AvaloniaProperty.Register<ChartControl, double>(nameof(LineStrokeThickness));
	/// <summary>The selected index property</summary>
	public static readonly StyledProperty<int> SelectedIndexProperty = AvaloniaProperty.Register<ChartControl, int>(nameof(SelectedIndex), -1);
	/// <summary>The selected item property</summary>
	public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<ChartControl, object?>(nameof(SelectedItem));
	/// <summary>X-axis title property.</summary>
	public static readonly StyledProperty<string> XAxisTitleProperty = AvaloniaProperty.Register<ChartControl, string>(nameof(XAxisTitle), string.Empty);
	/// <summary>Y-values maximum property.</summary>
	public static readonly StyledProperty<object?> XMaximumProperty = AvaloniaProperty.Register<ChartControl, object?>(nameof(XMaximum));
	/// <summary>Y-values minimum property.</summary>
	public static readonly StyledProperty<object?> XMinimumProperty = AvaloniaProperty.Register<ChartControl, object?>(nameof(XMinimum));
	/// <summary>X-value converter property.</summary>
	public static readonly DirectProperty<ChartControl, IValueConverter?> XValueConverterProperty = AvaloniaProperty.RegisterDirect<ChartControl, IValueConverter?>(nameof(XValueConverter), c => c.XValueConverter, (o, v) => o.XValueConverter = v);
	/// <summary>Y-axis title property.</summary>
	public static readonly StyledProperty<string> YAxisTitleProperty = AvaloniaProperty.Register<ChartControl, string>(nameof(YAxisTitle), string.Empty);
	/// <summary>Y-values maximum property.</summary>
	public static readonly StyledProperty<object?> YMaximumProperty = AvaloniaProperty.Register<ChartControl, object?>(nameof(YMaximum));
	/// <summary>Y-values minimum property.</summary>
	public static readonly StyledProperty<object?> YMinimumProperty = AvaloniaProperty.Register<ChartControl, object?>(nameof(YMinimum));
	/// <summary>Y-value converter property.</summary>
	public static readonly DirectProperty<ChartControl, IValueConverter?> YValueConverterProperty = AvaloniaProperty.RegisterDirect<ChartControl, IValueConverter?>(nameof(YValueConverter), c => c.YValueConverter, (o, v) => o.YValueConverter = v);
	/// <summary>The selection changed event</summary>
	public static readonly RoutedEvent<ChartSelectionChangedEventArgs> SelectionChangedEvent = RoutedEvent.Register<ChartControl, ChartSelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

	/// <summary>Items property.</summary>
	public IEnumerable Items { get => items; set => SetAndRaise(ItemsProperty, ref items, value); }
	/// <summary>Item template property.</summary>
	public IDataTemplate? ItemTemplate { get => GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }
	/// <summary>Grid stroke property.</summary>
	public IBrush? GridStroke { get => GetValue(GridStrokeProperty); set => SetValue(GridStrokeProperty, value); }
	/// <summary>Grid stroke thickness.</summary>
	public double GridStrokeThickness { get => GetValue(GridStrokeThicknessProperty); set => SetValue(GridStrokeThicknessProperty, value); }
	/// <summary>Line stroke property.</summary>
	public IBrush? LineStroke { get => GetValue(LineStrokeProperty); set => SetValue(LineStrokeProperty, value); }
	/// <summary>Line stroke thickness.</summary>
	public double LineStrokeThickness { get => GetValue(LineStrokeThicknessProperty); set => SetValue(LineStrokeThicknessProperty, value); }
	/// <summary>Selected index</summary>
	public int SelectedIndex { get => GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
	/// <summary>Selected item</summary>
	public object? SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }
	/// <summary>X-axis property.</summary>
	public string XAxisTitle { get => GetValue(XAxisTitleProperty); set => SetValue(XAxisTitleProperty, value); }
	/// <summary>X-values maximum property.</summary>
	public object? XMaximum { get => GetValue(XMaximumProperty); private set => SetValue(XMaximumProperty, value); }
	/// <summary>X-values maximum property.</summary>
	public object? XMinimum { get => GetValue(XMinimumProperty); private set => SetValue(XMinimumProperty, value); }
	/// <summary>X-value converter property.</summary>
	public IValueConverter? XValueConverter { get => xValueConverter; set => SetAndRaise(XValueConverterProperty, ref xValueConverter, value); }
	/// <summary>Y-axis property.</summary>
	public string YAxisTitle { get => GetValue(YAxisTitleProperty); set => SetValue(YAxisTitleProperty, value); }
	/// <summary>Y-values maximum property.</summary>
	public object? YMaximum { get => GetValue(YMaximumProperty); private set => SetValue(YMaximumProperty, value); }
	/// <summary>Y-values maximum property.</summary>
	public object? YMinimum { get => GetValue(YMinimumProperty); private set => SetValue(YMinimumProperty, value); }
	/// <summary>Y-value converter property.</summary>
	public IValueConverter? YValueConverter { get => yValueConverter; set => SetAndRaise(YValueConverterProperty, ref yValueConverter, value); }
	/// <summary>Occurs when selection changed</summary>
	public event EventHandler<ChartSelectionChangedEventArgs> SelectionChanged { add => AddHandler(SelectionChangedEvent, value); remove => RemoveHandler(SelectionChangedEvent, value); }

	/// <summary>Initializes static members of the <see cref="ChartControl" /> class</summary>
	static ChartControl()
	{
		FocusableProperty.OverrideDefaultValue<ChartControl>(true);

		ItemsProperty.Changed.AddClassHandler<ChartControl>((x, e) => x.ItemsChanged(e));
		SelectedIndexProperty.Changed.AddClassHandler<ChartControl>((x, e) => x.SelectedIndexChanged(e));
		SelectedItemProperty.Changed.AddClassHandler<ChartControl>((x, e) => x.SelectedItemChanged(e));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ChartControl" /> class.
	/// </summary>
	public ChartControl()
	{
		AvaloniaXamlLoader.Load(this);
		canvas = this.FindControl<Canvas>("Canvas");
		polyline = this.FindControl<Polyline>("Polyline");
		xMinimumTextBlock = this.FindControl<TextBlock>("XMinimumTextBlock");
		xMaximumTextBlock = this.FindControl<TextBlock>("XMaximumTextBlock");
		yMinimumTextBlock = this.FindControl<TextBlock>("YMinimumTextBlock");
		yMaximumTextBlock = this.FindControl<TextBlock>("YMaximumTextBlock");

		canvas?.GetObservable(BoundsProperty).Subscribe(OnCanvasBoundsChanged);
	}

	/// <inheritdoc />
	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		leftButtonDown = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;
		base.OnPointerPressed(e);
	}

	/// <inheritdoc />
	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		if (!Items.Any() || !leftButtonDown || e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			leftButtonDown = false;
			base.OnPointerReleased(e);
			return;
		}

		var control = e.Pointer.Captured as ILogical;
		var chartPoint = control as ChartPoint;
		var index = chartPoint?.Index ?? -1;
		leftButtonDown = false;
		base.OnPointerReleased(e);
		var previousIndex = SelectedIndex;
		SelectedIndex = index;
		// Force re-trigger
		if (index == previousIndex)
			RaiseSelectionChanged(index);
	}

	/// <inheritdoc />
	protected override void OnKeyDown(KeyEventArgs e)
	{
		switch (e.Key)
		{
			case Key.Up or Key.Left:
				SelectNext(-1);
				e.Handled = true;
				break;
			case Key.Down or Key.Right:
				SelectNext(1);
				e.Handled = true;
				break;
		}

		base.OnKeyDown(e);
	}

	/// <summary>
	/// Items changed event.
	/// </summary>
	/// <param name="e">Argument for the event.</param>
	void ItemsChanged(AvaloniaPropertyChangedEventArgs e) => UpdateItems();

	/// <summary>
	/// Selected index changed event.
	/// </summary>
	/// <param name="e">Argument for the event.</param>
	void SelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (skipSelectedIndexChanged || e.NewValue is not int value) return;
		SetSelection(value);
		RaiseSelectionChanged(value);
	}

	/// <summary>
	/// Selected item changed event.
	/// </summary>
	/// <param name="e">Argument for the event.</param>
	void SelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (skipSelectedItemChanged || e.NewValue is not { } value) return;
		var index = GetItemsAsList().IndexOf(value);
		SetSelection(index);
		RaiseSelectionChanged(value);
	}

	/// <summary>
	/// Canvas bounds changed: adjust scrollable grid as well.
	/// /// </summary>
	/// <param name="rect">Rectangle of the canvas.</param>
	void OnCanvasBoundsChanged(Rect rect) => UpdateItems();

	/// <summary>
	/// Raises the selection changed event.
	/// </summary>
	/// <param name="index">Index selected.</param>
	void RaiseSelectionChanged(int index)
	{
		object? item = null;
		var list = GetItemsAsList();
		if (index >= 0 && index < list.Count)
			item = list[index];
		var eventArgs = new ChartSelectionChangedEventArgs(SelectionChangedEvent)
		{ SelectedIndex = index, SelectedItem = item };
		RaiseEvent(eventArgs);
		skipSelectedItemChanged = true;
		SelectedItem = item;
		skipSelectedItemChanged = false;
	}

	/// <summary>
	/// Sets the selection bit for the selected chart point.
	/// </summary>
	/// <param name="selectedIndex">Index of the selected item.</param>
	void SetSelection(int selectedIndex)
	{
		if (canvas == null) return;
		var chartPoints = canvas.GetLogicalDescendants().OfType<ChartPoint>().ToList();
		var chartPointIndex = chartPoints.FindIndex(x => x.Index == selectedIndex);

		for (var i = 0; i < chartPoints.Count; i++)
			chartPoints[i].IsSelected = i == chartPointIndex;
	}

	/// <summary>
	/// Raises the selection changed event.
	/// </summary>
	/// <param name="item">Item selected.</param>
	void RaiseSelectionChanged(object item)
	{
		var index = GetItemsAsList().IndexOf(item);

		var eventArgs = new ChartSelectionChangedEventArgs(SelectionChangedEvent)
		{ SelectedIndex = index, SelectedItem = item };
		RaiseEvent(eventArgs);
		skipSelectedIndexChanged = true;
		SelectedIndex = index;
		skipSelectedIndexChanged = false;
	}

	/// <summary>
	/// Select next chart point.
	/// </summary>
	/// <param name="step">Step to take.</param>
	void SelectNext(int step)
	{
		if (canvas == null) return;
		var chartPoints = canvas.GetLogicalDescendants().OfType<ChartPoint>().ToList();
		if (chartPoints.Count == 0) return;
		var chartPointIndex = chartPoints.FindIndex(x => x.Index == SelectedIndex);
		chartPointIndex += step;
		if (chartPointIndex < 0)
			chartPointIndex = chartPoints.Count - 1;
		else if (chartPointIndex >= chartPoints.Count)
			chartPointIndex = 0;
		SelectedItem = chartPoints[chartPointIndex].DataContext;
	}

	/// <summary>
	/// Updates the items in the view.
	/// </summary>
	void UpdateItems()
	{
		if (canvas == null || polyline == null) return;
		ClearCanvasItems();
		if (Items == null) return;
		var indexedItems = Items.ToList();
		var chartPoints = Convert(indexedItems);
		if (chartPoints.Count == 0)
		{
			polyline.Points = new List<Point>();
			XMinimum = null;
			XMaximum = null;
			YMinimum = null;
			YMaximum = null;
			UpdateMinimaMaximaTextBlocks();
			return;
		}

		var points = new List<Point>();

		var stats = GetStatistics(chartPoints);
		var xMinimum = chartPoints[stats.XMinimumIndex].XToDouble();
		var xMaximum = chartPoints[stats.XMaximumIndex].XToDouble();
		var yMinimum = chartPoints[stats.YMinimumIndex].YToDouble();
		var yMaximum = chartPoints[stats.YMaximumIndex].YToDouble();
		var xScale = canvas.Bounds.Width / (xMaximum - xMinimum);
		var yScale = canvas.Bounds.Height / (yMaximum - yMinimum);
		foreach (var item in chartPoints)
		{
			var x = (item.XToDouble() - xMinimum) * xScale;
			var y = canvas.Bounds.Height - (item.YToDouble() - yMinimum) * yScale;

			points.Add(new Point(x, y));
			canvas.Children.Add(item);
			Canvas.SetLeft(item, x - (item.Width / 2.0));
			Canvas.SetTop(item, y - (item.Height / 2.0));
		}

		DrawXGridLines(xMinimum, xMaximum);
		DrawYGridLines(yMinimum, yMaximum);
		polyline.Points = points;
		YMinimum = indexedItems[stats.YMinimumIndex];
		YMaximum = indexedItems[stats.YMaximumIndex];
		XMinimum = indexedItems[stats.XMinimumIndex];
		XMaximum = indexedItems[stats.XMaximumIndex];
		UpdateMinimaMaximaTextBlocks();
	}

	/// <summary>
	/// Updates the minimum and maximum text blocks.
	/// </summary>
	void UpdateMinimaMaximaTextBlocks()
	{
		if (xMinimumTextBlock != null) xMinimumTextBlock.Text = ConvertXValueToText(XMinimum);
		if (xMaximumTextBlock != null) xMaximumTextBlock.Text = ConvertXValueToText(XMaximum);
		if (yMinimumTextBlock != null) yMinimumTextBlock.Text = ConvertYValueToText(YMinimum);
		if (yMaximumTextBlock != null) yMaximumTextBlock.Text = ConvertYValueToText(YMaximum);
	}

	/// <summary>
	/// Gets the items as a list.
	/// </summary>
	/// <returns>The list.</returns>
	IList GetItemsAsList() => Items as IList ?? Items.ToList();

	static (int XMinimumIndex, int XMaximumIndex, int YMinimumIndex, int YMaximumIndex) GetStatistics(IList<ChartPoint> items)
	{
		var xMinimumIndex = -1;
		var xMaximumIndex = -1;
		var yMinimumIndex = -1;
		var yMaximumIndex = -1;
		var xMinimum = double.NaN;
		var xMaximum = double.NaN;
		var yMinimum = double.NaN;
		var yMaximum = double.NaN;

		for (var i = 0; i < items.Count; i++)
		{
			if (i == 0)
			{
				xMinimumIndex = i;
				xMaximumIndex = i;
				yMinimumIndex = i;
				yMaximumIndex = i;
				xMinimum = items[i].XToDouble();
				xMaximum = items[i].XToDouble();
				yMinimum = items[i].YToDouble();
				yMaximum = items[i].YToDouble();
				continue;
			}
			if (items[i].XToDouble() < xMinimum)
			{
				xMinimumIndex = i;
				xMinimum = items[i].XToDouble();
			}
			if (items[i].XToDouble() > xMaximum)
			{
				xMaximumIndex = i;
				xMaximum = items[i].XToDouble();
			}
			if (items[i].YToDouble() < yMinimum)
			{
				yMinimumIndex = i;
				yMinimum = items[i].YToDouble();
			}
			if (items[i].YToDouble() > yMaximum)
			{
				yMaximumIndex = i;
				yMaximum = items[i].YToDouble();
			}
		}
		return (xMinimumIndex, xMaximumIndex, yMinimumIndex, yMaximumIndex);
	}

	/// <summary>
	/// Clears the canvas items except the polyline.
	/// </summary>
	void ClearCanvasItems()
	{
		if (canvas == null || polyline == null) return;
		var i = 0;
		while (i < canvas.Children.Count)
		{
			if (!ReferenceEquals(canvas.Children[i], polyline))
				canvas.Children.RemoveAt(i);
			else
				i++;
		}
	}

	/// <summary>
	/// Draws the grid lines for the X-axis.
	/// </summary>
	/// <param name="minimumX">Minimum of the X-values.</param>
	/// <param name="maximumX">Maximum of the X-values.</param>
	void DrawXGridLines(double minimumX, double maximumX)
	{
		if (canvas == null) return;
		var range = maximumX - minimumX;
		var stepSize = GridHelper.GetGridSize(range, 8);
		var list = new List<Line>();
		var beginValue = GridHelper.RoundUp(minimumX, stepSize);
		for (var d = beginValue; d < maximumX; d += stepSize)
		{
			var x = (d - minimumX) / range * canvas.Bounds.Width;
			if (x < 0.0 || x > canvas.Bounds.Width)
				continue;
			var line = new Line { StartPoint = new Point(x, 0.0), EndPoint = new Point(x, canvas.Bounds.Height) };
			BindGridLine(line);
			list.Add(line);
		}
		canvas.Children.InsertRange(0, list);
	}

	/// <summary>
	/// Draws the grid lines for the Y-axis.
	/// </summary>
	/// <param name="minimumY">Minimum of the Y-values.</param>
	/// <param name="maximumY">Maximum of the XY-values.</param>
	void DrawYGridLines(double minimumY, double maximumY)
	{
		if (canvas == null) return;
		var range = maximumY - minimumY;
		var stepSize = GridHelper.GetGridSize(range, 8);
		var list = new List<Line>();
		var beginValue = GridHelper.RoundUp(minimumY, stepSize);
		for (var d = beginValue; d < maximumY; d += stepSize)
		{
			var y = (1.0 - ((d - minimumY) / range)) * canvas.Bounds.Height;
			if (y < 0.0 || y > canvas.Bounds.Height)
				continue;
			var line = new Line { StartPoint = new Point(0.0, y), EndPoint = new Point(canvas.Bounds.Width, y) };
			BindGridLine(line);
			list.Add(line);
		}
		canvas.Children.InsertRange(0, list);
	}

	/// <summary>
	/// Bind the given line to the grid properties of this instance.
	/// </summary>
	/// <param name="line">Line to bind.</param>
	void BindGridLine(IAvaloniaObject line)
	{
		var binding = new Binding("GridStroke")
		{
			RelativeSource = new RelativeSource(RelativeSourceMode.Self),
			Source = this
		};
		line.Bind(Shape.StrokeProperty, binding);
		binding = new Binding("GridStrokeThickness")
		{
			RelativeSource = new RelativeSource(RelativeSourceMode.Self),
			Source = this
		};
		line.Bind(Shape.StrokeThicknessProperty, binding);
	}

	/// <summary>
	/// Converts the given items to a handleable format.
	/// </summary>
	/// <param name="enumerable">Items to process.</param>
	/// <returns>Internal handleable format.</returns>
	IList<ChartPoint> Convert(IEnumerable enumerable)
	{
		var result = new List<ChartPoint>();
		if (!CanBuildItems()) return result;
		{
			var i = 0;
			foreach (var e in enumerable)
			{
				var p =  Convert(e, i);
				result.Add(p);
				i++;
			}
		}

		return result;
	}

	/// <summary>
	/// Converts an item from the source (items) to its equivalent control.
	/// </summary>
	/// <param name="o">Object from items.</param>
	/// <param name="index">Index to assign.</param>
	/// <returns>The control.</returns>
	ChartPoint Convert(object o, int index)
	{
		var p = BuildItem(o);
		p.DataContext = o;
		p.Index = index;
		return p;
	}

	/// <summary>
	/// Checks if an item can be built.
	/// </summary>
	/// <returns>True if it can and false otherwise.</returns>
	bool CanBuildItems() => ItemTemplate != null;

	/// <summary>
	/// Builds the item (control).
	/// </summary>
	/// <param name="param">Parameter to supply to the item template builder.</param>
	/// <returns>The item.</returns>
	ChartPoint BuildItem(object param)
	{
		var result = ItemTemplate?.Build(param) as ChartPoint;
		return result ?? new ChartPoint
		{
			[!ChartPoint.XProperty] = new Binding("X"),
			[!ChartPoint.YProperty] = new Binding("Y")
		};
	}

	/// <summary>
	/// Converts the given x-value to formatted text.
	/// </summary>
	/// <param name="xValue">X-value to convert.</param>
	/// <returns>The string.</returns>
	string ConvertXValueToText(object? xValue) => ValueConverterHelper.ConvertValueToText(xValueConverter, xValue);

	/// <summary>
	/// Converts the given y-value to formatted text.
	/// </summary>
	/// <param name="yValue">Y-value to convert.</param>
	/// <returns>The string.</returns>
	string ConvertYValueToText(object? yValue) => ValueConverterHelper.ConvertValueToText(yValueConverter, yValue);

	/// <inheritdoc />
	Type IStyleable.StyleKey => typeof(ChartControl);
	/// <summary>Items.</summary>
	IEnumerable items = new AvaloniaList<object>();
	/// <summary>State of the left mouse button</summary>
	bool leftButtonDown;
	/// <summary>Skip handling the selected index changed event flag.</summary>
	bool skipSelectedIndexChanged;
	/// <summary>Skip handling the selected item changed event flag.</summary>
	bool skipSelectedItemChanged;
	/// <summary>X-value converter.</summary>
	IValueConverter? xValueConverter = new DefaultXValueConverter();
	/// <summary>Y-value converter.</summary>
	IValueConverter? yValueConverter = new DefaultYValueConverter();
	/// <summary>Canvas.</summary>
	readonly Canvas? canvas;
	/// <summary>Polygon.</summary>
	readonly Polyline? polyline;
	/// <summary>X-values minimum text block.</summary>
	readonly TextBlock? xMinimumTextBlock;
	/// <summary>X-values maximum text block.</summary>
	readonly TextBlock? xMaximumTextBlock;
	/// <summary>Y-values minimum text block.</summary>
	readonly TextBlock? yMinimumTextBlock;
	/// <summary>Y-values maximum text block.</summary>
	readonly TextBlock? yMaximumTextBlock;
}