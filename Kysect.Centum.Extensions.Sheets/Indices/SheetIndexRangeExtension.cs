using System.Collections.Generic;
using System.Linq;
using Google.Apis.Sheets.v4.Data;
using Kysect.Centum.Sheets.Indices;

namespace Kysect.Centum.Extensions.Sheets.Indices
{
    public static class SheetIndexRangeExtension
    {
        public static GridRange ToGoogleGridRange(this ISheetIndexRange range, int sheetId) =>
            new GridRange
            {
                SheetId = sheetId,
                StartColumnIndex = ((int?)range.From.Column).ApplyIfNotNull(i => i - 1),
                StartRowIndex = ((int?)range.From.Row).ApplyIfNotNull(i => i - 1),
                EndColumnIndex = (int?)range.To.Column,
                EndRowIndex = (int?)range.To.Row
            };

        public static IReadOnlyCollection<GridRange> ToGoogleGridRanges(
            this IEnumerable<ISheetIndexRange> ranges, int sheetId)
            => ranges.Select(r => ToGoogleGridRange(r, sheetId)).ToList();

        public static ISheetIndexRange ToSheetIndexRange(this GridRange range)
            => new SheetIndexRange(
                new SheetIndex((range.StartColumnIndex ?? 0) + 1, (range.StartRowIndex ?? 0) + 1),
                new SheetIndex(range.EndColumnIndex ?? 1, range.EndRowIndex ?? 1));

        public static IReadOnlyCollection<ISheetIndexRange> ToSheetIndexRanges(this IEnumerable<GridRange> rages)
            => rages.Select(r => r.ToSheetIndexRange()).ToList();
    }
}