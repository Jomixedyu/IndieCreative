using UnityEngine;

namespace JxUnity.Jxugui
{
    /// <summary>
    /// 子UI面板
    /// </summary>
    public abstract class JuiSubBase : JuiAbstract
    {
        protected override GameObject LoadResource(string path)
        {
            return JuiManager.Instance.LoadResource(this.parent.Name + '/' + path);
        }
    }
}