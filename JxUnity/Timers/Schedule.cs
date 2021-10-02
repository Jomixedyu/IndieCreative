using System;

namespace JxUnity.Timers
{
    public static class Schedule
    {
        /// <summary>
        /// 添加一个待办事项
        /// </summary>
        /// <param name="time">等待时间</param>
        /// <param name="cb">待办内容</param>
        public static void Add(float time, Action cb)
        {
            ScheduleMono.Instance.Add(time, cb);
        }
        /// <summary>
        /// 按顺序完成所有待办事项
        /// </summary>
        public static void CompleteAll()
        {
            ScheduleMono.Instance.CompleteAll();
        }
    }
}