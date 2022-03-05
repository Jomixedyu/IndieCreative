using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace JxUnity.Languages
{
    public static class LanguageManager
    {
        public static bool EnableCaching
        {
            get => LanguageCaching.EnableCaching;
            set => LanguageCaching.EnableCaching = value;
        }

        private static SystemLanguage currentLang;
        public static SystemLanguage CurrentLang
        {
            get => currentLang;
        }
        public static SystemLanguage SystemLangFallback { get; set; } = SystemLanguage.English;

        private static LanguageQuery currentTable;

        public static event Action LanguageChanged;

        public static readonly string FolderName = "Lang";
        public static string ReadPath => $"{GetWritablePath()}/{FolderName}";

        internal static string GetFilePath(string name) => $"{ReadPath}/{name}.xml";

        internal static string GetWritablePath()
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

        public static void LoadLang(SystemLanguage value)
        {
            if (!HasLangType(value))
            {
                throw new ArgumentOutOfRangeException("lang file not found: " + value.ToString());
            }

            currentLang = value;
            currentTable = new LanguageQuery(value);

            Debug.Log("[JxLanguage] lang changed: " + currentLang);

            LanguageChanged?.Invoke();
        }



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void UnityStaticConstructor()
        {
            isTriedUseSystemLangFile = false;
            supportLang = new List<LanguageInfo>();
            LanguageCaching.Init();
            ScanLang();
        }

        private static bool isTriedUseSystemLangFile;
        private static void TryUseSystemLangFile()
        {
            if (currentTable == null)
            {
                //try to find systemlang
                var sysLang = GetSystemLanguage();
                if (HasLangType(sysLang))
                {
                    Debug.Log($"[JxLanguage] load system lang file. path: {GetFilePath(sysLang.ToString())}");
                    LoadLang(sysLang);
                }
                else
                {
                    Debug.LogWarning("[JxLanguage] system lang file not found.");
                    if (SystemLangFallback != SystemLanguage.Unknown)
                    {
                        if (HasLangType(SystemLangFallback))
                        {
                            Debug.Log($"[JxLanguage] load system fallback lang file. path: {GetFilePath(sysLang.ToString())}");
                            LoadLang(SystemLangFallback);
                        }
                        else
                        {
                            Debug.LogWarning("[JxLanguage] system fallback lang file not found.");
                        }
                    }

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
                return currentTable.GetString(id);
            }
            return null;
        }


        private static readonly List<LanguageInfo> langInfos = new List<LanguageInfo>()
        {
            new LanguageInfo(SystemLanguage.Arabic,"العربية" , "arabic" ),
            new LanguageInfo(SystemLanguage.Bulgarian, "български език" , "bulgarian"),
            new LanguageInfo(SystemLanguage.ChineseSimplified, "简体中文" , "schinese"),
            new LanguageInfo(SystemLanguage.ChineseTraditional, "繁體中文" , "tchinese"),
            new LanguageInfo(SystemLanguage.Czech, "čeština" , "czech"),
            new LanguageInfo(SystemLanguage.Danish, "Dansk" , "danish"),
            new LanguageInfo(SystemLanguage.Dutch, "Nederlands" , "dutch"),
            new LanguageInfo(SystemLanguage.English, "English" , "english"),
            new LanguageInfo(SystemLanguage.Finnish, "Soumi" , "finnish"),
            new LanguageInfo(SystemLanguage.French, "Français" , "french"),
            new LanguageInfo(SystemLanguage.German, "Deutsch" , "german"),
            new LanguageInfo(SystemLanguage.Greek, "Ελληνικά" , "greek"),
            new LanguageInfo(SystemLanguage.Hungarian, "Magyar" , "hungarian"),
            new LanguageInfo(SystemLanguage.Italian, "Italiano" , "italian"),
            new LanguageInfo(SystemLanguage.Japanese, "日本語" , "japanese"),
            new LanguageInfo(SystemLanguage.Korean, "한국어" , "koreana"),
            new LanguageInfo(SystemLanguage.Norwegian, "Norsk" , "norwegian"),
            new LanguageInfo(SystemLanguage.Polish, "Polski" , "polish"),
            new LanguageInfo(SystemLanguage.Portuguese, "Português" , "portuguese"),
            new LanguageInfo(SystemLanguage.Romanian, "Română" , "romanian"),
            new LanguageInfo(SystemLanguage.Russian, "Русский" , "russian"),
            new LanguageInfo(SystemLanguage.Spanish, "Español-España" , "spanish"),
            new LanguageInfo(SystemLanguage.Swedish, "Svenska" , "swedish"),
            new LanguageInfo(SystemLanguage.Thai, "ไทย" , "thai"),
            new LanguageInfo(SystemLanguage.Turkish, "Türkçe" , "turkish"),
            new LanguageInfo(SystemLanguage.Ukrainian, "Українська" , "ukrainian"),
            new LanguageInfo(SystemLanguage.Vietnamese, "Tiếng Việt" , "vietnamese"),
        };
        public static IReadOnlyList<LanguageInfo> LangInfos => langInfos;

        public static LanguageInfo GetInfoByType(SystemLanguage type)
        {
            foreach (var item in langInfos)
            {
                if (item.Type == type)
                {
                    return item;
                }
            }
            return null;
        }
        public static LanguageInfo GetInfoByName(string name)
        {
            foreach (var item in langInfos)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
        public static LanguageInfo GetInfoByCode(string code)
        {
            foreach (var item in langInfos)
            {
                if (item.NameCode == code)
                {
                    return item;
                }
            }
            return null;
        }

        //quick query
        public static bool HasLangType(SystemLanguage type) => GetInfoByType(type) != null;
        public static bool HasLangCode(string code) => GetInfoByCode(code) != null;
        public static bool HasLangName(string name) => GetInfoByName(name) != null;
        public static string GetDisplayName(SystemLanguage type) => GetInfoByType(type)?.DisplayName;
        public static string GetLangName(SystemLanguage type) => GetInfoByType(type)?.Name;
        public static string GetLangCode(SystemLanguage type) => GetInfoByType(type)?.NameCode;
        public static SystemLanguage GetLangTypeByName(string langName) => GetInfoByName(langName)?.Type ?? SystemLanguage.Unknown;
        public static SystemLanguage GetLangTypeByCode(string code) => GetInfoByCode(code)?.Type ?? SystemLanguage.Unknown;


        private static List<LanguageInfo> supportLang;
        public static IReadOnlyList<LanguageInfo> SupportLang => supportLang;

        private static void ScanLang()
        {
            foreach (var info in LangInfos)
            {
                var path = $"{ReadPath}/{info.Name}.xml";
                if (File.Exists(path))
                {
                    supportLang.Add(info);
                }
            }
            Debug.Log($"[JxLanguage] count of scanned langfile: {supportLang.Count}");
        }


        public static SystemLanguage GetSystemLanguage()
        {
            var lang = Application.systemLanguage;

            switch (lang)
            {
                case SystemLanguage.Chinese:
                    return SystemLanguage.ChineseSimplified;
            }
            return lang;
        }
    }

}