using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

/// <summary>
/// A comment with a list of tokens.
/// </summary>
public class CommentNode : INode
{
    public List<Token> Comment { get; set; }

    public CommentNode(List<Token> comment)
    {
        Comment = comment;
    }

    public override string ToString()
    {
        var comment = string.Empty;
        foreach (var token in Comment)
        {
            comment += token.ToString();
        }
        return $"// {comment} ";
    }

    public bool IsValid()
    {
        return true;
    }
}