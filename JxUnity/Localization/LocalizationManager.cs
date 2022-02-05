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
                if (!IsExistLang(value))
                {
                    throw new ArgumentOutOfRangeException("not found lang file");
                }
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
                Debug.Log("[JxLocalization] lang changed: " + currentLang);
                LanguageChanged?.Invoke();
            }
        }
        private static LocalizedTable currentTable;
        public static event Action LanguageChanged;

        public static bool EnableCaching { get; set; } = true;
        private static Dictionary<string, WeakReference<LocalizedTable>> tableCaching
            = new Dictionary<string, WeakReference<LocalizedTable>>();


        public static readonly string FolderName = "Lang";
        public static string ReadPath { get; } = System.Environment.CurrentDirectory + "/" + FolderName;

        static LocalizedTable LoadLang(string name)
        {
            var table = new LocalizedTable();
            table.OpenAndDeserialize(GetFilePath(name));
            return table;
        }

        static string GetFilePath(string name) => $"{ReadPath}/{name}.xml";

        static bool IsExistLang(string name) => File.Exists(GetFilePath(name));



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void UnityStaticConstructor()
        {
            isTriedUseSystemLangFile = false;
        }

        private static bool isTriedUseSystemLangFile;
        private static void TryUseSystemLangFile()
        {
            if (currentTable == null)
            {
                //try to find systemlang
                var langFilename_ = GetSystemLang().ToString();
                if (!IsExistLang(langFilename_))
                {
                    Debug.LogWarning("[JxLocalization] not found system lang file: " + langFilename_);
                }
                else
                {
                    CurrentLang = langFilename_;
                }
            }
            isTriedUseSystemLangFile = true;
        }

        public static string GetString(string id)
        {
            if (currentTable == null && !isTriedUseSystemLangFile)
            {
                TryUseSystemLangFile();
            }

            if (currentTable != null)
            {
                if (currentTable.Records.TryGetValue(id, out var record))
                {
                    return record;
                }
                Debug.LogWarning("[JxLocalization] not found lang record: " + id);
            }
            return null;
        }

        public static SystemLanguage GetSystemLang()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Chinese:
                    return SystemLanguage.ChineseSimplified;
            }
            return Application.systemLanguage;
        }
    }

}