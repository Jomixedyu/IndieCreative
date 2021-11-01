using System;
using System.Collections.Generic;
using UnityEngine;

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
    /// <summary>
    /// 待办时间表管理器，按时间延迟执行。
    /// </summary>
    internal class ScheduleMono : MonoBehaviour
    {
        internal struct Schedule
        {
            internal float endtime;
            internal Action cb;
            internal Schedule(float endtime, Action cb)
            {
                this.endtime = endtime;
                this.cb = cb;
            }
        }

        private int _icount = 0;
        private int GetId()
        {
            return ++this._icount;
        }

        private Dictionary<int, Schedule> schedule;

        private List<int> waitRemoveList;

        private static ScheduleMono instance;
        internal static ScheduleMono Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject($"__m_{nameof(ScheduleMono)}");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<ScheduleMono>();
                }
                return instance;
            }
        }
        private void Awake()
        {
            this.schedule = new Dictionary<int, Schedule>();
            this.waitRemoveList = new List<int>();
        }

        internal void Add(float time, Action cb)
        {
            this.schedule.Add(this.GetId(), new Schedule(Time.time + time, cb));
        }

        internal void CompleteAll()
        {
            if (this.schedule.Count == 0)
            {
                return;
            }

            List<Schedule> schedules = new List<Schedule>(this.schedule.Values);
            schedules.Sort((x, y) => x.endtime < y.endtime ? 1 : 0);

            foreach (Schedule item in schedules)
            {
                item.cb?.Invoke();
            }
            this.schedule.Clear();
        }

        private void Update()
        {
            foreach (var item in this.schedule)
            {
                if (item.Value.endtime <= Time.time)
                {
                    this.waitRemoveList.Add(item.Key);
                }
            }
            if (this.waitRemoveList.Count > 0)
            {
                foreach (int item in this.waitRemoveList)
                {
                    this.schedule[item].cb?.Invoke();
                    this.schedule.Remove(item);
                }
                this.waitRemoveList.Clear();
            }
        }
    }
}