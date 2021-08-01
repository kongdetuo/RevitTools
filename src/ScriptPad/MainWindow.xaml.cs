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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScriptPad
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //if (string.IsNullOrWhiteSpace(Properties.Settings.Default.WorkFolder))
            //{
            //    Properties.Settings.Default.WorkFolder = Environment.CurrentDirectory;
            //}
            //var dir = new System.IO.DirectoryInfo(Properties.Settings.Default.WorkFolder);
            //treeRoot.Header = dir.FullName;
            //treeRoot.IsExpanded = true;

            //foreach (var item in dir.EnumerateFiles())
            //{
            //    if (item.Extension == ".csx")
            //    {
            //        this.treeRoot.Items.Add(new TreeViewItem() { Header = item.Name });
            //    }
            //}

            AddEditor();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tc.SelectedIndex == tc.Items.Count - 1)
            {
                AddEditor();
            }
        }

        private void AddEditor(string path = null)
        {
            var editor = new CodeEditor();
            var tb = new TextBlock();
            tb.Text = editor.Script.Name;
            tb.ContextMenu = new ContextMenu();
            var menuItem = new MenuItem();
            menuItem.Header = "关闭";
            tb.ContextMenu.Items.Add(menuItem);


            var tabitem = (TabItem)tc.Items[tc.Items.Count - 1];
            tabitem.Header = tb;// editor.Script.Name;
            tabitem.Content = editor;

            menuItem.Click += (sender, e) =>
            {
                CloseTab(tabitem);
            };

            var tab = new TabItem() { Header = "+" };
            tc.Items.Add(tab);
        }

        /// <summary>
        /// 关闭标签页
        /// </summary>
        /// <param name="tab"></param>
        private void CloseTab(TabItem tab)
        {
            try
            {
                var codeEditor = tab.Content as CodeEditor;
                codeEditor.Close();
                tc.Items.Remove(tab);
                if (tc.Items.Count == 1)
                {
                    AddEditor();
                }
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var names = tc.Items.Cast<TabItem>()
                .Select(tab => tab.Content as CodeEditor)
                .Where(editor => editor != null)
                .Select(editor => editor.Script.Name)
                .ToList();
        }

        private async void RunCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var tabItem = this.tc.SelectedItem as TabItem;
            var codeEditor = tabItem.Content as CodeEditor;
            await codeEditor.Run();
        }
    }
}