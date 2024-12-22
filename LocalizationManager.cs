// © 2024 led-mirage. All rights reserved.

using System.Globalization;
using System.Resources;

public static class LocalizationManager
{
    private static ResourceManager resourceManager = 
        new ResourceManager("PingMonitor.Properties.Resources", typeof(LocalizationManager).Assembly);

    private static CultureInfo currentCulture = CultureInfo.CurrentUICulture;

    public static void SetLanguage(string culture)
    {
        currentCulture = new CultureInfo(culture);
        Thread.CurrentThread.CurrentUICulture = currentCulture;
        Thread.CurrentThread.CurrentCulture = currentCulture;
    }

    public static string GetString(string name)
    {
        return resourceManager.GetString(name, currentCulture) ?? "";
    }
}