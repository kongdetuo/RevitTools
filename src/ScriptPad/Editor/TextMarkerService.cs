using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.CodeAnalysis;
using ScriptPad.Roslyn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ScriptPad.Editor
{
    public class TextMarkerService : IBackgroundRenderer
    {
        private readonly TextEditor textEditor;

        public CsScript Script { get; set; }

        private readonly TextSegmentCollection<TextMarker> markers;
        private ToolTip toolTip;

        DispatcherTimer Timer = new DispatcherTimer();

        public TextMarkerService(TextEditor textEditor, CsScript script)
        {
            this.textEditor = textEditor;
            this.Script = script;


            this.markers = new TextSegmentCollection<TextMarker>(textEditor.Document);

            TextView textView = textEditor.TextArea.TextView;
            textView.BackgroundRenderers.Add(this);

            textView.MouseHover += TextViewMouseHover;
            textView.MouseHoverStopped += TextViewMouseHoverStopped;
            textView.VisualLinesChanged += TextViewVisualLinesChanged;

            Timer.Interval = TimeSpan.FromSeconds(2);
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            var document = textEditor.Document;

            Clear();
            var diagnostics = await Script.GetDiagnostics();

            var listItems = diagnostics
                .Where(x => x.Severity != DiagnosticSeverity.Hidden)
                .Select(ErrorListItem.CreateErrorListItem);

            foreach (var item in listItems)
            {
                if (item.ErrorSeverity == ErrorSeverity.Info)
                    continue;

                var startOffset = document.GetOffset(new TextLocation(item.StartLine + 1, item.StartColumn + 1));
                var endOffset = document.GetOffset(new TextLocation(item.EndLine + 1, item.EndColumn + 1));

                if (item.ErrorSeverity == ErrorSeverity.Error)
                    Create(startOffset, endOffset - startOffset, item.Description, Colors.Red);
                else
                    Create(startOffset, endOffset - startOffset, item.Description, Colors.DarkGreen);
            }
        }

        public KnownLayer Layer => KnownLayer.Selection;

        public void Create(int offset, int length, string message, Color color)
        {
            var marker = new TextMarker(offset, length, message, color);
            markers.Add(marker);
            textEditor.TextArea.TextView.Redraw(marker);
        }

        public void Clear()
        {
            var oldMarkers = markers.ToArray();
            markers.Clear();

            foreach (TextMarker m in oldMarkers)
            {
                textEditor.TextArea.TextView.Redraw(m);
            }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (!markers.Any() || !textView.VisualLinesValid)
            {
                return;
            }
            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }
            int viewStart = visualLines.First().FirstDocumentLine.Offset;
            int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
            foreach (TextMarker marker in markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                foreach (Rect rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
                {
                    Point startPoint = rect.BottomLeft;
                    Point endPoint = rect.BottomRight;

                    var pen = new Pen(new SolidColorBrush(marker.MarkerColor), 1);
                    pen.Freeze();

                    const double offset = 2.5;
                    int count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);

                    var geometry = new StreamGeometry();
                    using (StreamGeometryContext ctx = geometry.Open())
                    {
                        ctx.BeginFigure(startPoint, false, false);
                        ctx.PolyLineTo(CreatePoints(startPoint, offset, count).ToArray(), true, false);
                    }
                    geometry.Freeze();

                    drawingContext.DrawGeometry(Brushes.Transparent, pen, geometry);
                    break;
                }
            }
        }

        public void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements)
        {
        }

        private void TextViewMouseHover(object sender, MouseEventArgs e)
        {
            if (!markers.Any()) { return; }

            TextViewPosition? position = textEditor.TextArea.TextView.GetPositionFloor(
                e.GetPosition(textEditor.TextArea.TextView) + textEditor.TextArea.TextView.ScrollOffset);
            if (position.HasValue)
            {
                TextLocation logicalPosition = position.Value.Location;
                int offset = textEditor.Document.GetOffset(logicalPosition);

                var markersAtOffset = markers.FindSegmentsContaining(offset);
                TextMarker marker = markersAtOffset.LastOrDefault(m => !string.IsNullOrEmpty(m.Message));

                if (marker != null)
                {
                    if (toolTip == null)
                    {
                        toolTip = new ToolTip();
                        toolTip.Closed += (s2, e2) => toolTip = null;
                        toolTip.PlacementTarget = textEditor;
                        toolTip.Content = new TextBlock
                        {
                            Text = marker.Message,
                            TextWrapping = TextWrapping.Wrap
                        };
                        toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        private void TextViewMouseHoverStopped(object sender, MouseEventArgs e)
        {
            if (toolTip != null)
            {
                toolTip.IsOpen = false;
                e.Handled = true;
            }
        }

        private void TextViewVisualLinesChanged(object sender, EventArgs e)
        {
            //if (toolTip != null)
            //{
            //    toolTip.IsOpen = false;
            //}
        }

        private static IEnumerable<Point> CreatePoints(Point start, double offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return new Point(start.X + (i * offset), start.Y - ((i + 1) % 2 == 0 ? offset : 0));
            }
        }
    }
}