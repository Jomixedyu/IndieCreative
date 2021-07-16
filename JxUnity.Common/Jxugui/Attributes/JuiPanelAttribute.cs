using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class JuiPanelAttribute : JuiPanelBaseAttribute
{
    /// <summary>
    /// 在场景初始化时将脚本绑定到UI
    /// </summary>
    public bool IsPreBind { get; set; } = false;

    public JuiPanelAttribute() { }
    public JuiPanelAttribute(string name) : base(name)
    {
    }
}
