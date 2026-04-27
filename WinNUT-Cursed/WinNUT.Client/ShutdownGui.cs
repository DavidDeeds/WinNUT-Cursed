#nullable enable
using WinNUT_Client.Controls;
using WinNUT_Client_Common;

namespace WinNUT_Client;

public partial class ShutdownGui : Form
{
    public static ShutdownGui Instance { get; } = new();

    private bool _redText = true;
    private readonly CProgressBar _shutdownPBar = new();
    private DateTime _startShutdown;
    private double _offsetSTimer;
    private double _sTimer;
    private double _remained;

    public System.Windows.Forms.Timer Shutdown_Timer { get; } = new();
    public System.Windows.Forms.Timer Grace_Timer { get; } = new();

    private ShutdownGui()
    {
        InitializeComponent();
        Grace_Button.Click += Grace_Button_Click;
        Load += ShutdownGui_Load;
        Shown += ShutdownGui_Shown;
        FormClosing += ShutdownGui_FormClosing;
        Run_Timer.Tick += Run_Timer_Tick;
        ShutDown_Btn.Click += ShutDown_Btn_Click;
    }

    private static WinNUT? FindMainForm()
    {
        foreach (Form f in Application.OpenForms)
        {
            if (f is WinNUT w)
            {
                return w;
            }
        }

        return null;
    }

    private void Grace_Button_Click(object? sender, EventArgs e)
    {
        Shutdown_Timer.Stop();
        Shutdown_Timer.Enabled = false;
        Grace_Button.Enabled = false;
        Grace_Timer.Enabled = true;
        Grace_Timer.Start();
        _offsetSTimer = Properties.Settings.Default.PW_ExtendDelaySec;
    }

    private void ShutdownGui_Load(object? sender, EventArgs e)
    {
        var main = FindMainForm();
        if (main != null)
        {
            Icon = main.Icon;
        }

        WinNutGlobals.LogFile.LogTracing("Load ShutDown Gui", LogLvl.LOG_DEBUG, this);
        Grace_Timer.Enabled = false;
        Grace_Timer.Stop();
        Shutdown_Timer.Interval = Properties.Settings.Default.PW_StopDelaySec * 1000;
        _sTimer = Properties.Settings.Default.PW_StopDelaySec;
        _remained = _sTimer;
        if (Properties.Settings.Default.PW_UserExtendStopTimer)
        {
            Grace_Button.Enabled = true;
            try
            {
                Grace_Timer.Interval = Properties.Settings.Default.PW_ExtendDelaySec * 1000;
            }
            catch
            {
                Grace_Button.Enabled = false;
            }
        }
        else
        {
            Grace_Button.Enabled = false;
        }

        _startShutdown = DateTime.Now;
        _shutdownPBar.Location = new Point(10, 150);
        _shutdownPBar.Size = new Size(400, 23);
        _shutdownPBar.Style = ProgressBarStyle.Continuous;
        _shutdownPBar.Font = new Font(_shutdownPBar.Font, _shutdownPBar.Font.Style | FontStyle.Bold);
        _shutdownPBar.ForeColor = Color.Black;
        string timeToShow;
        var iSpan = TimeSpan.FromMilliseconds(Shutdown_Timer.Interval);
        if (Shutdown_Timer.Interval == 3600 * 1000)
        {
            timeToShow = iSpan.Hours.ToString().PadLeft(2, '0') + ":" +
                         iSpan.Minutes.ToString().PadLeft(2, '0') + ":" +
                         iSpan.Seconds.ToString().PadLeft(2, '0');
        }
        else
        {
            timeToShow = iSpan.Minutes.ToString().PadLeft(2, '0') + ":" +
                         iSpan.Seconds.ToString().PadLeft(2, '0');
        }

        _shutdownPBar.Text = timeToShow;
        _shutdownPBar.Value = 0;
        Controls.Add(_shutdownPBar);
        Grace_Timer.Tick += Grace_Timer_Tick;
        Shutdown_Timer.Tick += Shutdown_Timer_Tick;
    }

    private void ShutdownGui_Shown(object? sender, EventArgs e)
    {
        Shutdown_Timer.Enabled = true;
        Shutdown_Timer.Start();
        var main = FindMainForm();
        var statFmt = WinNutGlobals.StrLog[(int)AppResxStr.STR_SHUT_STAT];
        if (main != null)
        {
            var battCh = main.UPS_BattCh;
            lbl_UPSStatus.Text = string.Format(statFmt, battCh.ToString(), main.Lbl_VRTime.Text);
        }
        else
        {
            lbl_UPSStatus.Text = string.Format(statFmt, "?", "?");
        }
        WinNutGlobals.LogFile.LogTracing(
            "Shutdown GUI is shown and timer started for " + Shutdown_Timer.Interval / 1000 + " seconds.",
            LogLvl.LOG_NOTICE, this);
    }

    private void Grace_Timer_Tick(object? sender, EventArgs e)
    {
        Shutdown_Timer.Interval = (int)(_remained * 1000);
        Shutdown_Timer.Enabled = true;
        Shutdown_Timer.Start();
    }

    private void ShutDown_Btn_Click(object? sender, EventArgs e)
    {
        FindMainForm()?.Shutdown_Action();
    }

    private void Shutdown_Timer_Tick(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Shutdown timer tick.", LogLvl.LOG_NOTICE, this);
        _shutdownPBar.Value = 100;
        Thread.Sleep(1000);
        Run_Timer.Enabled = false;
        Shutdown_Timer.Stop();
        Shutdown_Timer.Enabled = false;
        Grace_Timer.Stop();
        Grace_Timer.Enabled = false;
        Hide();
        FindMainForm()?.Shutdown_Action();
        Close();
    }

    private void Run_Timer_Tick(object? sender, EventArgs e)
    {
        if (_redText)
        {
            lbl_UPSStatus.ForeColor = Color.Red;
            _redText = false;
        }
        else
        {
            lbl_UPSStatus.ForeColor = Color.Black;
            _redText = true;
        }

        if (Shutdown_Timer.Enabled && _remained > 0)
        {
            _remained = (int)(_sTimer + _offsetSTimer - DateTime.Now.Subtract(_startShutdown).TotalSeconds);
            var newValue = 100;
            if (_remained > 0)
            {
                newValue -= (int)(100 * (_remained / _sTimer));
                if (newValue > 100)
                {
                    newValue = 100;
                }
            }

            string timeToShow;
            var iSpan = TimeSpan.FromSeconds(_remained);
            if (Shutdown_Timer.Interval == 3600 * 1000)
            {
                timeToShow = iSpan.Hours.ToString().PadLeft(2, '0') + ":" +
                             iSpan.Minutes.ToString().PadLeft(2, '0') + ":" +
                             iSpan.Seconds.ToString().PadLeft(2, '0');
            }
            else
            {
                timeToShow = iSpan.Minutes.ToString().PadLeft(2, '0') + ":" +
                             iSpan.Seconds.ToString().PadLeft(2, '0');
            }

            _shutdownPBar.Text = timeToShow;
            _shutdownPBar.Value = newValue;
            var main = FindMainForm();
            var statFmt = WinNutGlobals.StrLog[(int)AppResxStr.STR_SHUT_STAT];
            if (main != null)
            {
                var battCh = main.UPS_BattCh;
                lbl_UPSStatus.Text = string.Format(statFmt, battCh.ToString(), main.Lbl_VRTime.Text);
            }
            else
            {
                lbl_UPSStatus.Text = string.Format(statFmt, "?", "?");
            }
        }
    }

    private void ShutdownGui_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (Visible)
        {
            e.Cancel = true;
        }
    }
}
