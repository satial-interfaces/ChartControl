using System;
using System.Reflection;

namespace SatialInterfaces.Helpers;

/// <summary>System helper class: it provides methods and extension methods.</summary>
internal static class SystemHelper
{
	/// <summary>
	/// Gets a property/field value.
	/// </summary>
	/// <param name="obj">The obj to act on.</param>
	/// <param name="memberName">Name of the property/field.</param>
	/// <returns>The value.</returns>
	public static object? GetValue(object obj, string memberName)
	{
		if (string.IsNullOrEmpty(memberName)) return null;
		var p = Array.Find(obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly), m => string.Equals(m.Name, memberName, StringComparison.Ordinal));
		if (p != null)
    		return p.GetValue(obj);
        var f = Array.Find(obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly), m => string.Equals(m.Name, memberName, StringComparison.Ordinal));
        return f != null ? f.GetValue(obj) : null;
	}
}