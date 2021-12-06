using System;
using System.Linq;
using System.Text;
using Kysect.Centum.Extensions;
using Kysect.Centum.Sheets.Extensions;

namespace Kysect.Centum.Sheets.Indices
{
    /// <summary>
    /// A one-based index (starts from 1)
    /// </summary>
    public readonly struct ColumnIndex : IEquatable<ColumnIndex>
    {
        public ColumnIndex(int value)
        {
            Value = value.PositiveOrThrow(nameof(value));
        }

        public ColumnIndex(string value)
            : this(StringToInt(value)) { }

        public int Value { get; }

        public static readonly ColumnIndex None = new ColumnIndex();

        public static ColumnIndex operator +(ColumnIndex left, ColumnIndex right)
            => new ColumnIndex(left.Value + right.Value);

        public static ColumnIndex operator -(ColumnIndex left, ColumnIndex right)
            => new ColumnIndex(left.Value - right.Value);

        public static bool operator <(ColumnIndex left, ColumnIndex right)
            => left.Value < right.Value;

        public static bool operator >(ColumnIndex left, ColumnIndex right)
            => left.Value > right.Value;

        public static bool operator ==(ColumnIndex left, ColumnIndex right)
            => left.Equals(right);

        public static bool operator !=(ColumnIndex left, ColumnIndex right)
            => !(left == right);

        public static implicit operator ColumnIndex(int value)
            => new ColumnIndex(value);

        public static explicit operator int(ColumnIndex index)
            => index.Value;

        public static explicit operator int?(ColumnIndex index)
            => index.Equals(None) ? null : index.Value;

        public static bool IsValid(string value)
            => value.All(IsValid);

        public static bool IsValid(char c)
            => char.ToUpper(c) is >= 'A' and <= 'Z';

        public bool Equals(ColumnIndex other)
            => other.Value.Equals(Value);

        public override bool Equals(object? obj)
            => obj is ColumnIndex index && Equals(index);

        public override int GetHashCode()
        {
            return Value;
        }

        public override string ToString()
            => IntToString(Value);

        private static string IntToString(int value)
        {
            value.ThrowIfNull(nameof(value));

            if (value == 0)
                return string.Empty;

            var builder = new StringBuilder();

            while (value > 0)
            {
                int mod = (value - 1) % 26;
                builder.Append((char)('A' + mod));
                value = (value - mod) / 26;
            }

            return builder.ToString();
        }

        private static int StringToInt(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Invalid column string");

            if (!IsValid(value))
                throw new ArgumentException("Given string has char that couldn't be a column index");

            string upper = value.ToUpper();
            int result = 0;
            for (int i = upper.Length - 1; i >= 0; i--)
            {
                char symbol = upper[i];
                result += (int)Math.Pow(26, i) * (symbol - 'A' + 1);
            }

            return result;
        }
    }
}