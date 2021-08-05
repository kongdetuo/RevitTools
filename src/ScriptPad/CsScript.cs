using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScriptPad.Roslyn;
using Microsoft.CodeAnalysis.Text;

namespace ScriptPad
{
    public class CsScript
    {
        /// <summary>
        /// 是否更改过
        /// </summary>
        public bool IsChanged { get; private set; }

        public readonly DocumentId ID;
        public string Name { get; set; }
        public string Path { get; set; }

        public string Text { get; private set; }

        public IReadOnlyCollection<PortableExecutableReference> References => Workspace.GetReferences(this.ID).OfType<PortableExecutableReference>().ToList();

        /// <summary>
        /// 创建具有指定名字和内容的脚本对象
        /// </summary>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public CsScript(string name, string text)
        {
            this.Name = name;
            this.Text = text;
            if (text == null)
                this.Text = "";

            ID = Workspace.AddProjectWithDocument(name, this.Text);
            IsChanged = false;
        }

        private ScriptingWorkspace Workspace => ScriptingWorkspace.GetInstance();

        /// <summary>
        /// 从文件创建 Script 对象
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static CsScript CreateFromFile(string path)
        {
            var info = new FileInfo(path);
            var text = File.ReadAllLines(path);

            var i = 0;
            List<string> references = new List<string>();
            for (; i < text.Length; i++)
            {
                if (text[i].StartsWith("#r "))
                {
                    references.Add(text[i].Trim().Substring(4, text[i].Length - 5)); // #r "
                }
                else
                {
                    break;
                }
            }
            var code = new StringBuilder();
            for (; i < text.Length; i++)
            {
                code.Append(text[i]);
                code.Append("\r\n");
            }
            var script = new CsScript(info.Name, code.ToString());
            foreach (var item in references)
            {
                script.AddReference(item);
            }
            script.Path = path;

            return script;
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        /// <param name="path">文件路径</param>
        public void AddReference(string path)
        {
            Workspace.AddReference(path, this.ID);
        }

        /// <summary>
        /// 删除引用
        /// </summary>
        /// <param name="path"></param>
        public void RemoveReference(string path)
        {
            Workspace.RemoveReference(path, this.ID);
        }

        /// <summary>
        /// 整理脚本
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Microsoft.CodeAnalysis.Text.TextChange>> Format()
        {
            var document = Workspace.GetDocument(ID);
            var formattedDocument = await Microsoft.CodeAnalysis.Formatting.Formatter.FormatAsync(document).ConfigureAwait(false);
            return await formattedDocument.GetTextChangesAsync(document);
        }

        /// <summary>
        /// 获取诊断信息
        /// </summary>
        /// <returns></returns>
        public async Task<ImmutableArray<Diagnostic>> GetDiagnostics()
        {
            var project = Workspace.GetProject(ID);
            var compilation = await project.GetCompilationAsync();
            return compilation.GetDiagnostics();
        }

        /// <summary>
        /// 获取脚本内容
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetScriptText()
        {
            var text = await Workspace.GetDocument(ID).GetTextAsync();
            return text.ToString();
        }

        /// <summary>
        /// 获取脚本内容
        /// </summary>
        /// <returns></returns>
        public string ToCode()
        {
            var code = new StringBuilder();
            foreach (var item in References.Select(p => p.FilePath).Except(ScriptGlobals.InitAssemblies.Select(p => p.Location)))
            {
                var refstr = $"#r \"{item}\"\r\n";
                code.Append(refstr);
            }

            code.Append(GetScriptText().Result);
            return code.ToString();
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {
            if (string.IsNullOrEmpty(Path))
            {
                var dialog = new SaveFileDialog()
                {
                    Filter = "C# Script|*.csx",
                    Title = "Save File"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dialog.FileName, ToCode());
                    this.Path = dialog.FileName;
                }
            }
            else
            {
                File.WriteAllText(Path, ToCode());
            }
            IsChanged = false;
        }

        public void UpdateText(Document document)
        {
            IsChanged = true;
            Workspace.TryApplyChanges(document.Project.Solution);
        }

        public void UpdateText(SourceText sourceText)
        {
            Workspace.UpdateText(ID,sourceText);
        }

        internal IEnumerable<MetadataReference> GetReferences()
        {
            return Workspace.GetReferences(this.ID);
        }
    }
}