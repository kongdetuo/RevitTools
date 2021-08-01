using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitTools.DrawTool
{
    public partial class DrawingCanvas : PictureBox, IDisplay
    {
        public DrawingCanvas()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        private List<DrawBase> drawObjects = new List<DrawBase>();

        public void AddObj(DrawBase draw)
        {
            drawObjects.Add(draw);
            MoveObj(draw, 0, 0);
        }

        /// <summary>
        /// 绘制对象
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            foreach (var item in drawObjects)
            {
                item.Draw(e.Graphics);
            }

            base.OnPaint(e);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="db">要删除的对象</param>
        public void DeleteObj(DrawBase db)
        {
            if (db == null)
            {
                throw new Exception("没有选中的项目");
            }

            Rectangle bound = db.GetBound();
            this.drawObjects.Remove(db);
            bound.Inflate(2, 2);
            this.Invalidate(bound);
        }

        /// <summary>
        /// 移动绘图对象
        /// </summary>
        /// <param name="draw">要移动的绘图对象</param>
        /// <param name="p">目标位置</param>
        public void MoveObj(DrawBase draw, Point p)
        {
            Rectangle bound = draw.GetBound();
            Rectangle last = new Rectangle(bound.X, bound.Y, bound.Width, bound.Height);

            bound.Offset(p.X - bound.X, p.Y - bound.Y);
            draw.Move(p.X - bound.X, p.Y - bound.Y);

            last.Inflate(2, 2);
            bound.Inflate(2, 2);

            using (Region invReg = new Region(last))
            {
                invReg.Union(bound);
                this.Invalidate(invReg);
            }
        }

        /// <summary>
        /// 移动绘图对象
        /// </summary>
        /// <param name="db">要移动的对象</param>
        /// <param name="dx">x方向移动的距离</param>
        /// <param name="dy">y方向移动的距离</param>
        public void MoveObj(DrawBase db, int dx, int dy)
        {
            Rectangle bound = db.GetBound();
            Rectangle last = new Rectangle(bound.X, bound.Y, bound.Width, bound.Height);

            bound.Offset(dx, dy);
            db.Move(dx, dy);

            last.Inflate(2, 2);
            bound.Inflate(2, 2);

            using (Region invReg = new Region(last))
            {
                invReg.Union(bound);
                this.Invalidate(invReg);
            }
        }

        /// <summary>
        /// 移动所有对象
        /// </summary>
        /// <param name="dx">x方向移动的距离</param>
        /// <param name="dy">y方向移动的距离</param>
        public void MoveAllObj(int dx, int dy)
        {
            var list = new List<DrawBase>(drawObjects);
            foreach (var item in list)
            {
                MoveObj(item, dx, dy);
            }
        }

        /// <summary>
        /// 获取指定位置的对象
        /// </summary>
        /// <param name="e">指定位置的值</param>
        /// <returns>绘图对象</returns>
        public DrawBase GetObj(Point e)
        {
            return drawObjects.LastOrDefault(p => p.HitTest(e));
        }

        /// <summary>
        /// 清空所有对象
        /// </summary>
        public void Clear()
        {
            var v = new List<DrawBase>(drawObjects);
            foreach (var item in v)
            {
                DeleteObj(item);
            }
        }
    }
}