using System;

public class LocalizationManager : Singleton<LocalizationManager>, IDisposable
{
    private string languageType;
    public string LanguageType
    {
        get => languageType;
        set
        {
            languageType = value;
            LanguageChanged?.Invoke();
        }
    }

    public event Action LanguageChanged;

    public string GetString(string tableName, string id)
    {
        return string.Empty;
    }

}
