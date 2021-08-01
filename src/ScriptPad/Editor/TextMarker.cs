using ICSharpCode.AvalonEdit.Document;
using System.Windows.Media;

namespace ScriptPad.Editor
{
    public class TextMarker : TextSegment
    {
        public TextMarker(int startOffset, int length, string message, Color markerColor)
        {
            StartOffset = startOffset;
            Length = length;
            Message = message;
            MarkerColor = markerColor;
        }

        public string Message { get; }
    
        public Color MarkerColor { get; }
    }
}