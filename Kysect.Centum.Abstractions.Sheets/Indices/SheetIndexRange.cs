using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kysect.Centum.Extensions;

namespace Kysect.Centum.Sheets.Indices
{
    public readonly struct SheetIndexRange : ISheetIndexRange
    {
        public ISheetIndex From { get; }
        public ISheetIndex To { get; }

        /// <summary>
        /// </summary>
        /// <param name="from"> Top left corner </param>
        /// <param name="to"> Bottom right corner </param>
        public SheetIndexRange(ISheetIndex from, ISheetIndex to)
        {
            from.ThrowIfNull(nameof(from));
            to.ThrowIfNull(nameof(to));

            From = from;
            To = to;
        }

        public SheetIndexRange(string range)
        {
            range.ThrowIfNull(nameof(range));

            string[] rangeParts = range.Split(':');
            if (rangeParts.Length is not 1 or 2)
                throw new ArgumentException("Range string must contain one or two indices");

            From = new SheetIndex(rangeParts.First());
            To = new SheetIndex(rangeParts.Last());
        }

        public SheetIndexRange(ISheetIndex index)
            : this(index, index) { }

        public bool IntersectsWith(SheetIndexRange other)
        {
            other.ThrowIfNull(nameof(other));

            //Check if other is on `...` side of this
            bool left = other.To.Column < From.Column;
            bool right = To.Column < other.From.Column;
            bool up = other.To.Row < From.Row;
            bool down = To.Row < other.From.Row;

            return !(left || right || up || down);
        }

        public IEnumerator<ISheetIndex> GetEnumerator()
        {
            ISheetIndex index = From.Copy();

            for (int i = 0; i < To.Row - From.Row; i++)
            {
                for (int j = 0; j < To.Column - From.Column; j++)
                {
                    yield return index;
                    index += new SheetIndex(1, 1);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public override string ToString()
            => $"{From}:{To}";
    }
}