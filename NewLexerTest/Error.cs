using PirateLexer.Models;

namespace PirateLexer
{
    public class Error
    {
        public Position positionStart { get; set; }
        public Position positionEnd { get; set; }
        public string errorName { get; set; }
        public string details { get; set; }

        public Error(Position PositionStart, Position PositionEnd, string ErrorName, string Details)
        {
            positionStart = PositionStart;
            positionEnd = PositionEnd;
            errorName = ErrorName;
            details = Details;
        }

        public string AsString()
        {
            var result = $"{errorName}: {details}";
            result += $"File: {positionStart.fileName}, line {positionStart.lineNumber + 1}";
            result += $"\n\n {positionStart.fileText}"; //Here be dragons, to convert string_with_arrows.py
            return result;
        }
    }
}