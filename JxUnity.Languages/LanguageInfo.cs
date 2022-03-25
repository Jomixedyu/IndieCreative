
using UnityEngine;

namespace JxUnity.Languages
{
    public class LanguageInfo
    {
        public SystemLanguage Type { get; }
        public string Name { get; }
        public string DisplayName { get; }
        public string NameCode { get; }

        public LanguageInfo(SystemLanguage type, string displayName, string nameCode)
        {
            this.Type = type;
            this.Name = type.ToString();
            this.DisplayName = displayName;
            this.NameCode = nameCode;
        }
    }
}
