using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class JuiElementSubPanelAttribute : JuiPanelBaseAttribute
{
    /// <summary>
    /// 在父UI节点下的路径，如果和变量名相同则可省略
    /// </summary>
    public string Path { get; set; } = default;

    public JuiElementSubPanelAttribute() { }
    public JuiElementSubPanelAttribute(string name) : base(name)
    {
    }
    public JuiElementSubPanelAttribute(string name, string path) : base(name)
    {
        this.Path = path;
    }
}

