using Microsoft.Win32;

namespace APKTool.Services;

public interface IDialogService
{
    string OpenFile(string filter, string title);
    string OpenFolder(string title);
}

public class DialogService : IDialogService
{
    public string OpenFile(string filter, string title)
    {
        var dialog = new OpenFileDialog
        {
            Filter = filter,
            Title = title,
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = false
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string OpenFolder(string title)
    {
        var dialog = new OpenFolderDialog();

        return dialog.ShowDialog() == true ? dialog.FolderName : null;
    }
}
