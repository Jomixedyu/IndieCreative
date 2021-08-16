using System;


public static class LocalizationManager
{
    private static string languageType;
    public static string LanguageType
    {
        get => languageType;
        set
        {
            languageType = value;
            LanguageChanged?.Invoke();
        }
    }

    public static event Action LanguageChanged;

    public static string GetLangType()
    {
        return languageType;
    }

    public static string GetString(string id)
    {
        return string.Empty;
    }

}
