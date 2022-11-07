namespace PirateLexer;

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
}
