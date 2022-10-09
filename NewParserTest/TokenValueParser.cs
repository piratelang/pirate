using NewPirateLexer.Tokens;

namespace NewParserTest;

// functie: parsed Expression ( )
public class TokenValueParser:ITokenParser
{
    INode node = null;
    private List<Token> _tokens;
    private Token _currentToken;

    public TokenValueParser(List<Token> tokens, Token currentToken)
    {
        _tokens = tokens;
        _currentToken = currentToken;


    }

    public void Eveluate()
    {
        var index = _tokens.IndexOf(_currentToken);
        var LeftNode = _tokens[index];

        var secondToken = _tokens[ index++];
        if(secondToken.TokenGroup == NewPirateLexer.Enums.TokenGroup.OPERATORS)
        {
            node = new BinaryOperationNode();
        }
        if(secondToken.TokenGroup == NewPirateLexer.Enums.TokenGroup.COMPARISONOPERATORS)
        {
            node = new ComparisonOperationNode();
        }

        node.Left = LeftNode;
        node.Operator = secondToken;
    }
}
