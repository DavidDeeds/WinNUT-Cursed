using Microsoft.Toolkit.Uwp.Notifications;

namespace WinNUT_Client;

/// <summary>Sends Windows 10+ toast notifications (VB ToastPopup port).</summary>
public sealed class ToastPopup
{
    public void SendToast(string[] toastParts)
    {
        var toastBuilder = new ToastContentBuilder();
        for (var i = 0; i < toastParts.Length; i++)
        {
            toastBuilder.AddText(toastParts[i]);
        }

        toastBuilder.Show();
    }
}
