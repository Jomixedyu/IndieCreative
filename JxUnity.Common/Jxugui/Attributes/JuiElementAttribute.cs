using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class JuiElementAttribute : JuiAbstractAttribute
{
    private string path;
    public string Path { get => path; }
    public JuiElementAttribute() { }

    public JuiElementAttribute(string path)
    {
        this.path = path;
    }
}

