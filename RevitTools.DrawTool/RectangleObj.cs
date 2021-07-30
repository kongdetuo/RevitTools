using System.Drawing;

namespace RevitTools.DrawTool
{
    public class RectangleObj : DrawBase
    {
        private Point m_Start;
        private Point m_End;
        private bool m_Solid;

        public RectangleObj(Point start, Point end)
        {
            this.m_Start = start;
            this.m_End = end;
        }

        public bool Solid
        {
            get { return m_Solid; }
            set { m_Solid = value; }
        }

        public override System.Drawing.Rectangle GetBound()
        {
            int x = this.m_Start.X < this.m_End.X ? this.m_Start.X : this.m_End.X;
            int y = this.m_Start.Y < this.m_End.Y ? this.m_Start.Y : this.m_End.Y;
            int r = this.m_Start.X < this.m_End.X ? this.m_End.X : this.m_Start.X;
            int b = this.m_Start.Y < this.m_End.Y ? this.m_End.Y : this.m_Start.Y;
            return Rectangle.FromLTRB(x, y, r, b);
        }

        public override void Draw(Graphics g)
        {
            Rectangle bound = this.GetBound();
            if (this.m_Solid)
            {
                using (SolidBrush brush = new SolidBrush(this.m_BackColor))
                {
                    g.FillRectangle(brush, bound);
                }
            }
            using (Pen pen = new Pen(this.m_ForeColor))
            {
                g.DrawRectangle(pen, bound);
            }
        }

        public override void Move(int dx, int dy)
        {
            m_Start.X += dx;
            m_Start.Y += dy;

            m_End.X += dx;
            m_End.Y += dy;
        }
    }

}
