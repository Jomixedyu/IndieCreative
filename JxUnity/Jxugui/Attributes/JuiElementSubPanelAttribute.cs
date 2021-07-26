using System;

namespace JxUnity.Jxugui
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class JuiElementSubPanelAttribute : JuiPanelBaseAttribute
    {
        public JuiElementSubPanelAttribute() { }
        public JuiElementSubPanelAttribute(string name) : base(name)
        {
        }
        public JuiElementSubPanelAttribute(string name, string path) : base(name)
        {
            this.Path = path;
        }
    }

}