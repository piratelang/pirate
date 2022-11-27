using PirateParser.Node.Interfaces;

namespace PirateParser.Node;

public class CommentNode : INode
{
    public string Text { get; set; }

    public CommentNode(string text)
    {
        Text = text;
    }

    public override string ToString()
    {
        return $"// {Text}";
    }

    public bool IsValid()
    {
        return true;
    }
}