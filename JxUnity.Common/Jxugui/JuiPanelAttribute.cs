using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class JuiPanelAttribute : Attribute
{
    /// <summary>
    /// 在JuiManager节点下的路径
    /// </summary>
    public string UiPath { get; set; } = default;
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
    public JuiPanelAttribute() { }
    public JuiPanelAttribute(string uipath)
    {
        this.UiPath = uipath;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class JuiElementAttribute : Attribute
{
    private string path;
    public string Path { get => path; }
    public JuiElementAttribute() { }

    public JuiElementAttribute(string path)
    {
        this.path = path;
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class JuiElementArrayAttribute : Attribute
{
    private string path;
    public string Path { get => path; }

    private Type elementType;
    public Type ElementType { get => elementType; }

    public JuiElementArrayAttribute(Type elementType)
    {
        this.elementType = elementType;
    }

    public JuiElementArrayAttribute(Type elementType, string path)
    {
        this.elementType = elementType;
        this.path = path;
    }
}

