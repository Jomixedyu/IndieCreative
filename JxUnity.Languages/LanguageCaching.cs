using System;
using System.Collections.Generic;
using UnityEngine;

namespace JxUnity.Languages
{
    internal static class LanguageCaching
    {
        public static bool EnableCaching { get; set; } = !Application.isEditor;

        internal static Dictionary<SystemLanguage, WeakReference<LanguageTable>> tableCaching;

        internal static void Init()
        {
            tableCaching = new Dictionary<SystemLanguage, WeakReference<LanguageTable>>();
        }
    }
}
