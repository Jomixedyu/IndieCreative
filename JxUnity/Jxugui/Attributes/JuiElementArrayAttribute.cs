using System;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class JuiElementArrayAttribute : JuiAbstractAttribute
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

