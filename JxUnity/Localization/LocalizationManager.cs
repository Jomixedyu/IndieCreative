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

        public static string GetString(string id)
        {
            if (currentTable == null)
            {
                //try to find systemlang
                var langFilename_ = GetSystemLang().ToString();
                if (!IsExistLang(langFilename_))
                {
                    throw new ArgumentNullException("not found system lang file.");
                }
                CurrentLang = langFilename_;
            }
            if (currentTable.Records.TryGetValue(id, out var record))
            {
                return record;
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