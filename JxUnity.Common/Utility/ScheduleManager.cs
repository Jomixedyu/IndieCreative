using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        this.schedule = new Dictionary<int, Schedule>();
        this.waitRemoveList = new List<int>();
    }

    public void Add(float time, Action cb)
    {
        this.schedule.Add(this.GetId(), new Schedule(Time.time + time, cb));
    }

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
