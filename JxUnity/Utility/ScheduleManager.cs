using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 待办时间表管理器，按时间延迟执行。
/// </summary>
public class ScheduleManager : MonoSingleton<ScheduleManager>
{
    public struct Schedule
    {
        public float endtime;
        public Action cb;
        public Schedule(float endtime, Action cb)
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

    protected override void Awake()
    {
        if (CheckInstanceAndDestroy())
        {
            return;
        }
        base.Awake();
        this.schedule = new Dictionary<int, Schedule>();
        this.waitRemoveList = new List<int>();
    }
    /// <summary>
    /// 添加一个待办事项
    /// </summary>
    /// <param name="time">等待时间</param>
    /// <param name="cb">待办内容</param>
    public void Add(float time, Action cb)
    {
        this.schedule.Add(this.GetId(), new Schedule(Time.time + time, cb));
    }
    /// <summary>
    /// 按顺序完成所有待办事项
    /// </summary>
    public void CompleteAll()
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

    private void FixedUpdate()
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
