using System.Collections.Generic;
using System.Reflection;
using Google.Apis.Requests;

namespace Kysect.Centum.Fields.Contracts
{
    internal interface IEntityContract
    {
        IReadOnlyCollection<PropertyInfo> Filter(IReadOnlyCollection<PropertyInfo> infos);
    }

    internal interface IEntityContract<TEntity> : IEntityContract where TEntity : IDirectResponseSchema { }
}