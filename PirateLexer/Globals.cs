namespace PirateLexer;

public static class Globals
{
    public static string DIGITS { get; set; } = "1234567890";
    public static string LETTERS { get; set; } = "abdcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static string LETTERS_DIGITS { get; set; } = DIGITS + LETTERS;

    public static string[] BOOLEANS { get; set; } = new string[] {
                "true",
                "false",
                "True",
                "False"
        };
}
