using System;
using System.Collections.Generic;

namespace JxUnity.Subtitles
{
    //如果字幕语言为 简体中文-东北
    //并且一个扩展并没有该特化版本（东北版）的语言，那么将会退化为简体中文，如果简体中文不存在，那么将使用Config中的Fallback
    [Serializable]
    public class SubtitlesInfo
    {
        public string Id { get; set; }
        public SubtitlesLocale Locale { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string Date { get; set; }
    }
    [Serializable]
    public class SubtitlesLocale
    {
        //简体中文
        public string Locale { get; set; }
        //特化，如 东北话版
        public string Special { get; set; }

        public SubtitlesLocale()
        {

        }

        public SubtitlesLocale(string locale, string special)
        {
            this.Locale = locale;
            this.Special = special;
        }

        public string GetDisplayName()
        {
            return $"{Locale}_{Special}";
        }

        public override bool Equals(object obj)
        {
            return obj is SubtitlesLocale locale &&
                   this.Locale == locale.Locale &&
                   this.Special == locale.Special;
        }

        public override int GetHashCode()
        {
            int hashCode = 1641018399;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Locale);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Special);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{{Locale: {Locale}, Special: {Special}}}";
        }

    }

    public class SubtitlesTable
    {
        public SubtitlesInfo Info { get; set; }
        public Dictionary<string, string> Records { get; set; }
    }
}
