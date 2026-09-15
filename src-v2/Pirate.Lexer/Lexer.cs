using System.Globalization;
using System.Text;
using Pirate.Syntax;

namespace Pirate.Lexer;

/// <summary>
/// Single-pass scanner over the raw source text (docs/GRAMMAR.md §1). Unlike
/// v1's lexer, whitespace/newlines are never stripped before scanning, so
/// line/column are tracked per character and every token carries a real
/// source location; and unlike v1, scan errors are collected as diagnostics
/// instead of thrown, matching the rest of the v2 pipeline.
/// </summary>
public static class Lexer
{
    public static LexResult Tokenize(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return new Scanner(source).Run();
    }

    private sealed class Scanner
    {
        private readonly string _source;
        private readonly List<Token> _tokens = [];
        private readonly List<Diagnostic> _diagnostics = [];
        private int _position;
        private int _line = 1;
        private int _column = 1;

        public Scanner(string source) => _source = source;

        public LexResult Run()
        {
            while (!IsAtEnd)
            {
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.Eof, string.Empty, null, CurrentLocation));
            return new LexResult(_tokens, _diagnostics);
        }

        private bool IsAtEnd => _position >= _source.Length;

        private char Current => IsAtEnd ? '\0' : _source[_position];

        private char Peek(int offset) =>
            _position + offset < _source.Length ? _source[_position + offset] : '\0';

        private SourceLocation CurrentLocation => new(_line, _column);

        private char Advance()
        {
            var c = _source[_position];
            _position++;
            if (c == '\n')
            {
                _line++;
                _column = 1;
            }
            else
            {
                _column++;
            }
            return c;
        }

        private void Add(TokenType type, string lexeme, SourceLocation start) =>
            _tokens.Add(new Token(type, lexeme, null, start));

        private void ScanToken()
        {
            var start = CurrentLocation;
            var c = Advance();

            switch (c)
            {
                case ' ' or '\t' or '\r' or '\n':
                    return;

                case '/' when Current == '/':
                    Advance();
                    SkipLineComment();
                    return;
                case '/':
                    Add(TokenType.Slash, "/", start);
                    return;

                case '+':
                    Add(TokenType.Plus, "+", start);
                    return;
                case '-':
                    Add(TokenType.Minus, "-", start);
                    return;
                case '*':
                    Add(TokenType.Star, "*", start);
                    return;
                case '%':
                    Add(TokenType.Percent, "%", start);
                    return;
                case '^':
                    Add(TokenType.Caret, "^", start);
                    return;

                case '(':
                    Add(TokenType.LeftParen, "(", start);
                    return;
                case ')':
                    Add(TokenType.RightParen, ")", start);
                    return;
                case '{':
                    Add(TokenType.LeftBrace, "{", start);
                    return;
                case '}':
                    Add(TokenType.RightBrace, "}", start);
                    return;
                case '[':
                    Add(TokenType.LeftBracket, "[", start);
                    return;
                case ']':
                    Add(TokenType.RightBracket, "]", start);
                    return;

                case ',':
                    Add(TokenType.Comma, ",", start);
                    return;
                case ':':
                    Add(TokenType.Colon, ":", start);
                    return;
                case ';':
                    Add(TokenType.Semicolon, ";", start);
                    return;
                case '.':
                    Add(TokenType.Dot, ".", start);
                    return;

                case '=' when Current == '=':
                    Advance();
                    Add(TokenType.EqualEqual, "==", start);
                    return;
                case '=':
                    Add(TokenType.Equal, "=", start);
                    return;

                case '!' when Current == '=':
                    Advance();
                    Add(TokenType.BangEqual, "!=", start);
                    return;
                case '!':
                    Add(TokenType.Bang, "!", start);
                    return;

                case '<' when Current == '=':
                    Advance();
                    Add(TokenType.LessEqual, "<=", start);
                    return;
                case '<':
                    Add(TokenType.Less, "<", start);
                    return;

                case '>' when Current == '=':
                    Advance();
                    Add(TokenType.GreaterEqual, ">=", start);
                    return;
                case '>':
                    Add(TokenType.Greater, ">", start);
                    return;

                case '&' when Current == '&':
                    Advance();
                    Add(TokenType.AmpAmp, "&&", start);
                    return;
                case '&':
                    _diagnostics.Add(new Diagnostic("Unexpected character '&' (did you mean '&&'?)", start));
                    return;

                case '|' when Current == '|':
                    Advance();
                    Add(TokenType.PipePipe, "||", start);
                    return;
                case '|':
                    _diagnostics.Add(new Diagnostic("Unexpected character '|' (did you mean '||'?)", start));
                    return;

                case '"':
                    ScanString(start);
                    return;
                case '\'':
                    ScanChar(start);
                    return;

                case var digit when IsAsciiDigit(digit):
                    ScanNumber(start);
                    return;

                case var letter when IsIdentifierStart(letter):
                    ScanIdentifier(start);
                    return;

                default:
                    _diagnostics.Add(new Diagnostic($"Unexpected character '{c}'", start));
                    return;
            }
        }

        private void SkipLineComment()
        {
            while (!IsAtEnd && Current != '\n')
            {
                Advance();
            }
        }

        private void ScanNumber(SourceLocation start)
        {
            var startPos = _position - 1;

            while (!IsAtEnd && IsAsciiDigit(Current))
            {
                Advance();
            }

            var isFloat = false;
            if (!IsAtEnd && Current == '.' && IsAsciiDigit(Peek(1)))
            {
                isFloat = true;
                Advance();
                while (!IsAtEnd && IsAsciiDigit(Current))
                {
                    Advance();
                }
            }

            var text = _source[startPos.._position];

            if (isFloat)
            {
                var value = double.Parse(text, CultureInfo.InvariantCulture);
                _tokens.Add(new Token(TokenType.FloatLiteral, text, value, start));
                return;
            }

            try
            {
                var value = int.Parse(text, CultureInfo.InvariantCulture);
                _tokens.Add(new Token(TokenType.IntLiteral, text, value, start));
            }
            catch (OverflowException)
            {
                _diagnostics.Add(new Diagnostic($"Integer literal '{text}' is out of range", start));
                _tokens.Add(new Token(TokenType.IntLiteral, text, 0, start));
            }
        }

        private void ScanIdentifier(SourceLocation start)
        {
            var startPos = _position - 1;

            while (!IsAtEnd && IsIdentifierPart(Current))
            {
                Advance();
            }

            var text = _source[startPos.._position];

            if (Keywords.TryGetKeyword(text, out var keywordType))
            {
                object? value = keywordType switch
                {
                    TokenType.True => true,
                    TokenType.False => false,
                    _ => null,
                };
                _tokens.Add(new Token(keywordType, text, value, start));
            }
            else
            {
                _tokens.Add(new Token(TokenType.Identifier, text, null, start));
            }
        }

        private void ScanString(SourceLocation start)
        {
            var sb = new StringBuilder();

            while (!IsAtEnd && Current != '"')
            {
                var c = Advance();
                if (c != '\\')
                {
                    sb.Append(c);
                    continue;
                }

                if (IsAtEnd)
                {
                    break;
                }

                sb.Append(DecodeEscape(Advance(), start));
            }

            if (IsAtEnd)
            {
                _diagnostics.Add(new Diagnostic("Unterminated string literal", start));
                var partial = sb.ToString();
                _tokens.Add(new Token(TokenType.StringLiteral, partial, partial, start));
                return;
            }

            Advance(); // closing quote
            var value = sb.ToString();
            _tokens.Add(new Token(TokenType.StringLiteral, value, value, start));
        }

        private void ScanChar(SourceLocation start)
        {
            if (IsAtEnd)
            {
                _diagnostics.Add(new Diagnostic("Unterminated char literal", start));
                return;
            }

            var c = Advance();
            if (c == '\\')
            {
                if (IsAtEnd)
                {
                    _diagnostics.Add(new Diagnostic("Unterminated char literal", start));
                    return;
                }
                c = DecodeEscape(Advance(), start);
            }

            if (IsAtEnd || Current != '\'')
            {
                _diagnostics.Add(new Diagnostic("Unterminated char literal", start));
                return;
            }

            Advance(); // closing quote
            _tokens.Add(new Token(TokenType.CharLiteral, c.ToString(), c, start));
        }

        private char DecodeEscape(char escape, SourceLocation start)
        {
            switch (escape)
            {
                case 'n':
                    return '\n';
                case 't':
                    return '\t';
                case '"':
                    return '"';
                case '\'':
                    return '\'';
                case '\\':
                    return '\\';
                default:
                    _diagnostics.Add(new Diagnostic($"Unknown escape sequence '\\{escape}'", start));
                    return escape;
            }
        }

        private static bool IsAsciiDigit(char c) => c is >= '0' and <= '9';

        private static bool IsIdentifierStart(char c) => c is (>= 'a' and <= 'z') or (>= 'A' and <= 'Z');

        private static bool IsIdentifierPart(char c) => IsIdentifierStart(c) || IsAsciiDigit(c) || c == '_';
    }
}
