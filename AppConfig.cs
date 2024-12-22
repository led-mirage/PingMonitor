// © 2024 led-mirage. All rights reserved.

using System.Configuration;

public static class AppConfig
{
    private const int DefaultIntervalMs = 5000;
    private const int DefaultRetryCount = 3;
    private const int DefaultPingTimeoutMs = 500;
    private const string DefaultFontFamily = "Segoe UI";
    private const int DefaultFontSize = 10;

    public static int IntervalMs
    {
        get
        {
            return int.Parse(ConfigurationManager.AppSettings["IntervalMs"] ?? DefaultIntervalMs.ToString());
        }
    }

    public static int RetryCount
    {
        get
        {
            return int.Parse(ConfigurationManager.AppSettings["RetryCount"] ?? DefaultRetryCount.ToString());
        }
    }

    public static int PingTimeoutMs
    {
        get
        {
            return int.Parse(ConfigurationManager.AppSettings["PingTimeoutMs"] ?? DefaultPingTimeoutMs.ToString());
        }
    }

    public static string FontFamily
    {
        get
        {
            return ConfigurationManager.AppSettings["FontFamily"] ?? DefaultFontFamily;
        }
    }
    
    public static int FontSize
    {
        get
        {
            return int.Parse(ConfigurationManager.AppSettings["FontSize"] ?? DefaultFontSize.ToString());
        }
    }

}
