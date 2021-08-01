using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace RevitTools.Utils.Revit
{
    /// <summary>
    /// 外部事件处理程序
    /// </summary>
    public class ExEventHandle : IExternalEventHandler
    {
        /// <summary>
        /// 创建具有指定名字的外部事件处理程序
        /// </summary>
        /// <param name="name"></param>
        public ExEventHandle(string name)
        {
            this.name = name;
        }

        private readonly string name;

        /// <summary>
        /// 工作项队列
        /// </summary>
        internal readonly ConcurrentQueue<ExEventWorkItem> ExEventQueue = new ConcurrentQueue<ExEventWorkItem>();

        /// <summary>
        /// 执行外部事件函数
        /// </summary>
        /// <param name="app"></param>
        public void Execute(UIApplication app)
        {
            while (ExEventQueue.TryDequeue(out ExEventWorkItem workItem))
            {
                workItem.InvokeAction(app);
            }
        }

        /// <summary>
        /// 获取字符串标识
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return name;
        }
    }
}