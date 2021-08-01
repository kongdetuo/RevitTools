using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Autodesk.Revit.UI;
using System.IO;
using PropertyChanged;
using ScriptPad.Roslyn;
using System.Threading.Tasks;

namespace RevitCodePad.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class HistoryViewModel
    {
        public HistoryViewModel()
        {
            this.RunCommand = new DelegateCommand(RunScript);

            var path = Path.Combine(Environment.CurrentDirectory, "Scripts");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var items = new DirectoryInfo(path).GetFiles()
                .Select(p => new HistoryItem()
                {
                    Title = p.Name,
                    FullName = p.FullName
                });
            this.HistoryItems = new ObservableCollection<HistoryItem>(items);
            this.SelectedItem = this.HistoryItems.FirstOrDefault();

        }
        public ExternalCommandData CommandData { get; private set; }

        public ObservableCollection<HistoryItem> HistoryItems { get; set; }

        public HistoryItem SelectedItem { get; set; }

        public Visibility WindowVisibility { get; set; }
        public DelegateCommand RunCommand { get; }


        private async void RunScript(object obj)
        {
            WindowVisibility = Visibility.Collapsed;
            try
            {
                if (this.SelectedItem != null)
                {
                    var script = ScriptPad.CsScript.CreateFromFile(this.SelectedItem.FullName);
                    await ScriptRunner.Run(script);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
