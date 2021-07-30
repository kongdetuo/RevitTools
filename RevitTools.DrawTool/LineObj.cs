using System;
using System.Drawing;

namespace RevitTools.DrawTool
{
    public class LineObj : DrawBase
    {
        protected Point m_Start;
        protected Point m_End;

        public LineObj(Point start, Point end)
        {
            this.m_Start = start;
            this.m_End = end;
        }

        /// <summary>
        /// 获取图形外接矩形
        /// </summary>
        /// <returns>图形的外接矩形</returns>
        public override Rectangle GetBound()
        {
            int x = this.m_Start.X < this.m_End.X ? this.m_Start.X : this.m_End.X;
            int y = this.m_Start.Y < this.m_End.Y ? this.m_Start.Y : this.m_End.Y;
            int r = this.m_Start.X < this.m_End.X ? this.m_End.X : this.m_Start.X;
            int b = this.m_Start.Y < this.m_End.Y ? this.m_End.Y : this.m_Start.Y;
            return Rectangle.FromLTRB(x, y, r, b);
        }

        /// <summary>
        /// 在指定的Graphics上绘制图形
        /// </summary>
        /// <param name="g">指定的 Graphics</param>
        public override void Draw(Graphics g)
        {
            using (Pen pen = new Pen(this.m_ForeColor))
            {
                g.DrawLine(pen, this.m_Start, this.m_End);
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