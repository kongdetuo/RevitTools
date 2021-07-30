using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Editing;
using Microsoft.CodeAnalysis.Tags;
using System.Linq;

namespace ScriptPad.Editor
{
    public class CodeCompletionData : ICompletionData
    {
        public CodeCompletionData(DocumentId id, Microsoft.CodeAnalysis.Completion.CompletionItem item)
        {
            this.Text = item.DisplayText;
            image = new Lazy<ImageSource>(() => ImageResource.GetImage(item.Tags));
            description = new Lazy<object>(() => Service.ScriptCompletionService.GetDescriptionAsync(id, item).Result);
            this.Content = Text;
        }

        public string Text { get; }

        public object Description { get => description.Value; }

        public object Content { get; set; }

        public ImageSource Image { get => image.Value; }

        public double Priority => 0;

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

        private Lazy<object> description;
        private Lazy<ImageSource> image;
    }

}