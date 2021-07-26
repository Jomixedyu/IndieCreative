using System;

namespace JxUnity.Jxugui
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class JuiElementAttribute : JuiAbstractAttribute
    {
        public string Path { get; set; }

        public JuiElementAttribute() { }
        public JuiElementAttribute(string path)
        {
            this.Path = path;
        }
    }
}