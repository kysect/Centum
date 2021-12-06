using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Kysect.Centum.Extensions;
using Newtonsoft.Json;

namespace Kysect.Centum.Fields.Models
{
    internal sealed class FieldPath : IReadOnlyCollection<JsonPropertyAttribute>, IEquatable<FieldPath>
    {
        private readonly IReadOnlyList<JsonPropertyAttribute> _values;

        public FieldPath(IEnumerable<JsonPropertyAttribute> values)
        {
            _values = values.ToList();
        }

        public IEnumerator<JsonPropertyAttribute> GetEnumerator()
            => _values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int Count => _values.Count;

        public override string ToString()
            => _values.Select(f => f.PropertyName).DotJoin();

        public bool Equals(FieldPath? other)
        {
            if (other is null)
                return false;

            if (Count != other.Count)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (!_values[i].Equals(other._values[i]))
                    return false;
            }

            return true;
        }

        public override bool Equals(object? obj)
            => Equals(obj as FieldPath);

        public override int GetHashCode()
            => _values.GetHashCode();
    }

    internal static class FieldPathExtension
    {
        internal static FieldPath ToFieldPath(this IEnumerable<PropertyInfo> infos)
            => infos
                .Select(i => i.GetCustomAttribute<JsonPropertyAttribute>().ThrowIfNull(nameof(JsonPropertyAttribute)))
                .ToFieldPath();

        public static FieldPath ToFieldPath(this IEnumerable<JsonPropertyAttribute> attributes)
            => new FieldPath(attributes);

        public static string StringRepresentation(this IEnumerable<FieldPath> paths)
            => paths.CommaJoin();
    }
}