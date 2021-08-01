using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace RevitTools.DrawTool
{
    public partial class DrawForm : Form
    {
        System.Timers.Timer Timer = new System.Timers.Timer(20);

        public UIApplication uiApp { get; private set; }
        public DrawingCanvas Canvas { get; private set; }
        public DrawForm(Autodesk.Revit.UI.UIApplication uIApplication)
        {
            this.uiApp = uIApplication;
            InitializeComponent();
            var bitmap = new Bitmap(1, 1);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
            }


            this.BackgroundImage = bitmap;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            Canvas = new DrawingCanvas();
            Canvas.Dock = DockStyle.Fill;
            Canvas.BackgroundImage = bitmap;
            Canvas.BackgroundImageLayout = ImageLayout.Stretch;
            this.Controls.Add(Canvas);
            this.Timer.Elapsed += Timer_Elapsed;
        }

        private Point Point;
        private int i;
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            FormExtensionMethods.Invoke(this, () =>
            {
                var position = Control.MousePosition;
                if (position == this.Point)
                    return;

                this.Point = position;
                position -= new Size(this.Left, this.Top);

                i++;
                if (i % 50 == 0)
                {
                    UpdateWindow();
                    i = 0;
                }


                Canvas.Clear();

                Canvas.AddObj(new LineObj(new Point(0, position.Y), new Point(position.X - 3, position.Y)) { ForeColor = Color.Green });
                Canvas.AddObj(new LineObj(new Point(position.X + 3, position.Y), new Point(this.Width + 3, position.Y)) { ForeColor = Color.Green });

                Canvas.AddObj(new LineObj(new Point(position.X, 0), new Point(position.X, position.Y - 3)) { ForeColor = Color.Green });
                Canvas.AddObj(new LineObj(new Point(position.X, position.Y + 3), new Point(position.X, this.Height + 3)) { ForeColor = Color.Green });

                //Canvas.AddObj(new RectangleObj(position - new Size(2, 2), position + new Size(2, 2)) { ForeColor = Color.Green });
            });
        }

        private void UpdateWindow()
        {
            var uiView = uiApp.ActiveUIDocument.GetOpenUIViews()
                 .FirstOrDefault(p => p.ViewId == uiApp.ActiveUIDocument.ActiveGraphicalView.Id);
            var view = uiApp.ActiveUIDocument.ActiveView;
            var rect = uiView.GetWindowRectangle();
            this.Left = rect.Left;
            this.Top = rect.Top;
            this.Width = rect.Right - rect.Left;
            this.Height = rect.Bottom - rect.Top;
        }

        private void DrawForm_Shown(object sender, EventArgs e)
        {
            UpdateWindow();
            Timer.Start();
        }

        private void DrawForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Timer.Stop();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {

        }
    }

    public static class FormExtensionMethods
    {
        public static void Invoke(this Form form, Action action)
        {
            form.Invoke(new MethodInvoker(action));
        }


    }

}
