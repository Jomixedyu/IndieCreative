using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UObject = UnityEngine.Object;
using JxUnity.ResDB.Private;

namespace JxUnity.ResDB
{
    public enum AssetLoadMode
    {
        Inline,
        Package,
    }



    public static class ResDatabase
    {
        internal static AssetLoadMode AssetLoadMode { get; set; }
        internal static bool IsRealMode;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IResDBLoader GetLoader(AssetLoadMode mode)
        {
            if (!IsRealMode)
            {
                return ResDBEditorLoader.Instance;
            }
            switch (mode)
            {
                case AssetLoadMode.Inline: return ResDBInlineLoaderMono.Instance;
                case AssetLoadMode.Package: return ResDBPackageLoaderMono.Instance;
            }
            return null;
        }

        public static UObject Load(string path, Type type)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return GetLoader(AssetLoadMode).Load($"{ResDBConfig.ResDBFolderName}/{path}", type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Load<T>(string path) where T : UObject
        {
            return Load(path, typeof(T)) as T;
        }

        public static void LoadAsync(string path, Type type, Action<UObject> cb)
        {
            if (string.IsNullOrEmpty(path))
            {
                cb.Invoke(null);
                return;
            }
            GetLoader(AssetLoadMode).LoadAsync($"{ResDBConfig.ResDBFolderName}/{path}", type, cb);
        }
    }
}