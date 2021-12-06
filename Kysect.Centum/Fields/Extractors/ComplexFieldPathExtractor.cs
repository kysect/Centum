using System.Collections.Generic;
using System.Linq;
using Google.Apis.Requests;
using Kysect.Centum.Extensions;
using Kysect.Centum.Fields.Models;

namespace Kysect.Centum.Fields.Extractors
{
    internal sealed class ComplexFieldPathExtractor<TFrom, TTo> : FieldPathExtractor<TFrom>
        where TFrom : IDirectResponseSchema
        where TTo : IDirectResponseSchema
    {
        private readonly FieldPath _basePath;
        private readonly IReadOnlyCollection<FieldPathExtractor<TTo>> _targetAssigners;

        internal ComplexFieldPathExtractor(FieldPath basePath,
                                           IReadOnlyCollection<FieldPathExtractor<TTo>> targetAssigners)
        {
            basePath.ThrowIfNull(nameof(basePath));
            targetAssigners.ThrowIfNull(nameof(targetAssigners));

            _basePath = basePath;
            _targetAssigners = targetAssigners;
        }

        public override string ToString()
            => $"{_basePath}({_targetAssigners.Select(a => a.ToString()).CommaJoin()})";
    }
}