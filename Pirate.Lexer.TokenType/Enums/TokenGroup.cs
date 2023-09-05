using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Pirate.Lexer.TokenType.Enums;

/// <summary>
/// A enum for token groups.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum TokenGroup
{
    OPERATORS,
    COMPARISONOPERATORS,
    TYPEKEYWORD,
    CONTROLKEYWORD,
    SYNTAX,
    VALUE,
    Empty
}
