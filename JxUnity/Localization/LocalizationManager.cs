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
        }

        private static LocalizedTable currentTable;
        public static string CurrentLocale => currentTable?.Locale;

        public static event Action LanguageChanged;

        public static bool EnableCaching { get; set; } = true;
        private static Dictionary<string, WeakReference<LocalizedTable>> tableCaching
            = new Dictionary<string, WeakReference<LocalizedTable>>();


        public static readonly string FolderName = "Lang";
        public static string ReadPath => GetWritablePath() + "/" + FolderName;
        private static string GetWritablePath()
        {
            if (Application.isEditor)
            {
                return Application.streamingAssetsPath;
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

        static LocalizedTable LoadLangFile(string name)
        {
            var table = new LocalizedTable();
            table.OpenAndDeserialize(GetFilePath(name));
            return table;
        }
        public static void LoadLang(string value)
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
                    currentTable = LoadLangFile(value);
                    tableCaching.Add(value, new WeakReference<LocalizedTable>(currentTable));
                }
            }
            else
            {
                currentTable = LoadLangFile(value);
            }
            Debug.Log("[JxLocalization] lang changed: " + currentLang);
            LanguageChanged?.Invoke();
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
                var langFilename_ = GetLocaleDefaultFilename(GetSystemLocale());
                if (!IsExistLang(langFilename_))
                {
                    Debug.LogWarning("[JxLocalization] not found system lang file: " + langFilename_ + ", path: " + ReadPath);
                }
                else
                {
                    Debug.Log("[JxLocalization] find system lang file: " + langFilename_ + ", path: " + ReadPath);
                    LoadLang(langFilename_);
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

        private static Dictionary<string, string> localeDefaultFilename = new Dictionary<string, string>()
        {
            { "zh-CN", "简体中文-中国" },
            { "zh-TW", "繁體中文-台灣" },
            { "en-US", "English-US" }
        };

        public static string GetLocaleDefaultFilename(string locale)
        {
            if (localeDefaultFilename.TryGetValue(locale, out var value))
            {
                return value;
            }
            return locale;
        }

        public static string GetSystemLocale()
        {
            return System.Globalization.CultureInfo.InstalledUICulture.Name;
        }
    }

}