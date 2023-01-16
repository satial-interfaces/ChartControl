using System;

namespace SatialInterfaces.Helpers;

/// <summary>Grid helper class: it provides methods and extension methods.</summary>
internal static class GridHelper
{
	public static double GetGridSize(double chartRange, double maximumTicks)
	{
		var step = chartRange / maximumTicks;

		var mag = Math.Floor(Math.Log10(step));
		var magPow = Math.Pow(10, mag);

		var magMsd = (int)(step / magPow + 0.5);

		magMsd = magMsd switch
		{
			> 5 => 10,
			> 2 => 5,
			> 1 => 2,
			_ => magMsd
		};

		return magMsd * magPow;
	}

	/// <summary>
	/// Rounds a value up with the given increment.
	/// </summary>
	/// <param name="value">Value.</param>
	/// <param name="increment">Increment.</param>
	/// <returns>The rounded value.</returns>
	public static double RoundUp(double value, double increment)
	{
		if (value == 0.0)
			return value;
		var div = value / increment;
		if (value < 0)
			div -= Math.Truncate(div);
		else
			div = 1.0 - (div - Math.Truncate(div));
		return value + div * increment;
	}
}