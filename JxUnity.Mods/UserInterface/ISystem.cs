using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JxUnity.Mods.UserInterface
{
    public interface ISystem
    {
        void Log(string msg);
        void LogWarning(string msg);
        void LogError(string msg);
    }
}
