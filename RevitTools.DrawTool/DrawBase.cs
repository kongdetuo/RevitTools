using System.Drawing;
using System.Drawing.Drawing2D;

namespace RevitTools.DrawTool
{
    /// <summary>
    /// 基本绘图对象
    /// </summary>
    public abstract class DrawBase
    {
        internal Color m_BackColor;
        internal Color m_ForeColor;


        /// <summary>
        /// 背景色
        /// </summary>
        public Color BackColor
        {
            get { return m_BackColor; }
            set { m_BackColor = value; }
        }

        /// <summary>
        /// 前景色
        /// </summary>
        public Color ForeColor
        {
            get { return m_ForeColor; }
            set { m_ForeColor = value; }
        }

        /// <summary>
        /// 获取图形外接矩形
        /// </summary>
        /// <returns>图形的外接矩形</returns>
        public abstract Rectangle GetBound();

        /// <summary>
        /// 在指定的Graphics上绘制图形
        /// </summary>
        /// <param name="g">指定的 Graphics</param>
        public abstract void Draw(Graphics g);

        /// <summary>
        /// 命中测试
        /// </summary>
        /// <returns></returns>
        public virtual bool HitTest(Point point)
        {
            var bound = GetBound();
            if (bound.Left > point.X || bound.Right < point.X
                || bound.Top > point.Y || bound.Bottom > point.Y)
                return false;

            using (var bmp = new Bitmap(bound.Width, bound.Height))
            using (var g = Graphics.FromImage(bmp))
            {
                var dx = -bound.X;
                var dy = -bound.Y;

                var obj = this.Clone();
                obj.Draw(g);

                return bmp.GetPixel(point.X + dx, point.Y + dy).A == 0;
            }
        }

        /// <summary>
        /// 移动对象
        /// </summary>
        /// <param name="dx">X 偏移量</param>
        /// <param name="dy">Y 偏移量</param>
        public abstract void Move(int dx, int dy);

        public virtual DrawBase Clone()
        {
            return this.MemberwiseClone() as DrawBase;
        }
    }
    public static class ExtensionMethods
    {
        public static Matrix SetTranslate(this Matrix matrix, float offsetX, float offsetY)
        {
            matrix.Translate(offsetX, offsetY);
            return matrix;
        }
        public static Matrix SetTranslate(this Matrix matrix, float offsetX, float offsetY, MatrixOrder order)
        {
            matrix.Translate(offsetX, offsetY);
            return matrix;
        }
    }
}
