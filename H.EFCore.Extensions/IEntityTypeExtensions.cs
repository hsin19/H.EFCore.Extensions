using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace H.EFCore.Extensions;

/// <summary>
/// Extension Method for <see cref="IEntityType"/>
/// </summary>
public static class IEntityTypeExtensions
{
    private static readonly Dictionary<IEntityType, List<PropertyInfo>> s_cacheKey = new();

    /// <summary>
    /// Get the <see cref="PropertyInfo"/>s of <see cref="IEntityType"/> which can identify uniqueness
    /// </summary>
    /// <param name="entityType">The entity type</param>
    /// <returns>A list of unique fields.</returns>
    /// <remarks>
    /// Identify uniqueness rank:
    ///     <list type="number">
    ///         <item><description>Primary Key</description></item>
    ///         <item><description>Alternate Key with fewest properties</description></item>
    ///         <item><description>All Columns</description></item>
    ///     </list>
    /// </remarks>
    public static List<PropertyInfo> GetUniquePropertyInfo(this IEntityType entityType)
    {
        if (!s_cacheKey.TryGetValue(entityType, out var properties))
        {
            var keys = entityType.GetKeys();
            var key = keys.FirstOrDefault(k => k.IsPrimaryKey());
            key ??= keys.MinBy(e => e.Properties.Count);
            properties = key?.Properties
                .Select(k => k.PropertyInfo)
                .OfType<PropertyInfo>()
                .ToList();
            properties ??= entityType.GetProperties()
                .Select(p => p.PropertyInfo).OfType<PropertyInfo>()
                .ToList();
            s_cacheKey[entityType] = properties;
        }
        return properties;
    }
}
