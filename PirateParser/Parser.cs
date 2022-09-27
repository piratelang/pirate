using PirateLexer.Models;

namespace PirateParser;

public class Parser
{
    public List<Token> tokenList { get; set; }
    public string writePath { get; set; }

    public Parser(List<Token> TokenList, string WritePath)
    {
        tokenList = TokenList;
        writePath = WritePath;
    }

    public void Parse()
    {
        var indentation = 0;

        foreach (var token in tokenList)
        {
            
        }
    }
}