namespace Kysect.Centum.Sheets.Models
{
    public readonly struct ValueInputOption
    {
        private readonly string _value;

        private ValueInputOption(string value)
        {
            _value = value;
        }

        public static ValueInputOption UserEntered => new ValueInputOption("USER_ENTERED");
        public static ValueInputOption Raw => new ValueInputOption("RAW");

        public static implicit operator string(ValueInputOption option)
            => option.ToString();

        public override string ToString()
            => _value;
    }
}