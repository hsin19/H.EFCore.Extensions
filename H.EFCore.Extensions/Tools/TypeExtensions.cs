using System.Reflection;
using System.Runtime.CompilerServices;

namespace H.EFCore.Extensions.Tools;

/// <summary>
/// Extension Method for <see cref="Type"/>
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAnonymousType(this Type type)
    {
        return type.IsGenericType
            && (type.Name.Contains("AnonymousType") || type.Name.Contains("AnonType"))
            && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute)).Any();
    }

    /// <summary>
    /// Searches for the public property with the specified complex name. AnonymousType can omit the property name.
    /// <para>
    /// "User.UserId" will return {User, UserId}, same result for "UserId" if <paramref name="type"/> is anonymous.
    /// </para>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="name"></param>
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
