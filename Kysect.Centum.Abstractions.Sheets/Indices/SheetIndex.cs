using System;
using System.Collections.Generic;
using System.Linq;
using Kysect.Centum.Extensions;
using Kysect.Centum.Sheets.Extensions;

namespace Kysect.Centum.Sheets.Indices
{
    public readonly struct SheetIndex : ISheetIndex
    {
        public SheetIndex(ColumnIndex column, RowIndex row)
        {
            column.ThrowIfNull(nameof(column));
            row.ThrowIfNull(nameof(row));

            Column = column;
            Row = row;

            Validate();
        }

        public SheetIndex(string index)
        {
            index.ThrowIfNull(nameof(index));

            IEnumerable<char> columnCharacters = index.TakeWhile(ColumnIndex.IsValid);
            IEnumerable<char> rowCharacters = index.SkipWhile(ColumnIndex.IsValid);

            string column = string.Concat(columnCharacters);
            string row = string.Concat(rowCharacters);

            Column = string.IsNullOrEmpty(column) ? ColumnIndex.None : new ColumnIndex(column);
            Row = string.IsNullOrEmpty(row) ? RowIndex.None : new RowIndex(row);

            Validate();
        }

        public ColumnIndex Column { get; }

        public RowIndex Row { get; }

        public bool IsOpen => Column.Equals(ColumnIndex.None) || Row.Equals(RowIndex.None);

        public ISheetIndex Add(ISheetIndex other)
        {
            other.ThrowIfNull(nameof(other));
            this.ThrowIfNotEquallyOpen(other);
            return new SheetIndex(Column + other.Column, Row + other.Row);
        }

        public ISheetIndex Subtract(ISheetIndex other)
        {
            other.ThrowIfNull(nameof(other));
            this.ThrowIfNotEquallyOpen(other);
            return new SheetIndex(Column - other.Column, Row - other.Row);
        }

        public bool IsEquallyOpen(ISheetIndex other)
        {
            other.ThrowIfNull(nameof(other));

            if (Column.Equals(ColumnIndex.None) && other.Column != ColumnIndex.None)
                return false;

            return !Row.Equals(RowIndex.None) || other.Row == RowIndex.None;
        }

        public ISheetIndex Copy()
            => new SheetIndex(Column, Row);

        public bool Equals(ISheetIndex? other)
            => other is not null && other.Column.Equals(Column) && other.Row.Equals(Row);

        public override bool Equals(object? obj)
            => Equals(obj as ISheetIndex);

        public override int GetHashCode()
            => HashCode.Combine(Column, Row);

        public override string ToString()
            => $"{Column}{Row}";

        private void Validate()
        {
            if (Column.Equals(ColumnIndex.None) && Row.Equals(RowIndex.None))
                throw new ArgumentException("SheetIndex must at least specify one coordinate");
        }
    }
}