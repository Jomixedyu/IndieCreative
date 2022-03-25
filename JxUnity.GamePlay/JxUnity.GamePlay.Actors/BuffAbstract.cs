

public abstract class BuffAbstract
{
    public ActorAbstract Character { get; private set; }
    /// <summary>
    /// Buff持续时间
    /// </summary>
    public int Duration { get; private set; }

    public int EndTime { get; private set; }

    public void Initialize(ActorAbstract character, int duration, int endTime)
    {
        this.Character = character;
        this.Duration = duration;
        this.EndTime = endTime;
    }
    /// <summary>
    /// 消息：Buff开始时
    /// </summary>
    public virtual void OnEnter() { }
    /// <summary>
    /// 消息：Buff结束时
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// 消息：更新，66帧为一秒的固定帧率
    /// </summary>
    /// <param name="frame"></param>
    public virtual void FixedUpdate(int frame) { }
}
