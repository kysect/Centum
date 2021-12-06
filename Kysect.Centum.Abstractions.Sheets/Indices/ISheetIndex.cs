using System;

namespace Kysect.Centum.Sheets.Indices
{
    public interface ISheetIndex : IEquatable<ISheetIndex>
    {
        ColumnIndex Column { get; }
        RowIndex Row { get; }

        /// <summary>
        /// Indicates whether the index is open 
        /// </summary>
        /// <returns>
        /// false - if Column and Row != *.None; otherwise true.
        /// </returns>
        bool IsOpen { get; }

        ISheetIndex Add(ISheetIndex other);
        ISheetIndex Subtract(ISheetIndex other);

        bool IsEquallyOpen(ISheetIndex other);

        ISheetIndex Copy();

        public static ISheetIndex operator +(ISheetIndex left, ISheetIndex right)
        {
            return left.Add(right);
        }

        public static ISheetIndex operator -(ISheetIndex left, ISheetIndex right)
        {
            return left.Subtract(right);
        }
    }
}