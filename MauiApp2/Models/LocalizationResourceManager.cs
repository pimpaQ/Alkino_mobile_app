using System.Globalization;
using System.Resources;

public class LocalizationResourceManager
{
    private readonly ResourceManager _resourceManager;

    public LocalizationResourceManager()
    {
        _resourceManager = new ResourceManager("MauiApp1.Resources.Strings.AppResources", typeof(LocalizationResourceManager).Assembly);
    }

    public string GetString(string key)
    {
        return _resourceManager.GetString(key, CultureInfo.CurrentUICulture) ?? key;
    }

    public void SetCulture(string languageCode)
    {
        CultureInfo.CurrentUICulture = new CultureInfo(languageCode);
    }
}
