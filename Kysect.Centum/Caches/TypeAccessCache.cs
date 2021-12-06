using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kysect.Centum.Caches
{
    internal class TypeAccessCache
    {
        private readonly ConcurrentDictionary<string, Action<object, object?>> _setters = new();
        private readonly ConcurrentDictionary<string, Func<object, object?>> _getters = new();
        private readonly ConcurrentDictionary<string, Func<object, object[], object?>> _functions = new();

        private readonly Lazy<IReadOnlyCollection<FieldInfo>> _fieldInfos;
        private readonly Lazy<IReadOnlyCollection<PropertyInfo>> _propertyInfos;
        private readonly Lazy<IReadOnlyCollection<MethodInfo>> _methodInfos;

        public TypeAccessCache(Type type)
        {
            _fieldInfos = new Lazy<IReadOnlyCollection<FieldInfo>>(() => InfosCache.GetFieldInfos(type));
            _propertyInfos = new Lazy<IReadOnlyCollection<PropertyInfo>>(() => InfosCache.GetPropertiesInfo(type));
            _methodInfos = new Lazy<IReadOnlyCollection<MethodInfo>>(() => InfosCache.GetMethodInfos(type));
        }

        public Action<object, object?> SetterFunc(string name)
            => _setters.GetOrAdd(name, n =>
            {
                MemberInfo? info = _fieldInfos.Value.SingleOrDefault(i => i.Name == n);
                if (info is FieldInfo fieldInfo)
                    return (instance, value) => fieldInfo.SetValue(instance, value);

                info = _propertyInfos.Value.SingleOrDefault(i => i.Name == n);
                if (info is PropertyInfo propertyInfo)
                    return (instance, value) => propertyInfo.SetValue(instance, value);

                throw new InvalidOperationException("Given type does not contain field or property with specified name");
            });

        public Func<object, object?> GetterFunc(string name)
            => _getters.GetOrAdd(name, n =>
            {
                MemberInfo? info = _fieldInfos.Value.SingleOrDefault(i => i.Name == n);
                if (info is FieldInfo fieldInfo)
                    return instance => fieldInfo.GetValue(instance);

                info = _propertyInfos.Value.SingleOrDefault(i => i.Name == n);
                if (info is PropertyInfo propertyInfo)
                    return instance => propertyInfo.GetValue(instance);

                throw new InvalidOperationException("Given type does not contain field or property with specified name");
            });

        public Func<object, object[], object?> Function(string name)
            => _functions.GetOrAdd(name, n =>
            {
                MethodInfo? info = _methodInfos.Value.SingleOrDefault(i => i.Name == n);
                if (info is null)
                    throw new InvalidOperationException("Given type does not contain function with specified name");

                return (target, args) => info.Invoke(target, args);
            });
    }
}