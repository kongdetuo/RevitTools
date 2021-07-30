using RevitCodePad.Extensions;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RevitCodePad
{
    /// <summary>
    /// Show3D.xaml 的交互逻辑
    /// </summary>
    public partial class Show3DWindow : Window
    {
        public Show3DWindow(Autodesk.Revit.DB.Document doc, IEnumerable<Autodesk.Revit.DB.ElementId> elementIds)
        {
            InitializeComponent();


            var elements = doc.GetElements(elementIds);

            foreach (var item in Display3d(elements))
            {
                viewPort3d.Children.Add(item);
            }
        }

        private List<ModelVisual3D> Display3d(IEnumerable<Autodesk.Revit.DB.Element> elements)
        {
            return elements.Select(p => p.ToModel3D())
                .Where(p => p != null)
                .Select(p => new ModelVisual3D() { Content = p })
                .ToList();
        }
    }
}
