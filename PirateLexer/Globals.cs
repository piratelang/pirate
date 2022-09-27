namespace PirateLexer
{
    public static class Globals
    {
        public static string DIGITS { get; set; } = "1234567890";
        public static string LETTERS { get; set; } = "abdcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string LETTERS_DIGITS { get; set; } = DIGITS + LETTERS;

        public static string[] KEYWORDS { get; set; } = new string[] {
                "var",
                "and",
                "or",
                "not",
                "if",
                "then",
                "elif",
                "else",
                "for",
                "to",
                "step",
                "while",
                "func"
        };
    }
}