using System.Collections.Generic;
using System.Linq;
using Google.Apis.Requests;
using Kysect.Centum.Extensions;

namespace Kysect.Centum.Fields.Extractors
{
    public abstract class FieldPathExtractor<TFrom> where TFrom : IDirectResponseSchema
    {
        public abstract override string ToString();
    }

    public static class FieldPathAssignerExtension
    {
        public static string StringRepresentation<TFrom>(this IEnumerable<FieldPathExtractor<TFrom>> enumerable)
            where TFrom : IDirectResponseSchema
        {
            return enumerable.Select(a => a.ToString()).CommaJoin();
        }
    }
}