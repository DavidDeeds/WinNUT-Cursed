using System.ComponentModel;

namespace WinNUT_Client_Common.Updater;

public class UpdateDownloadCompletedEventArgs : AsyncCompletedEventArgs
{
    public FileInfo? DownloadedFile { get; }

    public UpdateDownloadCompletedEventArgs(FileInfo? downloadedFile, Exception? error = null, bool cancelled = false)
        : base(error, cancelled, null)
    {
        DownloadedFile = downloadedFile;
    }
}
