using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Languages
{
    internal class LanguageQuery
    {
        private List<LanguageTable> tables = new List<LanguageTable>(3);

        public SystemLanguage Language { get; private set; }

        private static LanguageTable GetTable(SystemLanguage lang)
        {
            if (LanguageCaching.EnableCaching)
            {
                if (LanguageCaching.tableCaching.TryGetValue(lang, out var weakCache)
                    && weakCache.TryGetTarget(out var cache))
                {
                    return cache;
                }
                else
                {
                    var tb = LoadLangFile(LanguageManager.GetLangName(lang));
                    LanguageCaching.tableCaching.Add(lang, new WeakReference<LanguageTable>(tb));
                    return tb;
                }
            }
            else
            {
                var tb = LoadLangFile(LanguageManager.GetLangName(lang));
                return tb;
            }
        }

        public LanguageQuery(SystemLanguage lang)
        {
            this.Language = lang;

            var table = GetTable(lang);
            tables.Add(table);

            while (table.Fallback != null)
            {
                if (!LanguageManager.HasLangName(table.Fallback))
                {
                    Debug.LogWarning($"[JxLanguage] fallback not found. lang: {lang}, fallback: {table.Fallback}");
                    break;
                }
                table = GetTable(LanguageManager.GetLangTypeByName(table.Fallback));
                tables.Add(table);
            }
        }

        private static LanguageTable LoadLangFile(string name)
        {
            var table = new LanguageTable();
            table.OpenAndDeserialize(LanguageManager.GetFilePath(name));
            return table;
        }

        public string GetString(string id)
        {
            if (this.tables[0].Records.TryGetValue(id, out string result))
            {
                return result;
            }
            else if (this.tables.Count == 1 && this.Language == LanguageManager.SystemLangFallback)
            {
                Debug.LogWarning($"[JxLanguage] lang record not found and no FALLBACK!. id: {id} lang: {this.Language}");
            }
            else
            {
                for (int i = 1; i < this.tables.Count; i++)
                {
                    if (this.tables[i].Records.TryGetValue(id, out string r))
                    {
                        return r;
                    }
                }
                Debug.LogWarning($"[JxLanguage] lang record not found. id: {id} lang: {this.Language}");
            }
            return null;
        }
    }
}
