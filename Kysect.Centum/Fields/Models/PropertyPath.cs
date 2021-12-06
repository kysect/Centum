using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Kysect.Centum.Fields.Models
{
    public class PropertyPath : IReadOnlyCollection<PropertyInfo>
    {
        private readonly IReadOnlyList<PropertyInfo> _propertyInfos;

        public int Count => _propertyInfos.Count;
        public PropertyInfo Destination => _propertyInfos[^1];

        public PropertyPath(IReadOnlyList<PropertyInfo> propertyInfos)
        {
            _propertyInfos = propertyInfos;
        }

        public IEnumerator<PropertyInfo> GetEnumerator()
            => _propertyInfos.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}