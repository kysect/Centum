using System.Collections.Generic;

namespace Kysect.Centum.Sheets.Indices
{
    public interface ISheetIndexRange : IEnumerable<ISheetIndex>
    {
        ISheetIndex From { get; }
        ISheetIndex To { get; }

        bool IntersectsWith(SheetIndexRange other);
    }
}