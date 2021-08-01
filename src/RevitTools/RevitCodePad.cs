using Autodesk.Revit.UI;
using System;
using System.Windows.Media.Imaging;
namespace RevitCodePad
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class RevitCodePad : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            var assembly = typeof(RevitCodePad).Assembly;
            var last = assembly.Location.LastIndexOf('\\');
            var dir = assembly.Location.Substring(0, last + 1);


            var ribbonPanel = application.CreateRibbonPanel("RevitScriptPad");
            var uri = new Uri(dir + "Images\\Csharp.png");
            var limg = new BitmapImage(uri);

            var spiltButtonData = new SplitButtonData("RevitScriptPad", "open Window");
            var splitButton = ribbonPanel.AddItem(spiltButtonData) as SplitButton;
            splitButton.LargeImage = limg;

            var pushButton1 = splitButton.AddPushButton(new PushButtonData("OpenScriptPad", "打开脚本窗口", assembly.Location, "RevitCodePad.OpenScriptPadCommand")) as PushButton;
            pushButton1.LargeImage = limg;

            var pushButton2 = splitButton.AddPushButton(new PushButtonData("OpenNoModal", "打开非模态窗口", assembly.Location, "RevitCodePad.OpenNoModalCommand")) as PushButton;
            pushButton2.LargeImage = limg;

            var pushButton3 = splitButton.AddPushButton(new PushButtonData("HistoryList", "最近使用的脚本", assembly.Location, "RevitCodePad.ShowHistoryListCommand")) as PushButton;
            pushButton3.LargeImage = limg;

            var pushButton4 = splitButton.AddPushButton(new PushButtonData("ShowElement", "显示3D图像", assembly.Location, "RevitCodePad.Show3DElementCommand")) as PushButton;
            pushButton4.LargeImage = limg;

            var pushButton5 = splitButton.AddPushButton(new PushButtonData("ShowLine", "显示线", assembly.Location, "RevitCodePad.ShowLineCommand")) as PushButton;
            pushButton5.LargeImage = limg;
            return Result.Succeeded;
        }
    }
}
