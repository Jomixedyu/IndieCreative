using UnityEngine;

namespace JxUnity.SaveDatas
{
    internal class PathHelper
    {
        public static string GetWritablePath()
        {
            if (Application.isEditor)
            {
                return System.Environment.CurrentDirectory;
            }
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                    return global::System.AppDomain.CurrentDomain.BaseDirectory;
                case RuntimePlatform.Android:
                    return Application.persistentDataPath;
                case RuntimePlatform.IPhonePlayer:
                    return Application.temporaryCachePath;
                default:
                    break;
            }
            return null;
        }
    }
}
