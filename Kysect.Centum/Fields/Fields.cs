using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Google.Apis.Requests;
using Kysect.Centum.Caches;
using Kysect.Centum.Extensions;
using Kysect.Centum.Fields.Contracts;
using Kysect.Centum.Fields.Extractors;
using Kysect.Centum.Fields.Models;
using Newtonsoft.Json;

namespace Kysect.Centum.Fields
{
    public static class Fields<TFrom> where TFrom : IDirectResponseSchema
    {
        public static FieldPathExtractor<TFrom> From(Expression<Func<TFrom, object?>> expression)
        {
            PropertyPath propertyPath = ExtractInfos(expression);
            return Create<TFrom>(propertyPath);
        }

        public static FieldPathExtractor<TFrom> FromMany<TTarget>(
            Expression<Func<TFrom, TTarget?>> originExpression,
            params FieldPathExtractor<TTarget>[] targetAssigners)
            where TTarget : IDirectResponseSchema
        {
            return CreateComplexExtractor(originExpression, targetAssigners);
        }

        public static FieldPathExtractor<TFrom> FromMany<TTarget>(
            Expression<Func<TFrom, TTarget?>> originExpression,
            params Expression<Func<TTarget, object?>>[] targetExpressions)
            where TTarget : IDirectResponseSchema
        {
            originExpression.ThrowIfNull(nameof(originExpression));
            targetExpressions.ThrowIfNull(nameof(targetExpressions));
            targetExpressions.ToList().ForEach(expression => expression.ThrowIfNull(nameof(expression)));

            IEnumerable<FieldPathExtractor<TTarget>> targetExtractors = targetExpressions
                .Select(ExtractInfos)
                .Select(Create<TTarget>);

            return CreateComplexExtractor(originExpression, targetExtractors);
        }

        public static FieldPathExtractor<TFrom> FromSequence<TTarget>(
            Expression<Func<TFrom, IEnumerable<TTarget>?>> originExpression,
            params FieldPathExtractor<TTarget>[] targetAssigners)
            where TTarget : IDirectResponseSchema
        {
            originExpression.ThrowIfNull(nameof(originExpression));
            targetAssigners.ThrowIfNull(nameof(targetAssigners));
            targetAssigners.ToList().ForEach(expression => expression.ThrowIfNull(nameof(expression)));

            return CreateComplexExtractor<TFrom, TTarget>(ExtractInfos(originExpression), targetAssigners);
        }

        public static FieldPathExtractor<TFrom> FromSequence<TTarget>(
            Expression<Func<TFrom, IEnumerable<TTarget>?>> originExpression,
            params Expression<Func<TTarget, object?>>[] targetExpressions)
            where TTarget : IDirectResponseSchema
        {
            IEnumerable<FieldPathExtractor<TTarget>> assigners = targetExpressions
                .ThrowIfNull(nameof(targetExpressions))
                .Select(ExtractInfos)
                .Select(Create<TTarget>);
            return CreateComplexExtractor<TFrom, TTarget>(ExtractInfos(originExpression), assigners);
        }

        private static FieldPathExtractor<TOrigin> Create<TOrigin>(PropertyPath infos)
            where TOrigin : IDirectResponseSchema
        {
            PropertyInfo destination = infos.Destination;
            FieldPath path = infos.ToFieldPath();

            Type fromType = typeof(TOrigin);
            Type toType = destination.PropertyType;

            if (toType.IsAssignableTo(typeof(IEnumerable)) && toType.IsConstructedGenericType)
            {
                Type? newToType = toType.GetGenericArguments().SingleOrDefault(t => t.IsAssignableTo(typeof(IDirectResponseSchema)));
                toType = newToType ?? toType;
            }

            IReadOnlyCollection<PropertyInfo> innerProperties = toType
                .GetProperties()
                .Where(p => p.GetCustomAttribute<JsonPropertyAttribute>() is not null)
                .ToArray();

            IEntityContract? contract = EntityContractLocator.GetContract(toType);
            if (contract is not null)
                innerProperties = contract.Filter(innerProperties);

            if (!innerProperties.Any())
                return PlainFieldPathExtractor<TOrigin>.GetInstance(path, fromType);

            object extractors = InfosCache
                .GetMethodInfos(typeof(Fields<TFrom>))
                .Single(i => i.Name.Equals(nameof(GetExtractors)))
                .GetGenericMethodInfo(toType)
                .Invoke(null, new object?[] { innerProperties })
                .ThrowIfNull(nameof(GetExtractors));

            ConstructorInfo constructor = InfosCache
                .GetConstructorInfos(typeof(ComplexFieldPathExtractor<,>), fromType, toType)
                .Single();

            return (FieldPathExtractor<TOrigin>)constructor.Invoke(new[] { path, extractors })
                .ThrowIfNull(nameof(FieldPathExtractor<TOrigin>));
        }

        private static FieldPathExtractor<TOrigin> CreateComplexExtractor<TOrigin, TTarget>(
            Expression<Func<TOrigin, TTarget?>> originExpression,
            IEnumerable<FieldPathExtractor<TTarget>> targetAssigners)
            where TOrigin : IDirectResponseSchema
            where TTarget : IDirectResponseSchema
        {
            originExpression.ThrowIfNull(nameof(originExpression));
            PropertyPath propertyPath = ExtractInfos(originExpression);

            return CreateComplexExtractor<TOrigin, TTarget>(propertyPath, targetAssigners);
        }

        private static FieldPathExtractor<TOrigin> CreateComplexExtractor<TOrigin, TTarget>(
            PropertyPath originPath,
            IEnumerable<FieldPathExtractor<TTarget>> targetAssigners)
            where TOrigin : IDirectResponseSchema
            where TTarget : IDirectResponseSchema
        {
            return new ComplexFieldPathExtractor<TOrigin, TTarget>(
                originPath.ToFieldPath(),
                targetAssigners.ThrowIfNull(nameof(targetAssigners)).ToList()
            );
        }

        private static IReadOnlyCollection<FieldPathExtractor<TOrigin>> GetExtractors<TOrigin>(IReadOnlyCollection<PropertyInfo> properties)
            where TOrigin : IDirectResponseSchema
        {
            return properties
                .Select(p => new PropertyPath(new[] { p }))
                .Select(Create<TOrigin>)
                .ToArray();
        }

        private static PropertyPath ExtractInfos<TOrigin, TTarget>(Expression<Func<TOrigin, TTarget?>> expression)
        {
            if (expression.Parameters.Count != 1)
                throw new ArgumentException("Provided expression has invalid parameter count. \n" +
                                            "Expected: 1\n" +
                                            $"Was: {expression.Parameters.Count}");

            ParameterExpression parameter = expression.Parameters.Single();
            return GetPropertyInfos(parameter, expression.Body);
        }

        private static PropertyPath GetPropertyInfos(Expression parameterExpression, Expression memberAccessExpression)
        {
            var memberInfos = new Stack<PropertyInfo>();

            do
            {
                Expression? pureExpression = RemoveTypeAs(RemoveConversions(memberAccessExpression));
                if (pureExpression is not MemberExpression memberExpression)
                    throw new NotSupportedException($"'{pureExpression?.NodeType}' expression type is not supported");

                if (memberExpression.Member is not PropertyInfo propertyInfo || memberExpression.Expression is null)
                    return new PropertyPath(Array.Empty<PropertyInfo>());

                memberInfos.Push(propertyInfo);
                memberAccessExpression = RemoveTypeAs(RemoveConversions(memberExpression.Expression))
                    .ThrowIfNull(nameof(memberExpression.Expression));
            }
            while (memberAccessExpression != parameterExpression);

            List<PropertyInfo> infoList = memberInfos
                .Aggregate(new List<PropertyInfo>(), (l, i) =>
                {
                    l.Add(i);
                    return l;
                });

            return new PropertyPath(infoList);
        }

        private static Expression RemoveConversions(Expression expression)
        {
            while (expression is UnaryExpression unaryExpression &&
                   expression.NodeType is ExpressionType.Convert or ExpressionType.ConvertChecked)
            {
                expression = unaryExpression.Operand;
            }

            return expression;
        }

        private static Expression? RemoveTypeAs(Expression? expression)
        {
            while (expression?.NodeType == ExpressionType.TypeAs)
            {
                expression = ((UnaryExpression)RemoveConversions(expression)).Operand;
            }

            return expression;
        }
    }
}