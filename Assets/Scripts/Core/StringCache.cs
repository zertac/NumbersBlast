namespace NumbersBlast.Core
{
    public static class StringCache
    {
        private static readonly string[] ValueStrings =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"
        };

        public static string IntToString(int value)
        {
            return value >= 0 && value < ValueStrings.Length ? ValueStrings[value] : value.ToString();
        }
    }
}
