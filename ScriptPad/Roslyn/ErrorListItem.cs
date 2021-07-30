using Microsoft.CodeAnalysis;

namespace ScriptPad.Roslyn
{
    /// <summary>
    /// 错误项
    /// </summary>
    public class ErrorListItem
    {
        public ErrorListItem(ErrorSeverity errorSeverity, string description, int startLine, int startColumn, int endLine, int endColumn)
        {
            ErrorSeverity = errorSeverity;
            Description = description;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
        }

        /// <summary>
        /// 严重性
        /// </summary>
        public ErrorSeverity ErrorSeverity { get; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 起始行
        /// </summary>
        public int StartLine { get; }

        /// <summary>
        /// 起始列
        /// </summary>
        public int StartColumn { get; }

        /// <summary>
        /// 结束行
        /// </summary>
        public int EndLine { get; }

        /// <summary>
        /// 结束列
        /// </summary>
        public int EndColumn { get; }

        public static ErrorListItem CreateErrorListItem(Diagnostic diagnostic)
        {
            var mappedSpan = diagnostic.Location.GetMappedLineSpan();
            ErrorSeverity errorSeverity;
            if (diagnostic.Severity == DiagnosticSeverity.Error)
            {
                errorSeverity = ErrorSeverity.Error;
            }
            else if (diagnostic.Severity == DiagnosticSeverity.Warning)
            {
                errorSeverity = ErrorSeverity.Warning;
            }
            else
            {
                errorSeverity = ErrorSeverity.Info;
            }
            return new ErrorListItem(errorSeverity, diagnostic.GetMessage(), mappedSpan.Span.Start.Line, mappedSpan.Span.Start.Character,
                mappedSpan.Span.End.Line, mappedSpan.Span.End.Character);
        }
    }
}