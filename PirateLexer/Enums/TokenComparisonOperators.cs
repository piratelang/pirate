namespace PirateLexer.Enums;

[JsonConverter(typeof(StringEnumConverter))]
public enum TokenComparisonOperators
{
    DOUBLEEQUALS,
    NOTEQUALS,
    LESSTHAN,
    GREATERTHAN,
    LESSTHANEQUALS,
    GREATERTHANEQUALS,
    DOUBLEPIPE,
    DOUBLEAMPERSAND
}
