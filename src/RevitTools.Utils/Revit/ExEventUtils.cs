using Autodesk.Revit.UI;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace RevitTools.Utils.Revit
{
    /// <summary>
    /// 外部事件封装类
    /// </summary>
    public class ExEventUtils
    {
        /// <summary>
        /// 创建外部事件
        /// </summary>
        /// <param name="handleName"></param>
        private ExEventUtils(string handleName)
        {
            this.Handle = new ExEventHandle(handleName);
            this.ExEvent = ExternalEvent.Create(this.Handle);
        }

        /// <summary>
        /// 外部事件实例
        /// </summary>
        private readonly ExternalEvent ExEvent;

        /// <summary>
        /// 外部事件处理程序
        /// </summary>
        private readonly ExEventHandle Handle;

        /// <summary>
        /// 在外部事件中执行函数, 可以等待函数执行完成
        /// </summary>
        /// <param name="action">要执行的函数</param>
        /// <param name="transactionGroupName">事务组名</param>
        /// <returns></returns>
        public async Task InvokeAsync(Action<UIApplication> action)
        {
            var workItem = new ExEventWorkItem(action);
            Handle.ExEventQueue.Enqueue(workItem);
            ExEvent.Raise();
            await workItem.CompletionSource.Task;
        }

        /// <summary>
        /// 在外部事件中执行函数, 并且可以等待函数执行完成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func">要执行的函数</param>
        /// <returns></returns>
        public async Task<T> InvokeAsync<T>(Func<UIApplication, T> func)
        {
            object func1(UIApplication app)
            {
                return func(app);
            }
            var workItem = new ExEventWorkItem(func1);
            Handle.ExEventQueue.Enqueue(workItem);
            ExEvent.Raise();
            var result = await workItem.CompletionSource.Task;
            return (T)result;
        }

        /// <summary>
        /// 获取或创建外部事件处理实例
        /// </summary>
        /// <param name="name">外部事件处理程序名称</param>
        /// <returns></returns>
        public static ExEventUtils GetOrCreate(string name = "ScriptPadExEventHandle")
        {
            return EventUtils.GetOrAdd(name, new ExEventUtils(name));
        }

        private static readonly ConcurrentDictionary<string, ExEventUtils> EventUtils = new ConcurrentDictionary<string, ExEventUtils>();
    }
}