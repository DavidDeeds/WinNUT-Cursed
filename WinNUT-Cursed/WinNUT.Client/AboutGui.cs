#nullable enable
using System.Diagnostics;
using WinNUT_Client_Common;

namespace WinNUT_Client;

public partial class AboutGui : Form
{
    public static AboutGui Instance { get; } = new();

    private AboutGui()
    {
        InitializeComponent();
        Load += AboutGui_Load;
        Btn_OK.Click += Btn_OK_Click;
        LkLbl_Github.LinkClicked += LkLbl_Github_LinkClicked;
    }

    private void AboutGui_Load(object? sender, EventArgs e)
    {
        Lbl_ProgNameVersion.Text = WinNutGlobals.ProgramName + Environment.NewLine + "Version " +
                                   WinNutGlobals.ProgramVersion;
        Lbl_Copyright_2019.Text = WinNutGlobals.Copyright.Replace("©", Environment.NewLine + "©");
        LkLbl_Github.Text = WinNutGlobals.GitHubUrl;
        foreach (Form f in Application.OpenForms)
        {
            if (f is WinNUT main)
            {
                Icon = main.Icon;
                break;
            }
        }

        WinNutGlobals.LogFile.LogTracing("Load About Gui", LogLvl.LOG_DEBUG, this);
    }

    private void Btn_OK_Click(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Close About Gui", LogLvl.LOG_DEBUG, this);
        Close();
    }

    private void LkLbl_Github_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        if (sender is LinkLabel ll)
        {
            Process.Start(new ProcessStartInfo(ll.Text) { UseShellExecute = true });
        }
    }
}
