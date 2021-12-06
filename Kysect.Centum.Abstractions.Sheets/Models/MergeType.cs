namespace Kysect.Centum.Sheets.Models
{
    public readonly struct MergeType
    {
        private readonly string _value;

        private MergeType(string value)
        {
            _value = value;
        }

        public static MergeType All => new MergeType("MERGE_ALL");
        public static MergeType Columns => new MergeType("MERGE_COLUMNS");
        public static MergeType Rows => new MergeType("MERGE_ROWS");

        public static implicit operator string(MergeType type)
            => type.ToString();

        public override string ToString()
            => _value;
    }
}