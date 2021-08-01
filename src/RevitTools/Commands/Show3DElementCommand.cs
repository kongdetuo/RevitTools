using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitCodePad
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class Show3DElementCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            var uiDoc = commandData.Application.ActiveUIDocument;
            var selectIds = uiDoc.Selection.GetElementIds();
            if (selectIds.Count == 0)
            {
                message = "没有选中的元素";
                return Result.Cancelled;
            }
            var window = new Show3DWindow(uiDoc.Document, selectIds);
            new System.Windows.Interop.WindowInteropHelper(window).Owner = Autodesk.Windows.ComponentManager.ApplicationWindow;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            window.Show();
            return Result.Succeeded;
        }
    }
}
