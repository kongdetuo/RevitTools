using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ScriptPad.Editor;
using ScriptPad.Roslyn;
using ReactiveUI;

namespace ScriptPad
{
    /// <summary>
    /// CodeEditor.xaml 的交互逻辑
    /// </summary>
    public partial class CodeEditor : UserControl
    {
        private CompletionWindow completionWindow;
        private CancellationTokenSource completionCancellation;
        private TextMarkerService markerService;
        readonly ReactiveUI.IReactiveCommand command;

        public TextContainer Container { get; private set; }

        private static int script;

        public CsScript Script;

        public CodeEditor(string path = null)
        {
            InitializeComponent();





            command = ReactiveUI.ReactiveCommand.Create(() =>
            {
                return 1;
            });

            // 需要提升效率, 暂时不用
            codeEditor.TextArea.TextEntering += textEditor_TextArea_TextEntering;
            codeEditor.TextArea.TextEntered += textEditor_TextArea_TextEntered;
            codeEditor.TextChanged += CodeEditor_TextChanged;

            if (string.IsNullOrEmpty(path))
            {
                script++;
                Script = new CsScript("script" + script, ScriptGlobals.templateScript);
            }
            else
            {
                Script = CsScript.CreateFromFile(path);
            }

            this.codeEditor.Text = Script.Text;
            ICSharpCode.AvalonEdit.Search.SearchPanel.Install(codeEditor);

            codeEditor.TextArea.IndentationStrategy = new CSIndentationStrategy();

            // 这个也有效率问题
            //var csFoldingStrategy = new CSharpFoldingStrategy();
            //var foldingManager = FoldingManager.Install(codeEditor.TextArea);

            //DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            //foldingUpdateTimer.Interval = TimeSpan.FromSeconds(1);
            //foldingUpdateTimer.Tick += (o, e) =>
            //{
            //    csFoldingStrategy.UpdateFoldings(foldingManager, codeEditor.Document);
            //};
            //foldingUpdateTimer.Start();

            // 需要提升效率
            markerService = new TextMarkerService(codeEditor, this.Script);

            this.Container = new TextContainer(this.codeEditor.Document);
        }

        /// <summary>
        /// 关闭代码编辑窗口
        /// </summary>
        internal void Close()
        {
            if (Script.IsChanged)
            {
                var result = MessageBox.Show("文件已修改, 是否保存?", "保存", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.OK)
                {
                    Script.Save();
                }
                if (result == MessageBoxResult.Cancel)
                {
                    throw new TaskCanceledException();
                }
            }
        }

        private void CodeEditor_TextChanged(object sender, EventArgs e)
        {
            Script.UpdateText(codeEditor.Text);
        }

        private void textEditor_TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!IsAllowedLanguageLetter(e.Text[0]))
                {
                    completionWindow.CompletionList.RequestInsertion(e);

                }
            }
        }

        private async void textEditor_TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {

            try
            {
                char? triggerChar = e.Text.FirstOrDefault();

                completionCancellation = new CancellationTokenSource();
                var position = codeEditor.CaretOffset;
                var cancellationToken = completionCancellation.Token;

                var isTrigger = Service.ScriptCompletionService.IsTrigger(Script.ID, this.Container.CurrentText, position, triggerChar);
                if (completionWindow == null && (triggerChar == null || triggerChar == '.' || IsAllowedLanguageLetter(triggerChar.Value)))
                {
                    var list = await Service.ScriptCompletionService.GetCompletionsAsync(Script.ID, position);
                    if (!list.Any())
                    {
                        return;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    completionWindow = new CompletionWindow(codeEditor.TextArea)
                    {
                        WindowStyle = WindowStyle.None,
                        AllowsTransparency = true,
                        MaxWidth = 340,
                        Width = 340,
                        MaxHeight = 206,
                        Height = 206
                    };
                    foreach (var item in list)
                    {
                        completionWindow.CompletionList.CompletionData.Add(item);
                    }

                    if (triggerChar == null || IsAllowedLanguageLetter(triggerChar.Value))
                    {
                        var word = GetWord(position);
                        completionWindow.StartOffset = word.Item1;
                        completionWindow.CompletionList.SelectItem(word.Item2);
                    }
                    completionWindow.Show();
                    completionWindow.Closed += (s2, e2) =>
                    {
                        completionWindow = null;
                    };
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private Tuple<int, string> GetWord(int position)
        {
            var wordStart = TextUtilities.GetNextCaretPosition(codeEditor.TextArea.Document, position, LogicalDirection.Backward, CaretPositioningMode.WordStart);
            var text = codeEditor.TextArea.Document.GetText(wordStart, position - wordStart);
            return new Tuple<int, string>(wordStart, text);
        }

        private static bool IsAllowedLanguageLetter(char character)
        {
            return TextUtilities.GetCharacterClass(character) == CharacterClass.IdentifierPart;
        }

        private void OpenFile_btn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "C# 脚本文件(*.csx)|*.csx"
            };
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Script = CsScript.CreateFromFile(openFile.FileName);
                this.codeEditor.Text = Script.Text;
                this.markerService.Script = Script;
            }
        }


        private async void formatBtn_Click(object sender, RoutedEventArgs e)
        {
            var changes = await Script.Format();
            if (changes.Any())
            {
                changes = changes.Reverse();
                codeEditor.Document.BeginUpdate();
                foreach (var item in changes)
                {
                    codeEditor.Document.Replace(item.Span.Start, item.Span.Length, item.NewText, OffsetChangeMappingType.RemoveAndInsert);
                }
                codeEditor.Document.EndUpdate();
            }
        }

        private async void runBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                flowDocument.Blocks.Clear();
                flowDocument.Blocks.Add(new Paragraph());
                Console.SetOut(new DelegateTextWriter((flowDocument.Blocks.First() as Paragraph).Inlines.Add));

                await ScriptRunner.Run(Script);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void SaveFile_btn_Click(object sender, RoutedEventArgs e)
        {
            Script.Save();
        }

        private void CommentBtn_Click(object sender, RoutedEventArgs e)
        {
            var document = codeEditor.Document;
            var startLine = document.GetLineByOffset(codeEditor.SelectionStart);
            var endLine = document.GetLineByOffset(codeEditor.SelectionStart + codeEditor.SelectionLength);

            document.BeginUpdate();
            var line = startLine;
            while (line.LineNumber <= endLine.LineNumber)
            {
                var whitespace = TextUtilities.GetLeadingWhitespace(document, line);
                if (line.Length > whitespace.Length)
                {
                    var text = document.GetText(whitespace) + "//";
                    document.Replace(whitespace.Offset, whitespace.Length, text, OffsetChangeMappingType.RemoveAndInsert);
                }
                line = line.NextLine;
            }
            document.EndUpdate();
        }

        private void UnCommentBtn_Click(object sender, RoutedEventArgs e)
        {
            var document = codeEditor.Document;
            var startLine = document.GetLineByOffset(codeEditor.SelectionStart);
            var endLine = document.GetLineByOffset(codeEditor.SelectionStart + codeEditor.SelectionLength);

            document.BeginUpdate();
            var line = startLine;
            while (line.LineNumber <= endLine.LineNumber)
            {
                var whitespace = TextUtilities.GetLeadingWhitespace(document, line);
                if (line.Length > whitespace.Length + 2)
                {
                    var text = document.GetText(whitespace.EndOffset, 2);
                    if (text == "//")
                        document.Remove(whitespace.EndOffset, 2);
                }
                line = line.NextLine;
            }
            document.EndUpdate();
        }

        private void Reference_Click(object sender, RoutedEventArgs e)
        {
            new ReferenceWindow(this.Script).ShowDialog();
            
        }
    }
}