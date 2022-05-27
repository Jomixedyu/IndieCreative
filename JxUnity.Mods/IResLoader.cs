using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JxUnity.Mods
{
    public interface IResLoader
    {
        UnityEngine.Object Load(string assetPath);
    }
}
