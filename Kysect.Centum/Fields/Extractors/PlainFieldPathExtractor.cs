using System;
using System.Collections.Concurrent;
using Google.Apis.Requests;
using Kysect.Centum.Fields.Models;

namespace Kysect.Centum.Fields.Extractors
{
    internal sealed class PlainFieldPathExtractor<TFrom> : FieldPathExtractor<TFrom>
        where TFrom : IDirectResponseSchema
    {
        private static readonly ConcurrentDictionary<PlainFieldPathAssignerCacheKey, PlainFieldPathExtractor<TFrom>> Cache = new ();

        internal FieldPath Path { get; }

        private PlainFieldPathExtractor(FieldPath path)
        {
            Path = path;
        }

        internal static PlainFieldPathExtractor<TFrom> GetInstance(FieldPath path, Type sourceType)
        {
            return Cache
                .GetOrAdd(new PlainFieldPathAssignerCacheKey(sourceType, path),
                          _ => new PlainFieldPathExtractor<TFrom>(path));
        }

        public override string ToString()
            => Path.ToString();

        private record PlainFieldPathAssignerCacheKey(Type SourceType, FieldPath Path);
    }
}