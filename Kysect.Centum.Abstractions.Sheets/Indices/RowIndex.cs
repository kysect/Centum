using System;
using System.Linq;
using Kysect.Centum.Sheets.Extensions;

namespace Kysect.Centum.Sheets.Indices
{
    /// <summary>
    /// A one-based index (starts from 1)
    /// </summary>
    public readonly struct RowIndex : IEquatable<RowIndex>
    {
        public RowIndex(int value)
        {
            Value = value.PositiveOrThrow(nameof(value));
        }

        public RowIndex(string value) : this(StringToInt(value)) { }

        public int Value { get; }

        public static readonly RowIndex None = new ();

        public static RowIndex operator +(RowIndex left, RowIndex right)
            => new RowIndex(left.Value + right.Value);

        public static RowIndex operator -(RowIndex left, RowIndex right)
            => new RowIndex(left.Value - right.Value);

        public static bool operator <(RowIndex left, RowIndex right)
            => left.Value < right.Value;

        public static bool operator >(RowIndex left, RowIndex right)
            => left.Value > right.Value;

        public static bool operator ==(RowIndex left, RowIndex right)
            => left.Equals(right);

        public static bool operator !=(RowIndex left, RowIndex right)
            => !(left == right);

        public static implicit operator RowIndex(int value)
            => new RowIndex(value);

        public static explicit operator int(RowIndex index)
            => index.Value;

        public static explicit operator int?(RowIndex index)
            => index.Equals(None) ? null : index.Value;

        public static bool IsValid(string value)
            => value.All(IsValid);

        public static bool IsValid(char c)
            => char.IsDigit(c);

        public bool Equals(RowIndex other)
            => other.Value.Equals(Value);

        public override bool Equals(object? obj)
            => obj is RowIndex index && Equals(index);

        public override string ToString()
            => IntToString(Value);

        public override int GetHashCode()
            => Value.GetHashCode();

        private static int StringToInt(string value)
            => int.Parse(value);

        private static string IntToString(int value)
            => value == 0 ? string.Empty : value.ToString();
    }
}