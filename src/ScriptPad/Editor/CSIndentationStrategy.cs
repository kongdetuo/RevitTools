using ICSharpCode.AvalonEdit.Document;
using System;

namespace ScriptPad
{
    class CSIndentationStrategy : ICSharpCode.AvalonEdit.Indentation.IIndentationStrategy
    {

        string IndentationString = "    ";

        public void IndentLine(ICSharpCode.AvalonEdit.Document.TextDocument document, DocumentLine line)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (line == null)
                throw new ArgumentNullException("line");

            DocumentLine previousLine = line.PreviousLine;
            if (previousLine != null)
            {
                ISegment indentationSegment = TextUtilities.GetWhitespaceAfter(document, previousLine.Offset);
                string indentation = document.GetText(indentationSegment);

                if (previousLine.EndOffset > 0)
                {
                    var c = document.GetCharAt(previousLine.EndOffset - 1);
                    if (c == '{')
                    {
                        //indentation += IndentationString;
                        indentationSegment = TextUtilities.GetWhitespaceAfter(document, line.Offset);
                        document.Replace(indentationSegment.Offset, indentationSegment.Length, indentation, OffsetChangeMappingType.RemoveAndInsert);
                    }
                    else
                    {
                        indentationSegment = TextUtilities.GetWhitespaceAfter(document, line.Offset);
                        document.Replace(indentationSegment.Offset, indentationSegment.Length, indentation, OffsetChangeMappingType.RemoveAndInsert);
                    }

    
                }


            }

        }

        public void IndentLines(ICSharpCode.AvalonEdit.Document.TextDocument document, int beginLine, int endLine)
        {

        }
    }
}