using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public abstract class JuiPanelBaseAttribute : JuiAbstractAttribute
{
    /// <summary>
    /// UI名字，如果和对象名相同则可省略
    /// </summary>
    public string Name { get; set; } = default;
    /// <summary>
    /// 启用Update函数
    /// </summary>
    public bool EnableUpdate { get; set; } = false;
    /// <summary>
    /// 资源加载路径，当UI不存在于场景时，进行动态加载
    /// </summary>
    public string ResourcePath { get; set; } = default;
    /// <summary>
    /// 自动绑定元素，对类型内标示的字段进行自动绑定
    /// </summary>
    public bool IsAutoBindElement { get; set; } = true;

    public JuiPanelBaseAttribute() { }
    public JuiPanelBaseAttribute(string name)
    {
        this.Name = name;
    }
}