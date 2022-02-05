using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JxUnity.Localization
{
    public static class LocalizationManager
    {
        private static string currentLang;
        public static string CurrentLang
        {
            get => currentLang;
            set
            {
                currentLang = value;
                if (EnableCaching)
                {
                    if (tableCaching.TryGetValue(value, out var weakCache)
                        && weakCache.TryGetTarget(out var cache))
                    {
                        currentTable = cache;
                    }
                    else
                    {
                        currentTable = LoadLang(value);
                        tableCaching.Add(value, new WeakReference<LocalizedTable>(currentTable));
                    }
                }
                else
                {
                    currentTable = LoadLang(value);
                }
                LanguageChanged?.Invoke();
            }
        }
        private static LocalizedTable currentTable;
        public static event Action LanguageChanged;


        private static Dictionary<string, WeakReference<LocalizedTable>> tableCaching
            = new Dictionary<string, WeakReference<LocalizedTable>>();

        public static bool EnableCaching { get; set; } = true;

        public static readonly string FolderName = "Lang";
        public static string ReadPath { get; } = System.Environment.CurrentDirectory + "/" + FolderName;

        static LocalizedTable LoadLang(string name) => LocalizedSerializer.LoadAndDeserialize(GetFilePath(name));

        static string GetFilePath(string name) => $"{ReadPath}/{name}.xml";

        static bool IsExistLang(string name) => File.Exists(GetFilePath(name));

        public static string GetString(string id)
        {
            return string.Empty;
        }

        private static string GetSystemLang()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                    return LocalizedType.zh_CN;
                case SystemLanguage.ChineseTraditional:
                    return LocalizedType.zh_TW;
                case SystemLanguage.English:
                    return LocalizedType.en_US;
                case SystemLanguage.Japanese:
                    return LocalizedType.ja_JP;
            }
            return null;
        }
    }

}