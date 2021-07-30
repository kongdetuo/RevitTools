using Autodesk.Revit.UI;
using System;
using System.Threading.Tasks;

namespace RevitTools.Utils.Revit
{
    /// <summary>
    /// 外部事件工作项
    /// </summary>
    internal class ExEventWorkItem
    {
        /// <summary>
        /// 异步阻塞信号
        /// </summary>
        public TaskCompletionSource<object> CompletionSource { get; protected set; }

        /// <summary>
        /// 在外部事件中执行的, 带有一个返回值的函数
        /// </summary>
        private Action<UIApplication> Action { get; set; }

        /// <summary>
        /// 在外部事件中执行的, 带有一个返回值的函数
        /// </summary>
        private Func<UIApplication, object> Func { get; set; }

        /// <summary>
        /// 执行外部事件函数
        /// </summary>
        /// <param name="application"></param>
        public void InvokeAction(UIApplication application)
        {
            try
            {
                if (Action != null)
                {
                    Action.Invoke(application);
                    this.CompletionSource.TrySetResult(null);
                }
                else
                {
                    var result = Func.Invoke(application);
                    this.CompletionSource.TrySetResult(result);
                }
            }
            catch (Exception ex)
            {
                this.CompletionSource.TrySetException(ex);
            }
        }

        public ExEventWorkItem(Action<UIApplication> action)
        {
            this.CompletionSource = new TaskCompletionSource<object>();
            this.Action = action;
        }

        public ExEventWorkItem(Func<UIApplication, object> action)
        {
            this.CompletionSource = new TaskCompletionSource<object>();
            this.Func = action;
        }
    }
}