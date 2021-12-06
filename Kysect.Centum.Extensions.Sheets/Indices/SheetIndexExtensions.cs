using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Extensions;
using Kysect.Centum.Sheets.Indices;

namespace Kysect.Centum.Extensions.Sheets.Indices
{
    public static class SheetIndexExtension
    {
        public static GridCoordinate ToGridCoordinate(this ISheetIndex index, int sheetId)
        {
            index.ThrowIfOpen();

            return new GridCoordinate
            {
                SheetId = sheetId,
                ColumnIndex = ((int?)index.Column).ApplyIfNotNull(i => i - 1),
                RowIndex = ((int?)index.Row).ApplyIfNotNull(i => i - 1),
            };
        }

        public static ISheetIndex ToSheetIndex(this GridCoordinate coordinate)
            => new SheetIndex(
                coordinate.ColumnIndex.ApplyIfNotNull(i => i + 1) ?? ColumnIndex.None,
                coordinate.RowIndex.ApplyIfNotNull(i => i + 1) ?? RowIndex.None);

        public static string ToString(this ISheetIndex index, string sheetName)
            => $"{sheetName}!{index}";
    }
}