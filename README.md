# ChartControl for Avalonia

This is a chart control (line with markers) for Avalonia. See and run the sample app to get to know it.

![ChartControl screenshot](/Images/ChartControl.png)

## How to use

First add the package to your project. Use NuGet to get it: https://www.nuget.org/packages/ChartControl.Avalonia/

Or use this command in the Package Manager console to install the package manually
```
Install-Package ChartControl.Avalonia
```

Second add a style to your App.axaml (from the sample app)

````Xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Styles>
        <!-- Overall style goes here: <FluentTheme Mode="Light"/> -->
        <StyleInclude Source="avares://SampleApp/Themes/FluentChartControl.axaml" />
    </Application.Styles>
</Application>
````

Or use the default one

````Xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Styles>
        <!-- Overall style goes here: <FluentTheme Mode="Light"/> -->
        <StyleInclude Source="avares://ChartControl/Themes/Default.axaml" />
    </Application.Styles>
</Application>
````

Then add the control to your Window.axaml (minimum)

````Xml
<Window xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:si="clr-namespace:SatialInterfaces.Controls;assembly=ChartControl">
    <Grid>
        <si:ChartControl />
    </Grid>
</Window>
````

It's even better to specify the item template with binding to your view model

````Xml
<Window xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:si="clr-namespace:SatialInterfaces.Controls;assembly=ChartControl">
    <Grid>
        <si:ChartControl>
            <si:ChartControl.ItemTemplate>
                <DataTemplate>
                    <si:ChartPoint X="{Binding X}" Y="{Binding Y}" />
                </DataTemplate>
            </si:ChartControl.ItemTemplate>
        </si:ChartControl>
    </Grid>
</Window>
````
