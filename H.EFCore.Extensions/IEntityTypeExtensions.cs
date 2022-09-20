using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Caching.Memory;

namespace H.EFCore.Extensions;

/// <summary>
/// Extension Method for <see cref="IEntityType"/>
/// </summary>
public static class IEntityTypeExtensions
{
    /// <summary>
    /// Get the <see cref="PropertyInfo"/>s of <see cref="IEntityType"/> which can identify uniqueness
    /// </summary>
    /// <param name="entityType">The entity type</param>
    /// <param name="cache">Memory Cache</param>
    /// <returns>A list of unique fields.</returns>
    /// <remarks>
    /// Identify uniqueness rank:
    ///     <list type="number">
    ///         <item><description>Primary Key</description></item>
    ///         <item><description>Alternate Key with fewest properties</description></item>
    ///         <item><description>All Columns</description></item>
    ///     </list>
    /// </remarks>
    public static List<PropertyInfo> GetUniquePropertyInfo(this IEntityType entityType, IMemoryCache? cache)
    {
        if (cache == null)
        {
            return GetUniquePropertyInfo(entityType);
        }
        var properties = cache.GetOrCreate(
            (typeof(IEntityTypeExtensions), entityType),
            (entry) =>
            {
                entry.SetSize(10);
                return GetUniquePropertyInfo(entityType);
            });
        return properties;
    }

    /// <inheritdoc cref="GetUniquePropertyInfo(IEntityType, IMemoryCache)"/>
    public static List<PropertyInfo> GetUniquePropertyInfo(this IEntityType entityType)
    {
        var key = entityType.FindPrimaryKey();
        key ??= entityType.GetKeys().MinBy(e => e.Properties.Count);
        var properties = key?.Properties ?? entityType.GetProperties();
        return properties.Select(p => p.PropertyInfo).OfType<PropertyInfo>().ToList();
    }
}
