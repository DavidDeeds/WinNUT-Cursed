#nullable enable
using System.Diagnostics;
using WinNUT_Client_Common;
using WinNUT_Client_Common.Updater;

namespace WinNUT_Client;

public partial class UpdateAvailableForm : Form
{
    public UpdateAvailableForm()
    {
        InitializeComponent();
        var rel = WinNutGlobals.UpdateController.LatestRelease;
        if (rel != null)
        {
            Title.Text = rel.Name;
        }

        foreach (Form f in Application.OpenForms)
        {
            if (f is WinNUT main)
            {
                Icon = main.Icon;
                break;
            }
        }

        TB_ChgLog.Text = rel?.Body ?? "";
        VisitPageButton.Click += VisitPageButton_Click;
        Update_Btn.Click += Update_Btn_Click;
        Close_Btn.Click += Close_Btn_Click;
        FormClosed += UpdateAvailableForm_FormClosed;
    }

    private void UpdateAvailableForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        WinNutGlobals.UpdateController.UpdateDownloadProgressChanged -= UpdateDownloadProgressChanged;
        WinNutGlobals.UpdateController.UpdateDownloadCompleted -= UpdateDownloadCompleted;
    }

    private void VisitPageButton_Click(object? sender, EventArgs e)
    {
        var url = WinNutGlobals.UpdateController.LatestRelease?.HtmlUrl;
        if (!string.IsNullOrEmpty(url))
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }

    private void Update_Btn_Click(object? sender, EventArgs e)
    {
        DownloadProgressPanel.Visible = true;
        WinNutGlobals.UpdateController.UpdateDownloadProgressChanged += UpdateDownloadProgressChanged;
        WinNutGlobals.UpdateController.UpdateDownloadCompleted += UpdateDownloadCompleted;
        _ = WinNutGlobals.UpdateController.BeginUpdateDownload();
    }

    private void UpdateDownloadProgressChanged(object? sender, UpdateDownloadProgressChangedEventArgs e)
    {
        var asset = WinNutGlobals.UpdateController.LatestReleaseAsset;
        if (asset is { Size: > 0 })
        {
            DownloadProgressBar.Value = (int)(e.BytesDownloaded * 100L / asset.Size);
            DownloadProgressBar.Text = string.Format("{0:F2} MB / {1:F2} MB", e.BytesDownloaded / 1048576.0,
                asset.Size / 1048576.0);
        }
    }

    private void UpdateDownloadCompleted(object? sender, UpdateDownloadCompletedEventArgs e)
    {
        if (e.DownloadedFile != null)
        {
            Process.Start(new ProcessStartInfo(e.DownloadedFile.FullName) { UseShellExecute = true });
        }

        Application.Exit();
    }

    private void Close_Btn_Click(object? sender, EventArgs e) => Close();
}
