using System.Reflection;
using System.Runtime.CompilerServices;

namespace H.EFCore.Extensions.Tools;

/// <summary>
/// Extension Method for <see cref="Type"/>
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Gets a value indicating whether the current type is an anonymous type.
    /// </summary>
    /// <param name="type">current type</param>
    /// <returns>true if the current type is an anonymous type; otherwise, false.</returns>
    public static bool IsAnonymousType(this Type type)
    {
        return type.IsGenericType
            && (type.Name.Contains("AnonymousType") || type.Name.Contains("AnonType"))
            && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute)).Any();
    }

    /// <summary>
    /// Searches for the public property with the specified complex name.
    /// Anonymous Type can omit the property name.
    /// </summary>
    /// <param name="type">The type that contains the property.</param>
    /// <param name="name">Complex property name</param>
    /// <returns>
    /// An List for property chain, if found; otherwise, null.
    /// </returns>
    public static List<PropertyInfo>? GetComplexProperty(this Type type, string name)
    {
        var dotIndex = name.IndexOf('.');
        if (dotIndex > 0)
        {
            var first = GetComplexProperty(type, name[..dotIndex]);
            if (first is not null)
            {
                var others = GetComplexProperty(first.Last().PropertyType, name[(dotIndex + 1)..]);
                if (others is not null)
                {
                    first.AddRange(others);
                    return first;
                }
            }
            return null;
        }
        if (type.GetProperty(name) is PropertyInfo propertyInfo)
        {
            return new List<PropertyInfo>() { propertyInfo };
        }
        else if (type.IsAnonymousType())
        {
            foreach (var pi in type.GetProperties())
            {
                var child = pi.PropertyType.GetComplexProperty(name);
                if (child is not null)
                {
                    child.Insert(0, pi);
                    return child;
                }
            }
        }
        return null;
    }
}
