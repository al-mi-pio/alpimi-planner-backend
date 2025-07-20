namespace AlpimiAPI.Utilities
{
    public static class TextUtils
    {
        public static string FirstLetterToLower(string text)
        {
            return char.ToLower(text[0]) + text.Substring(1);
        }
    }
}
