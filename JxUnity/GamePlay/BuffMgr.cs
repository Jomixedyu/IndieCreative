using System;
using System.Collections.Generic;

public class BuffMgr
{
    private List<BuffAbstract> buffs = new List<BuffAbstract>();
    private ActorAbstract actor;
    private int curFrame;

    public BuffMgr(ActorAbstract actor)
    {
        this.actor = actor;
    }

    public bool ExistType(Type buffType)
    {
        foreach (var item in buffs)
        {
            if (item.GetType() == buffType)
            {
                return true;
            }
        }
        return false;
    }
    public bool ExistType<T>()
    {
        return ExistType(typeof(T));
    }
    public bool Exist(BuffAbstract buff)
    {
        return this.buffs.Contains(buff);
    }

    public void FixedUpdate(int frame)
    {
        this.curFrame = frame;

        List<BuffAbstract> deleteBuff = null;
        
        foreach (var item in buffs)
        {
            if (frame >= item.EndTime)
            {
                if (deleteBuff == null)
                {
                    deleteBuff = new List<BuffAbstract>();
                }
                deleteBuff.Add(item);
            }
            item.FixedUpdate(frame);
        }

        if (deleteBuff != null)
        {
            foreach (var item in deleteBuff)
            {
                this.Remove(item);
            }
        }
    }

    public void Add(BuffAbstract buff, int duration)
    {
        if (this.ExistType(buff.GetType()))
        {
            this.RemoveByType(buff.GetType());
        }
        this.buffs.Add(buff);
        buff.Initialize(this.actor, duration, curFrame + duration);
        buff.OnEnter();
    }

    public T Add<T>(int duration) where T : BuffAbstract, new()
    {
        var buf = Activator.CreateInstance<T>();
        this.Add(buf, duration);
        return buf;
    }

    public void Remove(BuffAbstract buff)
    {
        this.buffs.Remove(buff);
        buff.OnExit();
    }
    public void RemoveByType(Type type)
    {
        List<BuffAbstract> deleteBuff = new List<BuffAbstract>();
        foreach (var item in this.buffs)
        {
            if (item.GetType() == type)
            {
                deleteBuff.Add(item);
            }
        }
        foreach (var item in deleteBuff)
        {
            this.Remove(item);
        }
    }
    public void RemoveAll()
    {
        foreach (var item in this.buffs)
        {
            item.OnExit();
        }
        this.buffs.Clear();
    }

}