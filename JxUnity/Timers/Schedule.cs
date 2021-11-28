using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Timers
{
    /// <summary>
    /// 待办事项时间表
    /// </summary>
    public class Schedule
    {
        private static Schedule _uiWaitDelay;
        public static Schedule UiWaitDelay => _uiWaitDelay ?? (_uiWaitDelay = new Schedule(nameof(UiWaitDelay)));

        private static Schedule _systemWaitDelay;
        public static Schedule SystemWaitDelay => _systemWaitDelay ?? (_systemWaitDelay = new Schedule(nameof(SystemWaitDelay)));

        private static Schedule _gameWaitDelay;
        public static Schedule GameWaitDelay => _gameWaitDelay ?? (_gameWaitDelay = new Schedule(nameof(GameWaitDelay)));

        public string Group { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="group">待办组名</param>
        public Schedule(string group)
        {
            this.Group = group;
        }

        /// <summary>
        /// 添加一个待办事项
        /// </summary>
        /// <param name="time">等待时间</param>
        /// <param name="cb">待办内容</param>
        public void Add(float time, Action cb)
        {
            ScheduleMono.Instance.Add(this.Group, time, cb);
        }

        /// <summary>
        /// 按顺序完成所有待办事项
        /// </summary>
        public void CompleteAll()
        {
            ScheduleMono.Instance.CompleteAll(this.Group);
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

        private Dictionary<string, Dictionary<int, Schedule>> groups;

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
            this.groups = new Dictionary<string, Dictionary<int, Schedule>>();
            this.waitRemoveList = new List<int>();
        }

        internal void Add(string group, float time, Action cb)
        {
            Dictionary<int, Schedule> schedules;
            if (!this.groups.TryGetValue(group, out schedules))
            {
                schedules = new Dictionary<int, Schedule>();
                this.groups.Add(group, schedules);
            }

            schedules.Add(this.GetId(), new Schedule(Time.time + time, cb));
        }

        internal void CompleteAll(string group)
        {
            if (this.groups.Count == 0)
            {
                return;
            }

            Dictionary<int, Schedule> schedule;
            if (!this.groups.TryGetValue(group, out schedule))
            {
                return;
            }

            List<Schedule> schedules = new List<Schedule>(schedule.Values);
            schedules.Sort((x, y) => x.endtime < y.endtime ? 1 : 0);

            foreach (Schedule item in schedules)
            {
                item.cb?.Invoke();
            }

            this.groups.Remove(group);
        }

        private void Update()
        {
            foreach (var schedules in this.groups)
            {
                foreach (var schedule in schedules.Value)
                {
                    if (schedule.Value.endtime <= Time.time)
                    {
                        this.waitRemoveList.Add(schedule.Key);
                    }
                }
                if (this.waitRemoveList.Count > 0)
                {
                    foreach (int item in this.waitRemoveList)
                    {
                        schedules.Value[item].cb?.Invoke();
                        schedules.Value.Remove(item);
                    }
                    this.waitRemoveList.Clear();
                }
            }


        }
    }
}