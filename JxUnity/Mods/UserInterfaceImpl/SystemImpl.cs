using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JxUnity.Mods.UserInterface;
using UnityEngine;

namespace JxUnity.Mods.UserInterfaceImpl
{
    internal class SystemImpl : ISystem
    {
        public void Log(string msg)
        {
            Debug.Log(msg);
        }

        public void LogError(string msg)
        {
            Debug.LogError(msg);
        }

        public void LogWarning(string msg)
        {
            Debug.LogWarning(msg);
        }
    }
}
