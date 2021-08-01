using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace ScriptPad
{
    /// <summary>
    /// ReferenceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ReferenceWindow : Window
    {
        private CsScript script;

        public ReferenceWindow(CsScript script)
        {
            InitializeComponent();
            this.script = script;
            ShowReferences();
        }

        private void BrowseBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog()
            {
                Filter = "dll文件|*.dll|exe文件|*.exe",
                Multiselect = true
            };
            
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (var item in openFileDialog.FileNames)
                {
                    try
                    {
                        script.AddReference(item);
                    }
                    catch(Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
                ShowReferences();
            }
        }

        private void ShowReferences()
        {
            ReferenceList.Items.Clear();
            foreach (var item in script.References)
            {
                var path = item.FilePath;
                ReferenceList.Items.Add(path);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var items = ReferenceList.SelectedItems.OfType<string>().ToList();
            foreach (var item in items)
            {
                ReferenceList.Items.Remove(item);
                script.RemoveReference(item);
            }
            if(items.Count >0)
            {
                ShowReferences();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            new AddReferenceWindow(this.script).ShowDialog();
            ShowReferences();
        }
    }
}