namespace NumbersBlast.Core
{
    /// <summary>
    /// Provides cached string representations of common integers to avoid runtime allocations.
    /// </summary>
    public static class StringCache
    {
        private static readonly string[] ValueStrings =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10",
            "11", "12", "13", "14", "15", "16", "17", "18", "19", "20"
        };

        /// <summary>
        /// Returns a cached string for values 0-20, or falls back to ToString() for out-of-range values.
        /// </summary>
        public static string IntToString(int value)
        {
            return value >= 0 && value < ValueStrings.Length ? ValueStrings[value] : value.ToString();
        }
    }
}
