using System.ComponentModel;
using Octokit;

namespace WinNUT_Client_Common.Updater;

public class UpdateCheckCompletedEventArgs : AsyncCompletedEventArgs
{
    public Release? LatestRelease { get; }

    public UpdateCheckCompletedEventArgs(Release? latestRelease, Exception? error = null, bool cancelled = false)
        : base(error, cancelled, null)
    {
        LatestRelease = latestRelease;
    }
}
