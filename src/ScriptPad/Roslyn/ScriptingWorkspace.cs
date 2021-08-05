using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis.Host;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using Microsoft.CodeAnalysis.Host.Mef;
using System.Composition.Hosting;

namespace ScriptPad.Roslyn
{
    internal class ScriptingWorkspace : Workspace
    {
        private ScriptingWorkspace(HostServices hostServices) : base(hostServices, WorkspaceKind.Interactive)
        {
        }

        public Document GetDocument(DocumentId id)
        {
            return CurrentSolution.GetDocument(id);
        }

        public DocumentId AddProjectWithDocument(string documentFileName, string text)
        {
            var fileName = Path.GetFileName(documentFileName);
            var name = Path.GetFileNameWithoutExtension(documentFileName);

            var projectId = ProjectId.CreateNewId();

            var references = ScriptGlobals.InitAssemblies.Distinct().Select(CreateReference).ToList();

            var projectInfo = ProjectInfo.Create(
                projectId,
                VersionStamp.Default,
                name,
                name,
                LanguageNames.CSharp,
                isSubmission: true,
                compilationOptions: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithScriptClassName(name),
                metadataReferences: references,
                parseOptions: new CSharpParseOptions(languageVersion: LanguageVersion.Latest));

            OnProjectAdded(projectInfo);

            var documentId = DocumentId.CreateNewId(projectId);
            var documentInfo = DocumentInfo.Create(
                documentId,
                fileName,
                sourceCodeKind: SourceCodeKind.Script,
                loader: TextLoader.From(TextAndVersion.Create(SourceText.From(text, Encoding.UTF8), VersionStamp.Create())));
            OnDocumentAdded(documentInfo);
            return documentId;
        }

        public Project GetProject(DocumentId id)
        {
            return CurrentSolution.GetProject(id.ProjectId);
        }

        public void RemoveProject(DocumentId id)
        {
            OnProjectRemoved(id.ProjectId);
        }

        public void UpdateText(DocumentId documentId, string text)
        {
            OnDocumentTextChanged(documentId, SourceText.From(text, Encoding.UTF8), PreservationMode.PreserveValue);
        }

        public void UpdateText(DocumentId documentid, SourceText text)
        {
            OnDocumentTextChanged(documentid, text, PreservationMode.PreserveValue);
        }

        public Task<IReadOnlyList<Diagnostic>> GetDiagnosticsAsync(DocumentId documentId, CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                var project = CurrentSolution.GetProject(documentId.ProjectId);
                var compilation = await project.GetCompilationAsync(cancellationToken);
                return (IReadOnlyList<Diagnostic>)compilation.GetDiagnostics(cancellationToken);
            }, cancellationToken);
        }

        public override bool CanApplyChange(ApplyChangesKind feature)
        {
            return base.CanApplyChange(feature);
        }

        private MetadataReference CreateReference(Assembly assembly)
        {
            return MetadataReference.CreateFromFile(assembly.Location);
        }

        public void AddReference(string path, DocumentId id)
        {
            var references = GetReferences(id).OfType<PortableExecutableReference>();
            if (references.Any(p => p.FilePath == path))
                return;

            PortableExecutableReference data = MetadataReference.CreateFromFile(path);
            var pid = GetDocument(id).Project.Id;
            OnMetadataReferenceAdded(pid, data);
        }

        public void RemoveReference(string path, DocumentId id)
        {
            var project = GetDocument(id).Project;
            var data = project.MetadataReferences.FirstOrDefault(p => (p as PortableExecutableReference).FilePath == path);

            OnMetadataReferenceRemoved(project.Id, data);
        }

        public IEnumerable<MetadataReference> GetReferences(DocumentId id)
        {
            var project = GetDocument(id).Project;
            return project.MetadataReferences;
        }

        private static Lazy<ScriptingWorkspace> instance = new Lazy<ScriptingWorkspace>(() =>
        {
            var compositionHost = new ContainerConfiguration().WithAssemblies(MefHostServices.DefaultAssemblies).CreateContainer();
            var hostService = MefHostServices.Create(compositionHost);
            return new ScriptingWorkspace(hostService);
        }, true);

        public static ScriptingWorkspace GetInstance()
        {
            return instance.Value;
        }
    }
}