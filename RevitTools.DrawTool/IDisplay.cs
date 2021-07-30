using System.Drawing;

namespace RevitTools.DrawTool
{
    /// <summary>
    /// 管理接口
    /// </summary>
    public interface IDisplay
    {
        void AddObj(DrawBase draw);

        void Clear();

        void DeleteObj(DrawBase db);

        DrawBase GetObj(Point e);

        void MoveAllObj(int dx, int dy);

        void MoveObj(DrawBase db, int dx, int dy);
    }
}
