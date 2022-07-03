using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace IQuerableExtensions;

public static class IEntityTypeExtensions
{
    private static readonly Dictionary<IEntityType, List<PropertyInfo>> CacheKey = new();

    public static List<PropertyInfo> GetUniquePropertyInfo(this IEntityType entityType)
    {
        if (!CacheKey.TryGetValue(entityType, out var properties))
        {
            var keys = entityType.GetKeys();
            // 1. pk
            var key = keys.FirstOrDefault(k => k.IsPrimaryKey());
            // 2. the fewest properties alternate key if no pk
            key ??= keys.MinBy(e => e.Properties.Count);
            properties = key?.Properties.Select(k => k.PropertyInfo).OfType<PropertyInfo>().ToList();
            // 3. all columns if no key
            properties ??= entityType.GetProperties().Select(p => p.PropertyInfo).OfType<PropertyInfo>().ToList();
            CacheKey[entityType] = properties;
        }
        return properties;
    }
}
