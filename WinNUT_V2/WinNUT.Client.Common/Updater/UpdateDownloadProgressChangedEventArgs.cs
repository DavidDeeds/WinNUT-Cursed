namespace WinNUT_Client_Common.Updater;

public class UpdateDownloadProgressChangedEventArgs : EventArgs
{
    public int BytesDownloaded { get; }

    public UpdateDownloadProgressChangedEventArgs(int bytesDownloaded) => BytesDownloaded = bytesDownloaded;
}
