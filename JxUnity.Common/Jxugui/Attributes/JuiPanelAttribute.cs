using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class JuiPanelAttribute : JuiPanelBaseAttribute
{
    public JuiPanelAttribute() { }
    public JuiPanelAttribute(string name) : base(name)
    {
    }
}
