using System.Reactive.Linq;
using System;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace ScriptPad.Editor
{
    static class CodeEditorReactiveExtensions
    {
        public static IObservable<TextCompositionEventArgs> GetTextEnterings(this TextArea textArea)
        {
            return Observable.FromEventPattern<TextCompositionEventArgs>(textArea, nameof(textArea.TextEntering))
                .Select(p => p.EventArgs);
        }

        public static IObservable<TextCompositionEventArgs> GetTextEntereds(this TextArea textArea)
        {
            return Observable.FromEventPattern<TextCompositionEventArgs>(textArea, nameof(textArea.TextEntered))
                .Select(p => p.EventArgs);
        }

        public static IObservable<EventArgs> GetTextChangeds(this TextEditor editor)
        {
            return Observable.FromEventPattern<EventArgs>(editor, nameof(editor.TextChanged))
                .Select(p => p.EventArgs);
        }
    }
}