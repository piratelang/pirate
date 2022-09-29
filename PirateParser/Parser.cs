using System.Runtime.CompilerServices;
using PirateLexer.Models;

namespace PirateParser;

public class Parser
{
    public static List<Token> tokenList { get; set; }
    public string writePath { get; set; }
    public int indentation { get; set; }
    public static Token currentToken { get; set; }

    public Parser(List<Token> TokenList, string WritePath = null)
    {
        tokenList = TokenList;
        currentToken = tokenList[0];
        writePath = WritePath;
        if (WritePath == null)
        {
            writePath = "./Pirate/output.py";
        }
    }
    public static void Advance()
    {
        var index = tokenList.IndexOf(currentToken) + 1;
        if (tokenList.IndexOf(currentToken) + 2 >= tokenList.Count())
        {
            currentToken = null;
        }
        else
        {
            currentToken = tokenList[index];
        }
    }
    public bool Parse(string location)
    {
        bool exists = System.IO.Directory.Exists(location);
        if (!exists)
            System.IO.Directory.CreateDirectory(location);

        var file = File.CreateText("./" + location + "/output.py");

        var writeRepository = new WriteRepository();

        while (currentToken != null)
        {
            var indentString = string.Empty;
            switch (currentToken.tokenType)
            {
                case TokenType.LEFTCURLYBRACE:
                    indentation += 1;
                    indentString = String.Concat(Enumerable.Repeat("    ", indentation));
                    WriteString(":\n" + indentString, file);
                    Advance();
                    continue;
                case TokenType.RIGHTCURLYBRACE:
                    indentation -= 1;
                    indentString = String.Concat(Enumerable.Repeat("    ", indentation));
                    WriteString("\n" + indentString, file);
                    Advance();
                    continue;

                case TokenType.LEFTBRACKET:
                    WriteString("[", file, false, false);
                    Advance();
                    continue;
                case TokenType.RIGHTBRACKET:
                    WriteString("]", file, false, false);
                    Advance();
                    continue;

                case TokenType.SEMICOLON:
                    indentString = String.Concat(Enumerable.Repeat("    ", indentation));
                    WriteString("\n" + indentString, file);
                    Advance();
                    continue;

                case TokenType.INT:
                    WriteString(currentToken.value.ToString(), file);
                    Advance();
                    continue;
                case TokenType.FLOAT:
                    WriteString(currentToken.value.ToString(), file);
                    Advance();
                    continue;
                case TokenType.STRING:
                    WriteString('"' + currentToken.value.ToString() + '"', file);
                    Advance();
                    continue;
                case TokenType.IDENTIFIER:
                    WriteString(currentToken.value.ToString(), file);
                    Advance();
                    continue;

                case TokenType.PLUS:
                    WriteString("+", file, true, true);
                    Advance();
                    continue;
                case TokenType.PLUSEQUALS:
                    WriteString("+=", file, true, true);
                    Advance();
                    continue;
                case TokenType.MINUS:
                    WriteString("-", file, true, true);
                    Advance();
                    continue;
                case TokenType.MULTIPLY:
                    WriteString("*", file, true, true);
                    Advance();
                    continue;
                case TokenType.DIVIDE:
                    WriteString("/", file, true, true);
                    Advance();
                    continue;
                case TokenType.EQUALS:
                    WriteString("=", file, true, true);
                    Advance();
                    continue;
                case TokenType.LEFTPARENTHESES:
                    WriteString("(", file);
                    Advance();
                    continue;
                case TokenType.RIGHTPARENTHESES:
                    WriteString(")", file);
                    Advance();
                    continue;
                case TokenType.POWER:
                    WriteString("**", file, true, true);
                    Advance();
                    continue;
                case TokenType.DOUBLEEQUALS:
                    WriteString("==", file, true, true);
                    Advance();
                    continue;
                case TokenType.NOTEQUALS:
                    WriteString("!=", file, true, true);
                    Advance();
                    continue;
                case TokenType.LESSTHAN:
                    WriteString("<", file, true, true);
                    Advance();
                    continue;
                case TokenType.GREATERHAN:
                    WriteString(">", file, true, true);
                    Advance();
                    continue;
                case TokenType.LESSTHANEQUALS:
                    WriteString("<=", file, true, true);
                    Advance();
                    continue;
                case TokenType.GREATERTHANEQUALS:
                    WriteString(">=", file, true, true);
                    Advance();
                    continue;
                case TokenType.COMMA:
                    WriteString(",", file, false, true);
                    Advance();
                    continue;
                case TokenType.DOT:
                    WriteString(".", file, false, false);
                    Advance();
                    continue;
                case TokenType.DOLLAR:
                    WriteString("f", file, false, false);
                    Advance();
                    continue;

                case TokenType.KEYWORD:
                    switch (currentToken.value)
                    {
                        case "var":
                            Advance();
                            continue;
                        case "and":
                            WriteString("and", file, true, true);
                            Advance();
                            continue;
                        case "or":
                            WriteString("or", file, true, true);
                            Advance();
                            continue;
                        case "if":
                            WriteString("if", file);
                            Advance();
                            continue;
                        case "elif":
                            WriteString("elif", file);
                            Advance();
                            continue;
                        case "else":
                            WriteString("else", file);
                            Advance();
                            continue;
                        case "to":
                            WriteString("to", file, true, true);
                            Advance();
                            continue;
                        case "while":
                            WriteString("while", file);
                            Advance();
                            continue;
                        case "func":
                            WriteString("def", file, false, true);
                            Advance();
                            continue;
                        case "import":
                            WriteString("import", file, false, true);
                            Advance();
                            continue;
                        case "class":
                            WriteString("class", file, false, true);
                            Advance();
                            continue;
                        case "for":
                            writeRepository.WriteForLoop(file);
                            continue;
                        case "foreach":
                            writeRepository.WriteForeachLoop(file);
                            continue;
                        case "in":
                            WriteString("in", file, true, true);
                            Advance();
                            continue;
                    }
                    Advance();
                    continue;
                default:
                    throw new Exception("Token not found: " + currentToken.tokenType);
            }
        }
        file.Close();
        return true;
    }

    public static void WriteString(string input, StreamWriter file, bool spaceBefore = false, bool spaceAfter = false)
    {
        if (spaceBefore)
        {
            file.Write(" ");
        }
        foreach (var item in input)
        {
            file.Write(item);
        }
        if (spaceAfter)
        {
            file.Write(" ");
        }
    }


}