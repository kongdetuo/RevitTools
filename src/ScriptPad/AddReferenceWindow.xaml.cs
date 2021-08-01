using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ScriptPad
{
    /// <summary>
    /// AddReferenceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddReferenceWindow : Window
    {
        private CsScript script;
        private List<string> list;

        public AddReferenceWindow(CsScript script)
        {
            this.script = script;
            InitializeComponent();

            this.list = script.GetReferences().OfType<Microsoft.CodeAnalysis.PortableExecutableReference>().Select(p => p.FilePath).ToList();

            var path = typeof(object).Assembly.Location;
            path = Path.GetDirectoryName(path);

            var dir = new DirectoryInfo(path);

            var dic = dir.GetFiles().Where(p => p.Extension == ".dll").ToDictionary(p => p.Name);

            foreach (var item in dic.Keys.OrderBy(p => p.Substring(0, p.Length - 4)))
            {
                var cb = new CheckBox();
                cb.Content = item;
                cb.IsThreeState = false;
                cb.ToolTip = dic[item].FullName;

                if (list.Contains(dic[item].FullName))
                    cb.IsChecked = true;

                ReferenceList.Items.Add(cb);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in ReferenceList.Items)
            {
                var cb = item as CheckBox;
                var path = cb.ToolTip as string;
                
                if(list.Contains(path) && !cb.IsChecked.Value)
                {
                    this.script.RemoveReference(path);
                }
                if(!list.Contains(path) && cb.IsChecked.Value)
                {
                    this.script.AddReference(path);
                }
            }
            this.Close();
        }
    }
}
