using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JxUnity.Mods
{
    //外界只需要提供一个启动入口即可，只需要引入SDK（或添加UnityEngine引用）即可完成mod制作。

    public class ModObject
    {
        public ModInfo Info { get; protected set; }
        protected Action OnEnableFunction;
        protected Action OnDisableFunction;

        protected bool isEnabled = false;

        public ModObject(ModInfo info, Action onEnableFunction, Action onDisableFunction)
        {
            this.Info = info;
            this.OnEnableFunction = onEnableFunction;
            this.OnDisableFunction = onDisableFunction;
        }

        public void Enable()
        {
            if (!isEnabled)
            {
                isEnabled = true;
                OnEnableFunction?.Invoke();
            }
        }
        public void Disable()
        {
            if (isEnabled)
            {
                isEnabled = false;
                OnDisableFunction?.Invoke();
            }
        }

    }

}
