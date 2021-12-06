using System;

namespace Kysect.Centum.Extensions
{
    internal static class GenericExtension
    {
        public static TValue ThrowIfNull<TValue>(this TValue? value, string argumentName)
            => value.ThrowIfNull(new ArgumentNullException(argumentName));

        public static TValue ThrowIfNull<TValue, TException>(this TValue? value)
            where TException : Exception, new()
            => value.ThrowIfNull(new TException());

        public static TValue ThrowIfNull<TValue, TException>(this TValue? value, TException exception)
            where TException : Exception
        {
            if (value is null)
                throw exception;

            return value;
        }

        public static TValue ThrowIfNull<TValue>(this TValue? value, string argumentName)
            where TValue : struct
            => value.ThrowIfNull(new ArgumentNullException(argumentName));

        public static TValue ThrowIfNull<TValue, TException>(this TValue? value)
            where TValue : struct
            where TException : Exception, new()
            => value.ThrowIfNull(new TException());

        public static TValue ThrowIfNull<TValue, TException>(this TValue? value, TException exception)
            where TValue : struct
            where TException : Exception
        {
            if (value is null)
                throw exception;

            return value.Value;
        }

        public static TResult? ApplyIfNotNull<TSource, TResult>(this TSource? value, Func<TSource, TResult?> modifier)
            => value is not null ? modifier(value) : default;
    }
}