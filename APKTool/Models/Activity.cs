using System.Windows.Media;

namespace APKTool.Models;

public class Activity
{
    public enum ActivityType
    {
        APK,
        Script,
        Device
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Time => FormatRelativeDateTime(CreatedAt);
    public SolidColorBrush Background { get; set; }
    public Uri Icon { get; set; }


    public Activity(string name, string description, ActivityType type)
    {
        Name = name;
        Description = description;
        switch (type)
        {
            case ActivityType.APK:
                Icon = new Uri("Resources/Icons/Activity/apk.svg", UriKind.Relative);
                Background = new SolidColorBrush(Color.FromArgb(32,0,255,159));
                break;
            case ActivityType.Script:
                Icon = new Uri("Resources/Icons/Activity/script.svg", UriKind.Relative);
                break;
            case ActivityType.Device:
                Icon = new Uri("Resources/Icons/Activity/device.svg", UriKind.Relative);
                Background = new SolidColorBrush(Color.FromArgb(32, 253, 151, 31));
                break;
            default:
                break;
        }
    }

    private string FormatRelativeDateTime(DateTime dateTime)
    {
        var now = DateTime.Now;
        var date = dateTime.Date;
        var today = now.Date;
        var timePart = dateTime.ToString("HH:mm");

        if (date == today)
        {
            return $"Today, {timePart}";
        }
        else if (date == today.AddDays(-1))
        {
            return $"Yesterday, {timePart}";
        }
        else
        {
            int daysAgo = (today - date).Days;

            if (daysAgo < 7)
                return $"{daysAgo} days ago, {timePart}";
            else
                return dateTime.ToString("dd MMM yyyy, HH:mm");
        }
    }

}
