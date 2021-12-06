namespace Kysect.Centum.Sheets.Models
{
    public readonly struct Dimension
    {
        private readonly string _value;

        private Dimension(string value)
        {
            _value = value;
        }

        public static Dimension Rows => new Dimension("ROWS");
        public static Dimension Columns => new Dimension("COLUMNS");

        public static implicit operator string(Dimension dimension)
            => dimension.ToString();

        public override string ToString()
            => _value;
    }
}