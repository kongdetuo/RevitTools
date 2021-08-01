using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;

namespace ScriptPad.Editor
{
    public abstract class FoldingStrategy
    {
        public string startToken { get; private set; }
        public string endToken { get; private set; }

        public FoldingStrategy(string start, string end)
        {
            this.startToken = start;
            this.endToken = end;
        }

        public virtual void SetName(TextDocument document, NewFolding folding)
        {
            return;
        }

        public virtual void OnMatchStart(TextDocument textDocument, int index)
        {
            return;
        }
    }

    public class BraceFoldingStrategy : FoldingStrategy
    {
        public BraceFoldingStrategy() : base("{", "}")
        {
        }
    }

    public class RegionFoldingStrategy : FoldingStrategy
    {
        public RegionFoldingStrategy() : base("#region", "#endregion")
        {
        }
    }

    public class CSharpFoldingStrategy
    {
        private List<FoldingStrategy> foldingStrategies;

        public CSharpFoldingStrategy()
        {
            foldingStrategies = new List<FoldingStrategy>()
            {
                new BraceFoldingStrategy(),
                new RegionFoldingStrategy()
            };
        }

        public void UpdateFoldings(FoldingManager manager, TextDocument document)
        {
            int firstErrorOffset;
            IEnumerable<NewFolding> newFoldings = CreateNewFoldings(document, out firstErrorOffset);
            manager.UpdateFoldings(newFoldings, firstErrorOffset);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
        {
            firstErrorOffset = -1;
            return CreateNewFoldings(document);
        }

        /// <summary>
        /// Create <see cref="NewFolding"/>s for the specified document.
        /// </summary>
        public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
        {
            List<NewFolding> newFoldings = new List<NewFolding>();

            Stack<int> startOffsets = new Stack<int>();
            int lastNewLineOffset = 0;

            for (int i = 0; i < document.TextLength; i++)
            {
                var c = document.GetCharAt(i);
                if (Is(document.Text, i, "\r\n"))
                {
                    lastNewLineOffset++;
                    continue;
                }
                foreach (var item in foldingStrategies)
                {
                    if (Is(document.Text, i, item.startToken))
                    {
                        startOffsets.Push(i);
                        i += item.endToken.Length;
                    }
                    else if (Is(document.Text, i, item.endToken) && startOffsets.Count > 0)
                    {
                        var startOffset = startOffsets.Pop();

                        if (startOffset < i)
                        {
                            var folding = new NewFolding(startOffset, i + 1);
                            SetName(folding);
                            newFoldings.Add(folding);
                        }
                        i += item.endToken.Length;
                    }
                }
            }
            newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
            return newFoldings;
        }

        private bool Is(string source, int index, string str)
        {
            if (index + str.Length > source.Length)
            {
                return false;
            }

            for (var i = 0; i < str.Length; i++)
            {
                if (source[index + i] != str[i])
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual void SetName(NewFolding folding)
        {
            return;
        }
    }
}