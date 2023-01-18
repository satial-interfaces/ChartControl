using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SampleApp.Views;

public class ChartPointViewModel
{
	public DateTime X { get; set; }
	public double Y { get; set; }
}

public class XValueConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return BindingOperations.DoNothing;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (targetType != typeof(string))
			return BindingOperations.DoNothing;

		return value is ChartPointViewModel point ? (point.X.ToShortDateString() + " " + point.X.ToLongTimeString()) : BindingOperations.DoNothing;
	}
}

public class YValueConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		return BindingOperations.DoNothing;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (targetType != typeof(string))
			return BindingOperations.DoNothing;

		return value is ChartPointViewModel point ? string.Format(CultureInfo.CurrentCulture, "{0:F2} kg", point.Y) : BindingOperations.DoNothing;
	}
}
public class MainWindow : Window
{
	/// <summary>
	/// Identifies the <see cref="XAxisTitle" /> styled property.
	/// </summary>
	public static readonly StyledProperty<string> XAxisTitleProperty = AvaloniaProperty.Register<MainWindow, string>(nameof(XAxisTitle), string.Empty);
	/// <summary>
	/// Identifies the <see cref="YAxisTitle" /> styled property.
	/// </summary>
	public static readonly StyledProperty<string> YAxisTitleProperty = AvaloniaProperty.Register<MainWindow, string>(nameof(YAxisTitle), string.Empty);
	/// <summary>Items property</summary>
	public static readonly DirectProperty<MainWindow, IEnumerable> ItemsProperty = AvaloniaProperty.RegisterDirect<MainWindow, IEnumerable>(nameof(Items), o => o.Items, (o, v) => o.Items = v);

	/// <summary>X-axis property.</summary>
	public string XAxisTitle { get => GetValue(XAxisTitleProperty); set => SetValue(XAxisTitleProperty, value); }
	/// <summary>Y-axis property.</summary>
	public string YAxisTitle { get => GetValue(YAxisTitleProperty); set => SetValue(YAxisTitleProperty, value); }
	/// <summary>Items property.</summary>
	public IEnumerable Items { get => items; set => SetAndRaise(ItemsProperty, ref items, value); }

#pragma warning disable CS8618
	public MainWindow()
#pragma warning restore CS8618
	{
		InitializeComponent();
#if DEBUG
		this.AttachDevTools();
#endif
	}
	void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);

		RandomItems();
	}

	void RandomItems()
	{
		var now = DateTime.Now;
		var list = new List<ChartPointViewModel>();
		var count = GetRandom(1, 256);
		var xDivider = GetRandom(1, 256);
		var yDivider = GetRandom(1, 256);
		var xOffset = GetRandom(0, 255);
		var yOffset = GetRandom(0, 255);
		for (var i = 0; i < count; i++)
		{
			var x = (GetRandom(0, 255) - xOffset) / (double)xDivider;
			var y = (GetRandom(0, 255) - yOffset)  / (double)yDivider;
			list.Add(new ChartPointViewModel { X = now.AddMinutes(x), Y = y });
		}
		list = list.OrderBy(x => x.X).ToList();
		Items = list;
	}

#pragma warning disable RCS1213
	void RandomButtonClick(object? sender, RoutedEventArgs e) => RandomItems();
#pragma warning restore RCS1213

	static int GetRandom(int minVal, int maxVal) => Random.Next(minVal, maxVal + 1);

	static readonly Random Random = new();
	/// <summary>Items.</summary>
	IEnumerable items = new AvaloniaList<ChartPointViewModel>();
}