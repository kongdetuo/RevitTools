using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace ScriptPad.Roslyn
{
    public class ScriptRunner
    {
        public static async Task Run(CsScript Script)
        {
            var options = ScriptOptions.Default;
            options = options.AddReferences(Script.GetReferences());
            options = options.AddReferences(ScriptGlobals.InitAssemblies);

            var script = CSharpScript.Create(await Script.GetScriptText(), options, globalsType: ScriptGlobals.GlobalObject.GetType());

            if (!string.IsNullOrWhiteSpace(ScriptGlobals.StartScript))
                script = script.ContinueWith(ScriptGlobals.StartScript, options);

            await script.RunAsync(ScriptGlobals.GlobalObject);
        }
    }

}