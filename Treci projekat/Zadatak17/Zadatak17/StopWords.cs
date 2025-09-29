namespace Zadatak17
{
    public static class StopWords
    {
        public static readonly HashSet<string> Default = new(StringComparer.OrdinalIgnoreCase)
        {
            "a","an","the","and","or","of","to","with","in","on","for","from",
            "into","then","than","is","are","be","it","as","that","this","by",
            "at","add","mix","pour","stir","shake","serve","garnish","over","until"
        };
    }
}
