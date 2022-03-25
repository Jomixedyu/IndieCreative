using UnityEngine;

public abstract class ActorAbstract
{
    public BuffMgr Buff { get; private set; }
    public int ActorId { get; private set; }

    public float Hp { get; private set; }
    public float Mp { get; private set; }

    public bool IsDied { get; private set; } = false;

    public float MaxHp { get; set; }
    public float MaxMp { get; set; }

    public GameObject gameObject { get; private set; }
    public Transform transform { get; private set; }

    public bool Selected { get; set; }

    public ActorAbstract()
    {
        this.Buff = new BuffMgr(this);
    }

    public void Initialize(GameObject go, int id)
    {
        this.gameObject = go;
        this.transform = go.transform;
        this.ActorId = id;
        this.OnInitialize();
    }
    /// <summary>
    /// 消息：初始化
    /// </summary>
    protected virtual void OnInitialize() { }

    public void FixedLogicUpdate(int frame)
    {
        this.Buff.FixedUpdate(frame);
        this.OnFixedLogicUpdate(frame);
    }
    public virtual void RenderUpdate()
    {
        this.OnRenderUpdate();
    }
    /// <summary>
    /// 消息：固定帧率更新，每66帧为1秒
    /// </summary>
    /// <param name="frame"></param>
    protected virtual void OnFixedLogicUpdate(int frame)
    {

    }

    protected virtual void OnRenderUpdate()
    {

    }

    /// <summary>
    /// 增加Hp
    /// </summary>
    /// <param name="hp"></param>
    public virtual void AddHp(float hp)
    {
        if (IsDied)
        {
            return;
        }
        float ohp = this.Hp;
        this.Hp = Mathf.Clamp(this.Hp + hp, 0, this.MaxHp);
        this.OnAddHp(this.Hp - ohp);
    }
    /// <summary>
    /// 减少Hp
    /// </summary>
    /// <param name="hp"></param>
    public virtual void ReduceHp(float hp)
    {
        if (IsDied)
        {
            return;
        }
        float ohp = this.Hp;
        this.Hp = Mathf.Clamp(this.Hp - hp, 0, this.MaxHp);
        this.OnReduceHp(ohp - this.Hp);
        if (this.Hp == 0)
        {
            if (!IsDied)
            {
                IsDied = true;
                this.OnDied();
            }
        }
    }
    /// <summary>
    /// 增加Mp
    /// </summary>
    /// <param name="mp"></param>
    public virtual void AddMp(float mp)
    {
        if (IsDied)
        {
            return;
        }
        float omp = this.Mp;
        this.Mp = Mathf.Clamp(this.Mp + mp, 0, this.MaxMp);
        this.OnAddMp(this.Mp - omp);
    }
    /// <summary>
    /// 减少Mp
    /// </summary>
    /// <param name="mp"></param>
    public virtual void ReduceMp(float mp)
    {
        if (IsDied)
        {
            return;
        }
        float omp = this.Mp;
        this.Mp = Mathf.Clamp(this.Mp - mp, 0, this.MaxMp);
        this.OnReduceMp(omp - this.Mp);
    }

    /// <summary>
    /// 恢复健康，满Hp满Mp
    /// </summary>
    public void ToHealthy()
    {
        if (IsDied)
        {
            return;
        }
        this.AddHp(this.MaxHp);
        this.AddMp(this.MaxMp);
    }

    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="hp">当hp为0时，代表使用恢复至满</param>
    public void Revive(int hp = 0)
    {
        IsDied = false;
        this.AddHp(hp != 0 ? hp : this.MaxHp);
        this.OnRevived();
    }
    /// <summary>
    /// 死亡
    /// </summary>
    public void Die()
    {
        if (IsDied)
        {
            return;
        }
        this.ReduceHp(this.MaxHp);
    }
    /// <summary>
    /// 消息：当Hp增加
    /// </summary>
    /// <param name="addHp"></param>
    protected virtual void OnAddHp(float addHp) { }
    /// <summary>
    /// 消息：当Hp减少
    /// </summary>
    /// <param name="reduceHp"></param>
    protected virtual void OnReduceHp(float reduceHp) { }
    /// <summary>
    /// 消息：当Mp增加
    /// </summary>
    /// <param name="addMp"></param>
    protected virtual void OnAddMp(float addMp) { }
    /// <summary>
    /// 消息：当Mp减少
    /// </summary>
    /// <param name="reduceMp"></param>
    protected virtual void OnReduceMp(float reduceMp) { }
    /// <summary>
    /// 消息：当角色死亡
    /// </summary>
    protected virtual void OnDied() { }
    /// <summary>
    /// 消息：当角色复活
    /// </summary>
    protected virtual void OnRevived() { }
}