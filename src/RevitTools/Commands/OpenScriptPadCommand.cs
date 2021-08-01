using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ScriptPad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RevitCodePad
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class OpenScriptPadCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var assembly = typeof(RevitCodePad).Assembly;
            var dir = Path.GetDirectoryName(assembly.Location);

            ScriptGlobals.GlobalObject = new RevitGlobals()
            {
                commandData = commandData,
                uiApp = commandData.Application
            };

            ScriptGlobals.StartScript = "main(uiApp)";
            ScriptGlobals.templateScript = File.ReadAllText(Path.Combine(dir, "template.txt"));
            ScriptGlobals.InitAssemblies = new List<Assembly>();
            ScriptGlobals.InitAssemblies.Add(typeof(object).Assembly);          // mscorelib
            ScriptGlobals.InitAssemblies.Add(typeof(Uri).Assembly);             // System
            ScriptGlobals.InitAssemblies.Add(typeof(Enumerable).Assembly);      // System.Core
            ScriptGlobals.InitAssemblies.Add(typeof(Element).Assembly);         // Autodesk.Revit.DB
            ScriptGlobals.InitAssemblies.Add(typeof(UIApplication).Assembly);   // Autodesk.Revit.UI

            var window = new MainWindow();
            new System.Windows.Interop.WindowInteropHelper(window).Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
            window.ShowDialog();
            return Result.Succeeded;
        }
    }
}