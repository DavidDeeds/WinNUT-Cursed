using System.Net.Http;
using Microsoft.VisualBasic;
using Octokit;

namespace WinNUT_Client_Common.Updater;

public class UpdateUtil
{
    private const string RepositoryOwner = "DavidDeeds";
    private const string RepositoryName = "WinNUT-Cursed";
    private static readonly string UserAgentHeader = RepositoryName;
    private const int ProgressChangedDelayMs = 500;

    private Release? _latestRelease;
    private ReleaseAsset? _releaseAsset;

    public event EventHandler<UpdateCheckCompletedEventArgs>? UpdateCheckCompleted;
    public event EventHandler<UpdateDownloadProgressChangedEventArgs>? UpdateDownloadProgressChanged;
    public event EventHandler<UpdateDownloadCompletedEventArgs>? UpdateDownloadCompleted;

    public Release? LatestRelease
    {
        get => _latestRelease;
        private set
        {
            _latestRelease = value;
            _releaseAsset = value?.Assets.FirstOrDefault(a => a.Name.ToLowerInvariant().EndsWith(".msi"));
        }
    }

    public ReleaseAsset? LatestReleaseAsset
    {
        get => _releaseAsset;
        private set => _releaseAsset = value;
    }

    public async Task BeginUpdateCheck(bool acceptPreRelease)
    {
        try
        {
            var releases = await new GitHubClient(new ProductHeaderValue(UserAgentHeader)).Repository.Release.GetAll(
                RepositoryOwner, RepositoryName);

            foreach (var rel in releases)
            {
                if ((acceptPreRelease && rel.Prerelease) || !rel.Prerelease)
                {
                    LatestRelease = rel;
                    UpdateCheckCompleted?.Invoke(this, new UpdateCheckCompletedEventArgs(rel));
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            UpdateCheckCompleted?.Invoke(this, new UpdateCheckCompletedEventArgs(null, ex));
        }
    }

    public async Task BeginUpdateDownload()
    {
        if (_releaseAsset == null)
        {
            throw new InvalidOperationException("No release asset available to download.");
        }

        var downloadFilePath = Path.GetTempPath() + _releaseAsset.Name;

        if (File.Exists(downloadFilePath))
        {
            var fileInfo = new FileInfo(downloadFilePath);
            if (fileInfo.Length != _releaseAsset.Size)
            {
                File.Delete(downloadFilePath);
            }
            else
            {
                UpdateDownloadCompleted?.Invoke(this, new UpdateDownloadCompletedEventArgs(fileInfo));
                return;
            }
        }

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
            var response = await client.GetAsync(new Uri(_releaseAsset.BrowserDownloadUrl),
                HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using var fs = new FileStream(downloadFilePath, System.IO.FileMode.Create);
            using (var contentStream = await response.Content.ReadAsStreamAsync())
            {
                var buffer = new byte[4096];
                var totalBytesRead = 0;
                var nextProgressUpdate = DateTime.MinValue;

                int bytesRead;
                do
                {
                    bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    await fs.WriteAsync(buffer, 0, bytesRead);
                    totalBytesRead += bytesRead;

                    if (DateTime.Now >= nextProgressUpdate)
                    {
                        UpdateDownloadProgressChanged?.Invoke(this,
                            new UpdateDownloadProgressChangedEventArgs(totalBytesRead));
                        nextProgressUpdate = DateTime.Now.AddMilliseconds(ProgressChangedDelayMs);
                    }
                } while (bytesRead > 0);
            }

            UpdateDownloadCompleted?.Invoke(this, new UpdateDownloadCompletedEventArgs(new FileInfo(downloadFilePath)));
        }
        catch (Exception ex)
        {
            UpdateDownloadCompleted?.Invoke(this, new UpdateDownloadCompletedEventArgs(null, ex));
        }
    }

    public static bool UpdateCheckDelayPassed(int autoCheckDelay, DateTime lastChecked)
    {
        var delayVerif = autoCheckDelay switch
        {
            0 => DateInterval.Day,
            1 => DateInterval.Weekday,
            2 => DateInterval.Month,
            _ => DateInterval.Day
        };

        var diff = 1;
        if (lastChecked != DateTime.MinValue)
        {
            diff = (int)DateAndTime.DateDiff(delayVerif, lastChecked, DateTime.Now);
        }

        return diff >= 1;
    }
}
