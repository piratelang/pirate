using System.Security.Principal;
using PirateLexer.Models;

namespace PirateParser
{
    public class WriteRepository
    {
        public void WriteForLoop(StreamWriter file)
        {
            var localTokenList = new List<Token>() {};
            while (Parser.currentToken.tokenType != TokenType.LEFTCURLYBRACE)
            {
                localTokenList.Add(Parser.currentToken);
                Parser.Advance();
            }
            Parser.WriteString($"for {localTokenList[3].value} in range({localTokenList[5].value}, {localTokenList[7].value})", file, false, true);
            
        }
        public void WriteForeachLoop(StreamWriter file)
        {
            var localTokenList = new List<Token>() {};
            while (Parser.currentToken.tokenType != TokenType.LEFTCURLYBRACE)
            {
                localTokenList.Add(Parser.currentToken);
                Parser.Advance();
            }
            Parser.WriteString($"for {localTokenList[3].value} in {localTokenList[5].value}", file, false, true);
            
        }
    }
}