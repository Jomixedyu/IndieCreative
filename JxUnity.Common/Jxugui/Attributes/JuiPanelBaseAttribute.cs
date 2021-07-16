using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public abstract class JuiPanelBaseAttribute : JuiAbstractAttribute
{
    public string Name { get; set; } = default;
    /// <summary>
    /// 启用Update函数
    /// </summary>
    public bool EnableUpdate { get; set; } = false;
    /// <summary>
    /// 提前将脚本绑定到UI
    /// </summary>
    public bool IsPreBind { get; set; } = false;
    /// <summary>
    /// 资源加载路径
    /// </summary>
    public string ResourcePath { get; set; } = default;
    /// <summary>
    /// 自动绑定元素
    /// </summary>
    public bool IsAutoBindElement { get; set; } = true;
    public JuiPanelBaseAttribute() { }
    public JuiPanelBaseAttribute(string name)
    {
        this.Name = name;
    }
}