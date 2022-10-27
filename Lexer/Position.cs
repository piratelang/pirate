namespace Lexer;

public class Position
{
    public int index { get; set; }
    public int lineNumber { get; set; }
    public int columnNumber { get; set; }
    public string fileName { get; set; }
    public string fileText { get; set; }

    public Position(int Index, int LineNumber, int ColumnNumber, string FileName, string FileText)
    {
        index = Index;
        lineNumber = LineNumber;
        columnNumber = ColumnNumber;
        fileName = FileName;
        fileText = FileText;
    }

    public void Advance(char CurrentChar = ' ')
    {
        index += 1;
        columnNumber += 1;

        if (CurrentChar == '\n')
        {
            lineNumber += 1;
            columnNumber = 0;
        }
    }

    public Position Copy()
    {
        return new Position(index, lineNumber, columnNumber, fileName, fileText);
    }
}
