using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitTools.DrawTool;
using System;
using System.Runtime.InteropServices;

namespace RevitCodePad
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class ShowLineCommand : IExternalCommand
    {
        private static DrawForm DrawForm;

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern int SetForegroundWindow(int hwnd);

        public Result Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            if (DrawForm == null)
            {
                DrawForm = new DrawForm(commandData.Application);
                DrawForm.Show(new rvtHandle());
                SetForegroundWindow(new rvtHandle().Handle.ToInt32());
            }
            else
            {
                if (DrawForm.Visible)
                {
                    DrawForm.Hide();
                }
                else
                {
                    DrawForm.Show(new rvtHandle());
                }
            }

            return Result.Succeeded;
        }
    }

    public class rvtHandle : System.Windows.Forms.IWin32Window
    {
        public IntPtr Handle
        {
            get { return Autodesk.Windows.ComponentManager.ApplicationWindow; }
        }
    }
}