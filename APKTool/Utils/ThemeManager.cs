using System.Windows;

namespace APKTool.Utils;

public static class ThemeManager
{
    public static void SetTheme(bool isDark)
    {
        var appResources = Application.Current.Resources.MergedDictionaries;

        // Sadece kendi temalarını ayıkla ve değiştir
        var existingTheme = appResources
            .FirstOrDefault(d => d.Source != null &&
                (d.Source.OriginalString.Contains("CustomDarkTheme.xaml") ||
                 d.Source.OriginalString.Contains("CustomLightTheme.xaml")));

        if (existingTheme != null)
            appResources.Remove(existingTheme);

        var newTheme = new ResourceDictionary();
        newTheme.Source = new Uri(
            isDark ? "/Resources/Themes/CustomDarkTheme.xaml" : "/Resources/Themes/CustomLightTheme.xaml",
            UriKind.Relative);

        appResources.Add(newTheme);
    }
}

