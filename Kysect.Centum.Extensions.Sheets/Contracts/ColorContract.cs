using System;
using System.Collections.Generic;
using System.Reflection;
using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Fields.Contracts;

namespace Kysect.Centum.Extensions.Sheets.Contracts
{
    internal class ColorContract : IEntityContract<Color>
    {
        public IReadOnlyCollection<PropertyInfo> Filter(IReadOnlyCollection<PropertyInfo> infos)
            => Array.Empty<PropertyInfo>();
    }
}