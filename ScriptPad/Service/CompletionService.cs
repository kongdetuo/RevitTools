using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScriptPad.Service
{
    public class ScriptCompletionService
    {
        public static async Task<IEnumerable<Editor.CodeCompletionData>> GetCompletionsAsync(DocumentId id, int position)
        {
            var document = Roslyn.ScriptingWorkspace.GetInstance().GetDocument(id);
            var completionService = CompletionService.GetService(document);
            var list = await completionService.GetCompletionsAsync(document, position);

            if (list == null || !list.Items.Any())
                return new List<Editor.CodeCompletionData>();

            return list.Items.Select(p => new Editor.CodeCompletionData(id, p));
        }

        public static async Task<string> GetDescriptionAsync(DocumentId id, CompletionItem completionItem)
        {
            var document = Roslyn.ScriptingWorkspace.GetInstance().GetDocument(id);
            var completionService = CompletionService.GetService(document);
            var des = await completionService.GetDescriptionAsync(document, completionItem);
            return des.Text;
        }

        public static bool IsTrigger(DocumentId id, SourceText text, int position, char? triggerChar)
        {
            var document = Roslyn.ScriptingWorkspace.GetInstance().GetDocument(id);
            var completionService = CompletionService.GetService(document);
            if(triggerChar != null)
                return completionService.ShouldTriggerCompletion(text, position, CompletionTrigger.CreateInsertionTrigger(triggerChar.Value));
            return completionService.ShouldTriggerCompletion(text, position, CompletionTrigger.Invoke);
        }
    }
}
