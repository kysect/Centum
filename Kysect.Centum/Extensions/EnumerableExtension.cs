using System.Collections.Generic;

namespace Kysect.Centum.Extensions
{
    internal static class EnumerableExtension
    {
        public static string CommaJoin<T>(this IEnumerable<T> enumerable)
            => string.Join(",", enumerable);
        
        public static string DotJoin<T>(this IEnumerable<T> enumerable)
            => string.Join(".", enumerable);
    }
}