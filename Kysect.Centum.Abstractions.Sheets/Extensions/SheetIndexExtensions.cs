using Kysect.Centum.Sheets.Exceptions;
using Kysect.Centum.Sheets.Indices;

namespace Kysect.Centum.Sheets.Extensions
{
    public static class SheetIndexExtensions
    {
        public static ISheetIndex ThrowIfOpen(this ISheetIndex index)
        {
            if (index.IsOpen)
                throw new OpenIndexException("SheetIndex is open\n" +
                                             $"ColumnIndex: {(index.Column.Equals(ColumnIndex.None) ? "None" : index.Column.ToString())}\n" +
                                             $"RowIndex: {(index.Row.Equals(RowIndex.None) ? "None" : index.Row.ToString())}");

            return index;
        }

        public static ISheetIndex ThrowIfNotEquallyOpen(this ISheetIndex index, ISheetIndex other)
        {
            if (!index.IsEquallyOpen(other))
                throw new OpenIndexException("Indices are not equally open: \n" +
                                             $"this: {index}\n" +
                                             $"other: {other}");

            return index;
        }
    }
}