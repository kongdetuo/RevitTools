using System;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.CodeAnalysis.Text;


namespace ScriptPad.Editor
{
    public class TextContainer : SourceTextContainer , IDisposable
    {
        public TextContainer(TextDocument document)
        {
            this.TextDocument = document;
            this.currentText = SourceText.From(document.GetText(0, document.TextLength));
            document.Changed += Document_Changed;
        }

        private void Document_Changed(object sender, DocumentChangeEventArgs e)
        {
            var old = currentText;

            var remove = new TextChange(new TextSpan(e.Offset, e.RemovalLength), "");
            currentText = currentText.WithChanges(remove);
            var insert = new TextChange(new TextSpan(e.Offset, 0), e.InsertedText.Text);
            currentText = currentText.WithChanges(insert);

            TextChanged?.Invoke(this, new Microsoft.CodeAnalysis.Text.TextChangeEventArgs(old, currentText, remove, insert));
        }

        public TextDocument TextDocument { get; private set; }

        private SourceText currentText;
        public override SourceText CurrentText => currentText;

        public override event EventHandler<Microsoft.CodeAnalysis.Text.TextChangeEventArgs> TextChanged;

        public void Dispose()
        {
            this.TextDocument.Changed -= Document_Changed;
        }
    }

}