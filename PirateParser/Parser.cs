using System.Runtime.CompilerServices;
using PirateLexer.Models;

namespace PirateParser;

public class Parser
{
    public List<Token> tokenList { get; set; }
    public string writePath { get; set; }
    public int indentation { get; set; }
    public Token currentToken { get; set; }

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
    public void Advance()
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
    public void Parse()
    {
        var file = File.CreateText("./output.py");

        while(currentToken != null)
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
                    WriteString(",", file);
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
                        case "not":
                            WriteString("not", file, true, true);
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
                        case "for":
                            WriteString("for", file);
                            // WriteRepository.WriteForLoop();
                            Advance();
                            continue;
                        case "to":
                            WriteString("to", file, true, true);
                            Advance();
                            continue;
                        case "step":
                            WriteString("step", file, true, true);
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
                    }
                    Advance();
                    continue;
            }
        }
        file.Close();
    }

    public void WriteString(string input, StreamWriter file, bool spaceBefore = false, bool spaceAfter = false)
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