using System;
using System.Collections.Generic;
using UObject = UnityEngine.Object;

namespace JxUnity.ResDB
{
    internal class ResDBEditorLoader : IResDBLoader
    {
        private static ResDBEditorLoader instance;
        public static ResDBEditorLoader Instance => instance ?? (instance = new ResDBEditorLoader());

        internal static Func<string, Type, UObject> LoadAssetAtPath;

        public UObject Load(string index, Type type)
        {
            return LoadAssetAtPath("Assets/" + index, type);
        }

        public void LoadAsync(string index, Type type, Action<UObject> cb)
        {
            cb?.Invoke(LoadAssetAtPath("Assets/" + index, type));
        }
    }
}
