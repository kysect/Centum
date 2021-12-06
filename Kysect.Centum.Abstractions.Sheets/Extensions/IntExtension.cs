using System;

namespace Kysect.Centum.Sheets.Extensions
{
    internal static class IntExtension
    {
        public static int PositiveOrThrow(this int value, string argumentName)
        {
            if (value <= 0)
                throw new ArgumentException($"{argumentName} must be positive");

            return value;
        }
    }
}