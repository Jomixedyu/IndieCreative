
using System;
using System.Collections.Generic;
using UnityEngine;
using UObject = UnityEngine.Object;

namespace JxUnity.ResDB
{
    internal interface IResDBLoader
    {
        UObject Load(string index, Type type);
        void LoadAsync(string index, Type type, Action<UObject> cb);
    }
}
