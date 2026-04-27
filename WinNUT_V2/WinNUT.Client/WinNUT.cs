using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Win32;
using WinNUT_Client.Properties;
using WinNUT_Client_Common;
using WinNUT_Client_Common.OldParams;
using WinNUT_Client_Common.Updater;

namespace WinNUT_Client;

public partial class WinNUT : Form
{
    private readonly Logger _log = WinNutGlobals.LogFile;
    private UpsDevice? _upsDevice;

    /// <summary>Write-only flag used by crash handler to suppress UI activation.</summary>
    public bool HasCrashed
    {
        set => _winNutCrashed = value;
    }

    public readonly ToastPopup ToastPopup = new();
    private readonly Version _windowsVersion = Environment.OSVersion.Version;
    private readonly Version _minOsVersionToast = Version.Parse("10.0.18362.0");
    private bool _allowToast;

    private int _lastAppIconIdx = -1;
    private int _actualAppIconIdx;
    private bool _winDarkMode;
    private bool _appDarkMode;

    public string UPS_Mfr = "";
    public string UPS_Model = "";
    public string UPS_Serial = "";
    public string UPS_Firmware = "";
    public double UPS_BattCh;
    public double UPS_BattV;
    public double UPS_BattRuntime;
    public double UPS_BattCapacity;
    public double UPS_InputF;
    public double UPS_InputV;
    public double UPS_OutputV;
    public double UPS_Load;
    public string UPS_Status = "";
    public double UPS_OutPower;
    public double UPS_InputA;

    private bool _hasFocus = true;
    private string _formTextCaption = "";
    private bool _winNutCrashed;

    private event Action<string?, string?>? UpdateNotifyIconStr;
    private event Action<string?>? UpdateBatteryState;
    private event Action? OnBattery;
    private event Action? OnLine;

    private bool _shutdownStatus;

    public WinNUT()
    {
        InitializeComponent();
        _formTextCaption = Text;

        Load += WinNUT_Load;
        Activated += WinNUT_Activated;
        Shown += WinNUT_Shown;
        FormClosing += WinNUT_FormClosing;
        Resize += WinNUT_Resize;
        Deactivate += WinNUT_Deactivate;

        Menu_Quit.Click += Menu_Quit_Click_1;
        Menu_Sys_Exit.Click += Menu_Sys_Exit_Click;
        NotifyIcon.MouseClick += NotifyIcon_MouseClick;
        NotifyIcon.MouseDoubleClick += NotifyIcon_MouseClick;
        Menu_About.Click += Menu_About_Click;
        Menu_Sys_About.Click += Menu_Sys_About_Click;
        Menu_Disconnect.Click += Menu_Disconnect_Click;
        Menu_Connect.Click += Menu_Reconnect_Click;
        Menu_Sys_Settings.Click += OpenPrefsForm;
        Menu_Settings.Click += OpenPrefsForm;
        ManageOldPrefsToolStripMenuItem.Click += ManageOldPrefsToolStripMenuItem_Click;
        Menu_Update.Click += Menu_Update_Click;
        Menu_Persist.CheckedChanged += Menu_Persist_CheckedChanged;
        Menu_UPS_Var.Click += Menu_UPS_Var_Click;

        UpdateNotifyIconStr += Event_UpdateNotifyIconStr;
        UpdateBatteryState += Event_UpdateBatteryState;
        OnBattery += ToastNotifyIcon;
        OnLine += ToastNotifyIcon;

        _log.DisplayedLogsLineAdded += AddLogLine;
        _log.DisplayedLogsTrimmed += TrimLogLine;

        Settings.Default.PropertyChanged += OnPropertyChanged;
        PrefGui.Instance.SavedPreferences += (_, _) => ResetUIState();
    }

    private static void StrInsert(AppResxStr key, string value) =>
        WinNutGlobals.StrLog.Insert((int)key, value);

    private static string StrItem(AppResxStr key) =>
        WinNutGlobals.StrLog[(int)key];

    private void WinNUT_Load(object? sender, EventArgs e)
    {
        StrInsert(AppResxStr.STR_MAIN_RECONNECT, Resources.Frm_Main_Str_03);
        StrInsert(AppResxStr.STR_MAIN_NOTCONN, Resources.Frm_Main_Str_05);
        StrInsert(AppResxStr.STR_MAIN_CONN, Resources.Frm_Main_Str_06);
        StrInsert(AppResxStr.STR_MAIN_OL, Resources.Frm_Main_Str_07);
        StrInsert(AppResxStr.STR_MAIN_OB, Resources.Frm_Main_Str_08);
        StrInsert(AppResxStr.STR_MAIN_LOWBAT, Resources.Frm_Main_Str_09);
        StrInsert(AppResxStr.STR_MAIN_BATOK, Resources.Frm_Main_Str_10);
        StrInsert(AppResxStr.STR_MAIN_UNKNOWN_UPS, Resources.Frm_Main_Str_11);
        StrInsert(AppResxStr.STR_MAIN_LOSTCONNECT, Resources.Frm_Main_Str_12);
        StrInsert(AppResxStr.STR_MAIN_INVALIDLOGIN, Resources.Frm_Main_Str_13);
        StrInsert(AppResxStr.STR_MAIN_EXITSLEEP, Resources.Frm_Main_Str_14);
        StrInsert(AppResxStr.STR_MAIN_GOTOSLEEP, Resources.Frm_Main_Str_15);
        StrInsert(AppResxStr.STR_SHUT_STAT, Resources.Frm_Shutdown_Str_01);
        StrInsert(AppResxStr.STR_APP_SHUT, Resources.App_Event_Str_01);
        StrInsert(AppResxStr.STR_LOG_PREFS, Resources.Log_Str_01);
        StrInsert(AppResxStr.STR_LOG_CONNECTED, Resources.Log_Str_02);
        StrInsert(AppResxStr.STR_LOG_CON_FAILED, Resources.Log_Str_03);
        StrInsert(AppResxStr.STR_LOG_CON_RETRY, Resources.Log_Str_04);
        StrInsert(AppResxStr.STR_LOG_LOGOFF, Resources.Log_Str_05);
        StrInsert(AppResxStr.STR_LOG_SHUT_START, Resources.Log_Str_08);
        StrInsert(AppResxStr.STR_LOG_SHUT_STOP, Resources.Log_Str_09);
        StrInsert(AppResxStr.STR_LOG_NO_UPDATE, Resources.Log_Str_10);
        StrInsert(AppResxStr.STR_LOG_UPDATE, Resources.Log_Str_11);
        StrInsert(AppResxStr.STR_LOG_NUT_FSD, Resources.Log_Str_12);

        NotifyIcon.Text = WinNutGlobals.ProgramName + " - " + WinNutGlobals.ShortProgramVersion;
        NotifyIcon.Visible = false;
        _log.LogTracing("NotifyIcons Initialised", LogLvl.LOG_DEBUG, this);

        if (_minOsVersionToast.CompareTo(_windowsVersion) < 0)
        {
            _allowToast = true;
            _log.LogTracing("Windows 10 Toast Notification Available", LogLvl.LOG_DEBUG, this);
        }
        else
        {
            _log.LogTracing(
                string.Format("Windows 10 Toast Notification Not Available. Required Version: {0}, Current: {1}",
                    _minOsVersionToast, _windowsVersion), LogLvl.LOG_NOTICE, this);
        }

        _appDarkMode = ReadCurrentUserDword(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "AppsUseLightTheme", 1) == 0;
        _log.LogTracing(_appDarkMode ? "Windows App Use Dark Theme" : "Windows App Use Light Theme", LogLvl.LOG_DEBUG,
            this);

        _winDarkMode = ReadCurrentUserDword(
            @"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize", "SystemUsesLightTheme", 1) == 0;
        _log.LogTracing(_winDarkMode ? "Windows Use Dark Theme" : "Windows Use Light Theme", LogLvl.LOG_DEBUG, this);

        var startAppIcon = _appDarkMode
            ? (int)(AppIconIdx.IDX_ICO_OFFLINE | AppIconIdx.WIN_DARK | AppIconIdx.IDX_OFFSET)
            : (int)(AppIconIdx.IDX_ICO_OFFLINE | AppIconIdx.IDX_OFFSET);
        Icon = GetIcon(startAppIcon);
        var startTrayIcon = _winDarkMode
            ? (int)(AppIconIdx.IDX_ICO_OFFLINE | AppIconIdx.WIN_DARK | AppIconIdx.IDX_OFFSET)
            : (int)(AppIconIdx.IDX_ICO_OFFLINE | AppIconIdx.IDX_OFFSET);

        NotifyIcon.Visible = false;
        NotifyIcon.Icon = GetIcon(startTrayIcon);
        UpdateNotifyIconStr?.Invoke(null, null);
        _actualAppIconIdx = startAppIcon;
        UpdateIcon_NotifyIcon();
        _log.LogTracing("Update Icon at Startup", LogLvl.LOG_DEBUG, this);

        UpdateMainMenuState();
        ReInitDisplayValues();

        foreach (var line in _log.DisplayedLogs)
        {
            AddLogLine(line);
        }

        WinNutGlobals.UpdateController.UpdateCheckCompleted += OnCheckForUpdateCompleted;
        if (Settings.Default.UP_CheckAtStart &&
            UpdateUtil.UpdateCheckDelayPassed(Settings.Default.UP_AutoChkDelay, Settings.Default.UP_LastCheck))
        {
            _log.LogTracing("Auto update delay passed, checking for updates...", LogLvl.LOG_DEBUG, this);
            StartUpdateCheck();
        }

        SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

#if DEBUG
        InsertDebugMenu();
#endif

        _log.LogTracing("WinNUT Form completed Load.", LogLvl.LOG_NOTICE, this);
    }

    private static int ReadCurrentUserDword(string subKey, string name, int defaultIfMissing)
    {
        using var key = Registry.CurrentUser.OpenSubKey(subKey);
        var v = key?.GetValue(name);
        return v is int i ? i : defaultIfMissing;
    }

#if DEBUG
    private void InsertDebugMenu()
    {
        var testFillAndTrimCommand = new ToolStripMenuItem("Test Fill and Trim");
        testFillAndTrimCommand.Click += (_, _) =>
        {
            for (var i = _log.DisplayedLogs.Count; i <= Logger.MaxDisplayedLogs + 3; i++)
            {
                _log.LogTracing("Test logging line " + i, LogLvl.LOG_DEBUG, this, "Test logging line " + i);
            }
        };
        var logDisplaySubmenu = new ToolStripMenuItem("LogDisplay");
        logDisplaySubmenu.DropDownItems.Add(testFillAndTrimCommand);
        var debugMenu = new ToolStripMenuItem("Debug");
        debugMenu.DropDownItems.Add(logDisplaySubmenu);
        Main_Menu.Items.Add(debugMenu);
        _log.LogTracing("Inserted debug menu to Main_Menu.", LogLvl.LOG_DEBUG, this, "Debug Menu enabled.");
    }
#endif

    private void WinNUT_Activated(object? sender, EventArgs e)
    {
        _log.LogTracing("Main GUI activated.", LogLvl.LOG_DEBUG, this);
        if (_winNutCrashed)
        {
            Hide();
            return;
        }

        _hasFocus = true;
        var tmpAppMode = _appDarkMode
            ? (int)(AppIconIdx.WIN_DARK | AppIconIdx.IDX_OFFSET)
            : (int)AppIconIdx.IDX_OFFSET;
        var tmpGuiIdx = _actualAppIconIdx | tmpAppMode;
        Icon = GetIcon(tmpGuiIdx);
        _log.LogTracing("Update Icon", LogLvl.LOG_DEBUG, this);
        UpdateIcon_NotifyIcon();
    }

    private void WinNUT_Shown(object? sender, EventArgs e)
    {
        _log.LogTracing("WinNUT_Shown for the first time.", LogLvl.LOG_DEBUG, this);
        UpdateIcon_NotifyIcon();
        if (Settings.Default.MinimizeToTray && Settings.Default.MinimizeOnStart)
        {
            _log.LogTracing("Minimize WinNut On Start", LogLvl.LOG_DEBUG, this);
            WindowState = FormWindowState.Minimized;
            NotifyIcon.Visible = true;
        }
        else
        {
            _log.LogTracing("Show WinNut Main Gui", LogLvl.LOG_DEBUG, this);
            NotifyIcon.Visible = false;
        }

        if (Settings.Default.NUT_AutoReconnect)
        {
            _log.LogTracing("Auto-connecting to UPS on startup.", LogLvl.LOG_NOTICE, this);
            UPS_Connect(true);
        }
    }

    private void WinNUT_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _log.LogTracing("Received FormClosing event. Reason: " + e.CloseReason, LogLvl.LOG_NOTICE, this);
        if (e.CloseReason == CloseReason.UserClosing && Settings.Default.CloseToTray && Settings.Default.MinimizeToTray)
        {
            UpdateIcon_NotifyIcon();
            _log.LogTracing("Minimize Main Gui To Notify Icon", LogLvl.LOG_DEBUG, this);
            WindowState = FormWindowState.Minimized;
            Visible = false;
            NotifyIcon.Visible = true;
            e.Cancel = true;
        }
        else
        {
            _log.LogTracing("Init Disconnecting Before Close WinNut", LogLvl.LOG_DEBUG, this);
            UPSDisconnect();
        }
    }

    private void WinNUT_Resize(object? sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized)
        {
            if (Settings.Default.MinimizeToTray)
            {
                UpdateIcon_NotifyIcon();
                _log.LogTracing("Minimize Main Gui To Notify Icon", LogLvl.LOG_DEBUG, this);
                WindowState = FormWindowState.Minimized;
                Visible = false;
                NotifyIcon.Visible = true;
            }

            Text = !NotifyIcon.Visible ? _formTextCaption : WinNutGlobals.ProgramName;
        }
        else if (WindowState is FormWindowState.Maximized or FormWindowState.Normal)
        {
            Text = WinNutGlobals.ProgramName;
        }
    }

    private void WinNUT_Deactivate(object? sender, EventArgs e)
    {
        if (_winNutCrashed)
        {
            Hide();
        }
        else
        {
            _log.LogTracing("Main Gui Lose Focus", LogLvl.LOG_DEBUG, this);
            _hasFocus = false;
            var tmpAppMode = !_appDarkMode
                ? (int)(AppIconIdx.WIN_DARK | AppIconIdx.IDX_OFFSET)
                : (int)AppIconIdx.IDX_OFFSET;
            var tmpGuiIdx = _actualAppIconIdx | tmpAppMode;
            Icon = GetIcon(tmpGuiIdx);
            UpdateIcon_NotifyIcon();
        }
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.NUT_AutoReconnect))
        {
            _log.LogTracing("Handling OnPropertyChanged for " + e.PropertyName, LogLvl.LOG_DEBUG, this);
            UpdateMainMenuState();
        }
    }

    private void UpdateMainMenuState()
    {
        if (WinNUT_Params.ParamsExist)
        {
            ManageOldPrefsToolStripMenuItem.Enabled = true;
            ManageOldPrefsToolStripMenuItem.ToolTipText = Resources.ManageOldPrefsToolstripMenuItem_Enabled_TooltipText;
        }
        else
        {
            ManageOldPrefsToolStripMenuItem.Enabled = false;
            ManageOldPrefsToolStripMenuItem.ToolTipText = Resources.ManageOldPrefsToolstripMenuItem_Disabled_TooltipText;
        }

        Menu_Persist.Checked = Settings.Default.NUT_AutoReconnect;
        Menu_UPS_Var.Enabled = _upsDevice?.IsConnected == true;
        Menu_Disconnect.Enabled = _upsDevice?.IsConnected == true || _upsDevice?.IsReconnecting == true;
        Menu_Connect.Enabled = !Menu_Disconnect.Enabled;
    }

    private void SystemEvents_PowerModeChanged(object? sender, PowerModeChangedEventArgs e)
    {
        _log.LogTracing("PowerModeChangedEvent: " + Enum.GetName(typeof(PowerModes), e.Mode), LogLvl.LOG_NOTICE, this);
        switch (e.Mode)
        {
            case PowerModes.Suspend:
                _log.LogTracing("Suspending WinNUT operations...", LogLvl.LOG_NOTICE, this,
                    StrItem(AppResxStr.STR_MAIN_GOTOSLEEP));
                UPSDisconnect();
                break;
            case PowerModes.Resume:
                if (_upsDevice is { IsConnected: true })
                {
                    _log.LogTracing("Trying to disconnect connected UPS after system resume...", LogLvl.LOG_NOTICE, this);
                    UPSDisconnect();
                }

                if (Settings.Default.NUT_AutoReconnect)
                {
                    _log.LogTracing("Reconnecting after system resume.", LogLvl.LOG_NOTICE, this,
                        StrItem(AppResxStr.STR_MAIN_EXITSLEEP));
                    UPS_Connect(true);
                }

                break;
        }
    }

    private void UPS_Connect(bool retryOnConnFailure = false)
    {
        _log.LogTracing("Client UPS_Connect subroutine beginning.", LogLvl.LOG_NOTICE, this);
        var nutConfig = new NutParameter(
            Settings.Default.NUT_ServerAddress,
            Settings.Default.NUT_ServerPort,
            Settings.Default.NUT_Username?.ToString() ?? "",
            Settings.Default.NUT_Password?.ToString() ?? "",
            Settings.Default.NUT_UPSName,
            Settings.Default.NUT_AutoReconnect);

        UnwireUpsDeviceEvents();
        _upsDevice = new UpsDevice(nutConfig, _log, Settings.Default.NUT_PollIntervalMsec, Settings.Default.CAL_FreqInNom);
        WireUpsDeviceEvents();
        _upsDevice.Connect_UPS(retryOnConnFailure);
        UpdateMainMenuState();
    }

    private void WireUpsDeviceEvents()
    {
        if (_upsDevice == null)
        {
            return;
        }

        _upsDevice.Connected += UPSReady;
        _upsDevice.ConnectionError += ConnectionError;
        _upsDevice.Lost_Connect += UPS_Lostconnect;
        _upsDevice.Disconnected += UPSDisconnectedEvent;
        _upsDevice.DataUpdated += Update_UPS_Data;
        _upsDevice.StatusesChanged += HandleUPSStatusChange;
        _upsDevice.EncounteredNUTException += HandleNUTException;
        _upsDevice.Connected += OnUpsDeviceConnectedToast;
        _upsDevice.Lost_Connect += OnUpsDeviceToastHookPlain;
        _upsDevice.Disconnected += OnUpsDeviceToastHookPlain;
    }

    private void OnUpsDeviceConnectedToast(UpsDevice _) => ToastNotifyIcon();

    private void OnUpsDeviceToastHookPlain() => ToastNotifyIcon();

    private void UnwireUpsDeviceEvents()
    {
        if (_upsDevice == null)
        {
            return;
        }

        _upsDevice.Connected -= UPSReady;
        _upsDevice.ConnectionError -= ConnectionError;
        _upsDevice.Lost_Connect -= UPS_Lostconnect;
        _upsDevice.Disconnected -= UPSDisconnectedEvent;
        _upsDevice.DataUpdated -= Update_UPS_Data;
        _upsDevice.StatusesChanged -= HandleUPSStatusChange;
        _upsDevice.EncounteredNUTException -= HandleNUTException;
        _upsDevice.Connected -= OnUpsDeviceConnectedToast;
        _upsDevice.Lost_Connect -= OnUpsDeviceToastHookPlain;
        _upsDevice.Disconnected -= OnUpsDeviceToastHookPlain;
    }

    private void UPSReady(UpsDevice nutUps)
    {
        var upsConf = nutUps.Nut_Config;
        _log.LogTracing(upsConf.UPSName + " has indicated it's ready to start sending data.", LogLvl.LOG_DEBUG, this);
        var d = nutUps.UPS_Datas;
        Lbl_VMfr.Text = d.Mfr;
        Lbl_VName.Text = d.Model;
        Lbl_VSerial.Text = d.Serial;
        Lbl_VFirmware.Text = d.Firmware;

        UpdateMainMenuState();
        UpdateIcon_NotifyIcon();
        UpdateNotifyIconStr?.Invoke("Connected", null);
        _log.LogTracing("Connection to Nut Host Established", LogLvl.LOG_NOTICE, this,
            string.Format(StrItem(AppResxStr.STR_LOG_CONNECTED), upsConf.Host, upsConf.Port));
    }

    private void ConnectionError(UpsDevice sender, Exception ex)
    {
        if (!sender.IsReconnecting)
        {
            _log.LogTracing(
                string.Format("Something went wrong connecting to UPS {0}. IsConnected: {1}, IsLoggedIn: {2}",
                    sender.Name, sender.IsConnected, sender.IsLoggedIn), LogLvl.LOG_ERROR, this,
                string.Format(StrItem(AppResxStr.STR_LOG_CON_FAILED), sender.Nut_Config.Host, sender.Nut_Config.Port,
                    ex.Message));
        }
    }

    private void UPSDisconnect()
    {
        _log.LogTracing("Running Client disconnect subroutine.", LogLvl.LOG_DEBUG, this);
        if (_upsDevice != null)
        {
            UnwireUpsDeviceEvents();
            _upsDevice.Disconnect(true);
        }
        else
        {
            _log.LogTracing("Attempted to disconnect when UPS_Device is Nothing.", LogLvl.LOG_ERROR, this);
        }
    }

    private void UPS_Lostconnect()
    {
        if (_upsDevice == null)
        {
            return;
        }

        _log.LogTracing("UPS reports lost (broken) connection.", LogLvl.LOG_ERROR, this,
            string.Format(StrItem(AppResxStr.STR_MAIN_LOSTCONNECT), _upsDevice.Nut_Config.Host,
                _upsDevice.Nut_Config.Port));

        ReInitDisplayValues();
        if (_upsDevice.Nut_Config.AutoReconnect)
        {
            _actualAppIconIdx = (int)AppIconIdx.IDX_ICO_RETRY;
            var message = string.Format(StrItem(AppResxStr.STR_MAIN_RECONNECT));
            _log.LogTracing("UPS reports it lost connection and is retrying.", LogLvl.LOG_WARNING, this, message);
            UpdateNotifyIconStr?.Invoke("Retry", message);
        }
        else
        {
            _actualAppIconIdx = (int)AppIconIdx.IDX_ICO_OFFLINE;
            UpdateNotifyIconStr?.Invoke("Lost Connect", null);
        }

        UpdateIcon_NotifyIcon();
        UpdateBatteryState?.Invoke("Lost Connect");
        UpdateMainMenuState();
    }

    private void UPSDisconnectedEvent()
    {
        ReInitDisplayValues();
        UpdateMainMenuState();
        _actualAppIconIdx = (int)AppIconIdx.IDX_ICO_OFFLINE;
        UpdateIcon_NotifyIcon();
        UpdateNotifyIconStr?.Invoke("Deconnected", null);
        UpdateBatteryState?.Invoke("Deconnected");
        _log.LogTracing("Disconnected from Nut Host", LogLvl.LOG_NOTICE, this, StrItem(AppResxStr.STR_LOG_LOGOFF));
    }

    private void Menu_Quit_Click_1(object? sender, EventArgs e)
    {
        _log.LogTracing("Close WinNut From Menu Quit", LogLvl.LOG_DEBUG, this);
        Application.Exit();
    }

    private void Menu_Sys_Exit_Click(object? sender, EventArgs e)
    {
        _log.LogTracing("Close WinNut From Systray", LogLvl.LOG_DEBUG, this);
        Application.Exit();
    }

    private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            _log.LogTracing("Restore Main Gui On Mouse Click Notify Icon", LogLvl.LOG_DEBUG, this);
            Visible = true;
            NotifyIcon.Visible = false;
            WindowState = FormWindowState.Normal;
        }
    }

    private void Event_UpdateNotifyIconStr(string? reason, string? message)
    {
        var showVersion = WinNutGlobals.ShortProgramVersion;
        var notifyStr = WinNutGlobals.ProgramName + " - " + showVersion + Environment.NewLine;
        var formText = WinNutGlobals.ProgramName;
        switch (reason)
        {
            case null:
                if (_upsDevice is not { IsConnected: true })
                {
                    notifyStr += StrItem(AppResxStr.STR_MAIN_NOTCONN);
                    formText += " - " + StrItem(AppResxStr.STR_MAIN_NOTCONN);
                }

                break;
            case "Retry":
                notifyStr += StrItem(AppResxStr.STR_MAIN_RECONNECT) + Environment.NewLine;
                notifyStr += message;
                formText += " - Bat: " + UPS_BattCh + "% - " + StrItem(AppResxStr.STR_MAIN_RECONNECT) + " - " + message;
                break;
            case "Connected":
                notifyStr += StrItem(AppResxStr.STR_MAIN_CONN);
                formText += " - Bat: " + UPS_BattCh + "% - " + StrItem(AppResxStr.STR_MAIN_CONN);
                break;
            case "Deconnected":
                notifyStr += StrItem(AppResxStr.STR_MAIN_NOTCONN);
                formText += " - " + StrItem(AppResxStr.STR_MAIN_NOTCONN);
                break;
            case "Unknown UPS":
                notifyStr += StrItem(AppResxStr.STR_MAIN_UNKNOWN_UPS);
                formText += " - " + StrItem(AppResxStr.STR_MAIN_UNKNOWN_UPS);
                break;
            case "Lost Connect":
                if (_upsDevice != null)
                {
                    notifyStr += string.Format(StrItem(AppResxStr.STR_MAIN_LOSTCONNECT), _upsDevice.Nut_Config.Host,
                        _upsDevice.Nut_Config.Port);
                    formText += " - " + string.Format(StrItem(AppResxStr.STR_MAIN_LOSTCONNECT),
                        _upsDevice.Nut_Config.Host, _upsDevice.Nut_Config.Port);
                }

                break;
            case "Update Data":
                formText += " - Bat: " + UPS_BattCh + "% - " + StrItem(AppResxStr.STR_MAIN_CONN) + " - ";
                notifyStr += StrItem(AppResxStr.STR_MAIN_CONN) + Environment.NewLine;
                if (_upsDevice != null)
                {
                    if (_upsDevice.UPS_Datas.UPS_Value.UPS_Status.HasFlag(UPS_States.OL))
                    {
                        notifyStr += StrItem(AppResxStr.STR_MAIN_OL) + Environment.NewLine;
                        formText += StrItem(AppResxStr.STR_MAIN_OL) + " - ";
                    }
                    else
                    {
                        notifyStr += string.Format(StrItem(AppResxStr.STR_MAIN_OB),
                                      _upsDevice.UPS_Datas.UPS_Value.Batt_Charge) +
                                  Environment.NewLine;
                        formText += string.Format(StrItem(AppResxStr.STR_MAIN_OB),
                                      _upsDevice.UPS_Datas.UPS_Value.Batt_Charge) +
                                  " - ";
                    }

                    switch (_upsDevice.UPS_Datas.UPS_Value.Batt_Charge)
                    {
                        case >= 0 and <= 40:
                            notifyStr += StrItem(AppResxStr.STR_MAIN_LOWBAT);
                            formText += StrItem(AppResxStr.STR_MAIN_LOWBAT);
                            break;
                        case > 40 and <= 100:
                            notifyStr += StrItem(AppResxStr.STR_MAIN_BATOK);
                            formText += StrItem(AppResxStr.STR_MAIN_BATOK);
                            break;
                    }
                }

                break;
        }

        if (notifyStr.Length > 63)
        {
            notifyStr = notifyStr.Substring(0, 60) + "...";
        }

        NotifyIcon.Text = notifyStr;
        if (WindowState == FormWindowState.Minimized && !NotifyIcon.Visible)
        {
            Text = formText;
        }
        else
        {
            Text = WinNutGlobals.ProgramName;
        }

        _formTextCaption = formText;
        _log.LogTracing("NotifyIcon Text => " + Environment.NewLine + notifyStr, LogLvl.LOG_DEBUG, this);
    }

    private void Event_UpdateBatteryState(string? reason)
    {
        var status = "Unknown";
        switch (reason)
        {
            case null:
            case "Deconnected":
            case "Lost Connect":
                if (_upsDevice is not { IsConnected: true })
                {
                    PBox_Battery_State.Image = null;
                }

                status = "Unknown";
                break;
            case "Update Data":
                if (UPS_BattCh == 100)
                {
                    PBox_Battery_State.Image = Resources.Battery_Charged;
                    status = "Charged";
                }
                else
                {
                    var t = UPS_Status.Trim();
                    if (t.StartsWith("OL", StringComparison.Ordinal) ||
                        StrReverse(t).StartsWith("LO", StringComparison.Ordinal))
                    {
                        PBox_Battery_State.Image = Resources.Battery_Charging;
                        status = "Charging";
                    }
                    else
                    {
                        PBox_Battery_State.Image = Resources.Battery_Discharging;
                        status = "Discharging";
                    }
                }

                break;
        }

        _log.LogTracing("Battery Status => " + status, LogLvl.LOG_DEBUG, this);
    }

    private static string StrReverse(string s)
    {
        var arr = s.ToCharArray();
        Array.Reverse(arr);
        return new string(arr);
    }

    private void HandleNUTException(UpsDevice sender, NutException ex)
    {
        if (ex.LastTransaction.ResponseType == NUTResponse.UNKNOWNUPS)
        {
            Event_Unknown_UPS();
        }

        _log.LogTracing("NUT protocol error encoutnered:", LogLvl.LOG_NOTICE, sender);
        _log.LogException(ex, this);
    }

    public void Event_Unknown_UPS()
    {
        _actualAppIconIdx = (int)AppIconIdx.IDX_ICO_OFFLINE;
        UpdateIcon_NotifyIcon();
        UpdateNotifyIconStr?.Invoke("Unknown UPS", null);
        _log.LogTracing("Unknow UPS Name", LogLvl.LOG_ERROR, this, StrItem(AppResxStr.STR_MAIN_UNKNOWN_UPS));
        Menu_UPS_Var.Enabled = false;
    }

    private void Menu_About_Click(object? sender, EventArgs e)
    {
        _log.LogTracing("Open About Gui From Menu", LogLvl.LOG_DEBUG, this);
        AboutGui.Instance.Activate();
        AboutGui.Instance.Visible = true;
        _hasFocus = false;
    }

    private void Menu_Sys_About_Click(object? sender, EventArgs e)
    {
        _log.LogTracing("Open About Gui From Systray", LogLvl.LOG_DEBUG, this);
        AboutGui.Instance.Activate();
        AboutGui.Instance.Visible = true;
        _hasFocus = false;
    }

    private void Update_UPS_Data()
    {
        if (_upsDevice == null)
        {
            return;
        }

        _log.LogTracing("Updating UPS data for Form.", LogLvl.LOG_DEBUG, this);
        var v = _upsDevice.UPS_Datas.UPS_Value;
        UPS_BattCh = v.Batt_Charge;
        UPS_BattV = v.Batt_Voltage;
        UPS_BattRuntime = v.Batt_Runtime;
        UPS_BattCapacity = v.Batt_Capacity;
        UPS_InputF = v.Power_Frequency;
        UPS_InputV = v.Input_Voltage;
        UPS_OutputV = v.Output_Voltage;
        UPS_Load = v.Load;
        UPS_Status = v.UPS_Status.ToString();
        UPS_OutPower = v.Output_Power;

        if (v.UPS_Status.HasFlag(UPS_States.OL))
        {
            Lbl_VOL.BackColor = Color.Green;
            Lbl_VOB.BackColor = Color.White;
            _actualAppIconIdx = (int)AppIconIdx.IDX_OL;
        }
        else if (v.UPS_Status.HasFlag(UPS_States.OB))
        {
            Lbl_VOL.BackColor = Color.Yellow;
            Lbl_VOB.BackColor = Color.Green;
            _actualAppIconIdx = 0;

            if (v.Batt_Charge == -1 && v.Batt_Runtime == -1)
            {
                _log.LogTracing("Battery properties unavailable, unable to validate shutdown conditions.",
                    LogLvl.LOG_WARNING, this);
            }
            else if (!_shutdownStatus)
            {
                if (v.Batt_Charge != -1 && v.Batt_Charge <= Settings.Default.PW_BattChrgFloor ||
                    v.Batt_Runtime != -1 && v.Batt_Runtime <= Settings.Default.PW_RuntimeFloor)
                {
                    _log.LogTracing("UPS battery has dropped below stop condition limits.", LogLvl.LOG_NOTICE, this,
                        StrItem(AppResxStr.STR_LOG_SHUT_START));
                    Shutdown_Event();
                }
                else
                {
                    _log.LogTracing(
                        string.Format(
                            "UPS charge ({0}%) or Runtime ({1}) have not met shutdown conditions {2} or {3}.",
                            v.Batt_Charge, v.Batt_Runtime, Settings.Default.PW_BattChrgFloor,
                            Settings.Default.PW_RuntimeFloor),
                        LogLvl.LOG_DEBUG, this);
                }
            }
        }

        if (v.UPS_Status.HasFlag(UPS_States.OVER))
        {
            Lbl_VOLoad.BackColor = Color.Red;
        }
        else
        {
            Lbl_VOLoad.BackColor = Color.White;
        }

        _log.LogTracing("Updating battery icons based on charge percent: " + UPS_BattCh + "%", LogLvl.LOG_DEBUG, this);

        switch (UPS_BattCh)
        {
            case >= 76 and <= 100:
                Lbl_VBL.BackColor = Color.White;
                _actualAppIconIdx |= (int)AppIconIdx.IDX_BATT_100;
                break;
            case >= 51 and <= 75:
                Lbl_VBL.BackColor = Color.White;
                _actualAppIconIdx |= (int)AppIconIdx.IDX_BATT_75;
                break;
            case >= 40 and <= 50:
                Lbl_VBL.BackColor = Color.White;
                _actualAppIconIdx |= (int)AppIconIdx.IDX_BATT_50;
                break;
            case >= 26 and <= 39:
                Lbl_VBL.BackColor = Color.Red;
                _actualAppIconIdx |= (int)AppIconIdx.IDX_BATT_50;
                break;
            case >= 11 and <= 25:
                Lbl_VBL.BackColor = Color.Red;
                _actualAppIconIdx |= (int)AppIconIdx.IDX_BATT_25;
                break;
            case >= 0 and <= 10:
                Lbl_VBL.BackColor = Color.Red;
                _actualAppIconIdx |= (int)AppIconIdx.IDX_BATT_0;
                break;
        }

        if (UPS_BattRuntime >= 0 && UPS_BattRuntime <= 86400)
        {
            var iSpan = TimeSpan.FromSeconds(UPS_BattRuntime);
            _log.LogTracing("Calculated estimated remaining battery time: " + iSpan, LogLvl.LOG_DEBUG, this);
            Lbl_VRTime.Text = iSpan.ToString("g", CultureInfo.CurrentCulture);
        }
        else
        {
            Lbl_VRTime.Text = Resources.VariableUnavailable;
        }

        AG_InV.Value1 = (float)UPS_InputV;
        AG_InF.Value1 = (float)UPS_InputF;
        AG_OutV.Value1 = (float)UPS_OutputV;
        AG_BattCh.Value1 = (float)UPS_BattCh;
        AG_Load.Value1 = (float)UPS_Load;
        AG_Load.Value2 = (float)UPS_OutPower;
        AG_BattV.Value1 = (float)UPS_BattV;
        UpdateIcon_NotifyIcon();
        UpdateNotifyIconStr?.Invoke("Update Data", null);
        UpdateBatteryState?.Invoke("Update Data");

        if (UPS_Status == "OL" && UPS_Status == "OB")
        {
            OnBattery?.Invoke();
        }

        if (UPS_Status == "OB" && UPS_Status == "OL")
        {
            OnLine?.Invoke();
        }
    }

    private void Menu_Disconnect_Click(object? sender, EventArgs e)
    {
        _log.LogTracing("Disconnect from menu", LogLvl.LOG_DEBUG, this);
        UPSDisconnect();
    }

    private void ReInitDisplayValues()
    {
        _log.LogTracing("Initializing all display values and configurations.", LogLvl.LOG_DEBUG, this);
        UPS_Mfr = "";
        UPS_Model = "";
        UPS_Serial = "";
        UPS_Firmware = "";
        Lbl_VOL.BackColor = Color.White;
        Lbl_VOB.BackColor = Color.White;
        Lbl_VOLoad.BackColor = Color.White;
        Lbl_VBL.BackColor = Color.White;
        Lbl_VRTime.Text = "";
        Lbl_VMfr.Text = UPS_Mfr;
        Lbl_VName.Text = UPS_Model;
        Lbl_VSerial.Text = UPS_Serial;
        Lbl_VFirmware.Text = UPS_Firmware;
        AG_BattCh.Value1 = 0;
        AG_Load.Value1 = 0;
        AG_Load.Value2 = 0;

        AG_InV.Value1 = Settings.Default.CAL_VoltInMin;
        if (AG_InV.MaxValue != Settings.Default.CAL_VoltInMax || AG_InV.MinValue != Settings.Default.CAL_VoltInMin)
        {
            _log.LogTracing("Parameter Dial Input Voltage Need to be Updated", LogLvl.LOG_DEBUG, this);
            AG_InV.MaxValue = Settings.Default.CAL_VoltInMax;
            AG_InV.MinValue = Settings.Default.CAL_VoltInMin;
            AG_InV.ScaleLinesMajorStepValue = (int)((AG_InV.MaxValue - AG_InV.MinValue) / 5);
            _log.LogTracing("Parameter Dial Input Voltage Updated", LogLvl.LOG_DEBUG, this);
        }

        AG_InF.Value1 = Settings.Default.CAL_FreqInMin;
        if (AG_InF.MaxValue != Settings.Default.CAL_FreqInMax || AG_InF.MinValue != Settings.Default.CAL_FreqInMin)
        {
            _log.LogTracing("Parameter Dial Input Frequency Need to be Updated", LogLvl.LOG_DEBUG, this);
            AG_InF.MaxValue = Settings.Default.CAL_FreqInMax;
            AG_InF.MinValue = Settings.Default.CAL_FreqInMin;
            AG_InF.ScaleLinesMajorStepValue = (int)((AG_InF.MaxValue - AG_InF.MinValue) / 5);
            _log.LogTracing("Parameter Dial Input Frequency Updated", LogLvl.LOG_DEBUG, this);
        }

        AG_OutV.Value1 = Settings.Default.CAL_VoltOutMin;
        if (AG_OutV.MaxValue != Settings.Default.CAL_VoltOutMax || AG_OutV.MinValue != Settings.Default.CAL_VoltOutMin)
        {
            _log.LogTracing("Parameter Dial Output Voltage Need to be Updated", LogLvl.LOG_DEBUG, this);
            AG_OutV.MaxValue = Settings.Default.CAL_VoltOutMax;
            AG_OutV.MinValue = Settings.Default.CAL_VoltOutMin;
            AG_OutV.ScaleLinesMajorStepValue = (int)((AG_OutV.MaxValue - AG_OutV.MinValue) / 5);
            _log.LogTracing("Parameter Dial Output Voltage Updated", LogLvl.LOG_DEBUG, this);
        }

        AG_BattV.Value1 = Settings.Default.CAL_BattVMin;
        if (AG_BattV.MaxValue != Settings.Default.CAL_BattVMax || AG_BattV.MinValue != Settings.Default.CAL_BattVMin)
        {
            _log.LogTracing("Parameter Dial Voltage Battery Need to be Updated", LogLvl.LOG_DEBUG, this);
            AG_BattV.MaxValue = Settings.Default.CAL_BattVMax;
            AG_BattV.MinValue = Settings.Default.CAL_BattVMin;
            AG_BattV.ScaleLinesMajorStepValue = (int)((AG_BattV.MaxValue - AG_BattV.MinValue) / 5);
            _log.LogTracing("Parameter Dial Voltage Battery Updated", LogLvl.LOG_DEBUG, this);
        }
    }

    private void Menu_Reconnect_Click(object? sender, EventArgs e)
    {
        _log.LogTracing("Force Reconnect from menu", LogLvl.LOG_DEBUG, this);
        UPSDisconnect();
        UPS_Connect();
    }

    private void OpenPrefsForm(object? sender, EventArgs e)
    {
        _log.LogTracing("Opening Prefs form...", LogLvl.LOG_NOTICE, this);
        PrefGui.Instance.ShowDialog();
    }

    public void ResetUIState()
    {
        _log.LogTracing("Beginning ResetUIState subroutine.", LogLvl.LOG_DEBUG, this);
        var autoReconnect = false;
        if (_upsDevice is { IsConnected: true })
        {
            autoReconnect = true;
            UPSDisconnect();
        }

        ReInitDisplayValues();
        if (autoReconnect)
        {
            UPS_Connect();
        }
    }

    private void UpdateIcon_NotifyIcon()
    {
        if (_actualAppIconIdx == _lastAppIconIdx)
        {
            return;
        }

        _log.LogTracing("Status Icon Changed", LogLvl.LOG_DEBUG, this);
        var tmpWinMode = _winDarkMode
            ? (int)(AppIconIdx.WIN_DARK | AppIconIdx.IDX_OFFSET)
            : (int)AppIconIdx.IDX_OFFSET;
        var tmpAppMode = _appDarkMode
            ? (int)(AppIconIdx.WIN_DARK | AppIconIdx.IDX_OFFSET)
            : (int)AppIconIdx.IDX_OFFSET;
        var tmpGuiIdx = _actualAppIconIdx | tmpAppMode;
        if (!_hasFocus)
        {
            tmpGuiIdx |= (int)AppIconIdx.WIN_DARK;
        }

        var tmpTrayIdx = _actualAppIconIdx | tmpWinMode;
        _log.LogTracing("New Icon Value For Systray : " + tmpTrayIdx, LogLvl.LOG_DEBUG, this);
        _log.LogTracing("New Icon Value For Gui : " + tmpGuiIdx, LogLvl.LOG_DEBUG, this);
        NotifyIcon.Icon = GetIcon(tmpTrayIdx);
        Icon = GetIcon(tmpGuiIdx);
        _lastAppIconIdx = _actualAppIconIdx;
    }

    private void ToastNotifyIcon()
    {
        _log.LogTracing("ToastNotifyIcon running.", LogLvl.LOG_DEBUG, this);
        NotifyIcon.BalloonTipText = NotifyIcon.Text;
        if (_allowToast && !string.IsNullOrEmpty(NotifyIcon.BalloonTipText))
        {
            var toastParts = NotifyIcon.BalloonTipText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            _log.LogTracing("Sending Toast popup with text: " + NotifyIcon.BalloonTipText, LogLvl.LOG_DEBUG, this);
            ToastPopup.SendToast(toastParts);
        }
        else if (NotifyIcon.Visible && !string.IsNullOrEmpty(NotifyIcon.BalloonTipText))
        {
            _log.LogTracing("Sending NotifyIcon ballowtip: " + NotifyIcon.BalloonTipText, LogLvl.LOG_DEBUG, this);
            NotifyIcon.ShowBalloonTip(10000);
        }
    }

    private Icon GetIcon(int iconIdx)
    {
        return iconIdx switch
        {
            1025 => Resources._1025,
            1026 => Resources._1026,
            1028 => Resources._1028,
            1032 => Resources._1032,
            1040 => Resources._1040,
            1057 => Resources._1057,
            1058 => Resources._1058,
            1060 => Resources._1060,
            1064 => Resources._1064,
            1072 => Resources._1072,
            1079 => Resources._1079,
            1080 => Resources._1080,
            1092 => Resources._1092,
            1096 => Resources._1096,
            1104 => Resources._1096,
            1121 => Resources._1121,
            1122 => Resources._1122,
            1124 => Resources._1124,
            1128 => Resources._1128,
            1136 => Resources._1136,
            1152 => Resources._1152,
            1216 => Resources._1216,
            1280 => Resources._1280,
            1344 => Resources._1344,
            _ => Resources._1136
        };
    }

    private void Menu_UPS_Var_Click(object? sender, EventArgs e)
    {
        _log.LogTracing("Open List Var Gui", LogLvl.LOG_DEBUG, this);
        if (_upsDevice != null)
        {
            new ListVarGui(_upsDevice).Show();
        }
    }

    private void AddLogLine(string logLine)
    {
        if (!CB_CurrentLog.Items.Contains(logLine))
        {
            CB_CurrentLog.Items.Insert(0, logLine);
            CB_CurrentLog.SelectedIndex = 0;
        }
        else
        {
            _log.LogTracing("Attempted to add duplicate item to CB_CurrentLog: " + logLine, LogLvl.LOG_ERROR, this);
        }
    }

    private void TrimLogLine(string removedLine)
    {
        _log.LogTracing("Receiving event to trim end of displayed logs list.", LogLvl.LOG_DEBUG, this);
        CB_CurrentLog.Items.Remove(removedLine);
    }

    private void HandleUPSStatusChange(UpsDevice sender, UPS_States newStatuses)
    {
        _log.LogTracing("Handling new UPS status(es)...", LogLvl.LOG_DEBUG, this);
        var val = sender.UPS_Datas.UPS_Value;
        if (newStatuses.Equals(UPS_States.None))
        {
            _log.LogTracing("Received unexpected None status from UPS.", LogLvl.LOG_WARNING, this);
        }
        else if (Settings.Default.PW_RespectFSD && newStatuses.HasFlag(UPS_States.FSD))
        {
            _log.LogTracing("Full Shut Down imposed by the NUT server.", LogLvl.LOG_NOTICE, this,
                StrItem(AppResxStr.STR_LOG_NUT_FSD));
            Shutdown_Event();
        }
        else if (newStatuses.HasFlag(UPS_States.OB))
        {
            _log.LogTracing(sender.Name + " has switched to battery power.", LogLvl.LOG_NOTICE, this);
        }
        else if (newStatuses.HasFlag(UPS_States.OL) && _shutdownStatus)
        {
            _log.LogTracing("UPS returned online during a pre-shutdown event.", LogLvl.LOG_NOTICE, this);
            Stop_Shutdown_Event();
        }
    }

    private void Shutdown_Event()
    {
        _shutdownStatus = true;
        if (Settings.Default.PW_Immediate)
        {
            _log.LogTracing("Immediately stopping due to shutdown event.", LogLvl.LOG_NOTICE, this);
            _upsDevice?.Disconnect();
            Shutdown_Action();
        }
        else
        {
            _log.LogTracing("Open Shutdown Gui", LogLvl.LOG_DEBUG, this);
            ShutdownGui.Instance.Activate();
            ShutdownGui.Instance.Visible = true;
            _hasFocus = false;
        }
    }

    private void Stop_Shutdown_Event()
    {
        _shutdownStatus = false;
        ShutdownGui.Instance.Shutdown_Timer.Stop();
        ShutdownGui.Instance.Shutdown_Timer.Enabled = false;
        ShutdownGui.Instance.Grace_Timer.Stop();
        ShutdownGui.Instance.Grace_Timer.Enabled = false;
        ShutdownGui.Instance.Hide();
        ShutdownGui.Instance.Close();
        _log.LogTracing("Stop condition cancelled.", LogLvl.LOG_NOTICE, this,
            StrItem(AppResxStr.STR_LOG_SHUT_STOP));
    }

    public void Shutdown_Action()
    {
        var stopAction = Settings.Default.PW_StopType;
        _log.LogTracing("Windows going down, WinNUT will disconnect.", LogLvl.LOG_NOTICE, this,
            StrItem(AppResxStr.STR_MAIN_GOTOSLEEP));
        UPSDisconnect();

#if DEBUG
        if (Debugger.IsAttached)
        {
            _log.LogTracing("Aborting stopAction " + stopAction + " due to attached debugger.", LogLvl.LOG_NOTICE,
                this);
            return;
        }
#endif
#if !DEBUG
        switch (stopAction)
        {
            case 0:
                Process.Start("C:\\WINDOWS\\system32\\Shutdown.exe", "-f -s -t 0");
                break;
            case 1:
                Application.SetSuspendState(PowerState.Suspend, false, true);
                break;
            case 2:
                Application.SetSuspendState(PowerState.Hibernate, false, true);
                break;
        }
#endif
    }

    private void ManageOldPrefsToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        _log.LogTracing("Launching UpgradePrefsDialog from ToolStripMenu.", LogLvl.LOG_NOTICE, this);
        UPSDisconnect();
        using (var dlg = new UpgradePrefsDialog())
        {
            dlg.ShowDialog();
        }
    }

    private void Menu_Update_Click(object? sender, EventArgs e)
    {
        _log.LogTracing("Check for update menu item clicked.", LogLvl.LOG_DEBUG, this);
        StartUpdateCheck();
    }

    private void StartUpdateCheck()
    {
        _log.LogTracing("Beginning update check in background...", LogLvl.LOG_NOTICE, this, Resources.LogCheckingForUpdate);
        _ = WinNutGlobals.UpdateController.BeginUpdateCheck(Settings.Default.UP_Branch == 1);
    }

    private void OnCheckForUpdateCompleted(object? sender, UpdateCheckCompletedEventArgs eventArgs)
    {
        _log.LogTracing("UpdateCheckCompleted event firing.", LogLvl.LOG_DEBUG, this);
        Settings.Default.UP_LastCheck = DateTime.Now;
        Settings.Default.Save();

        if (eventArgs.LatestRelease == null)
        {
            _log.LogTracing(
                $"No updates matching the parameter (acceptPreRelease = {Settings.Default.UP_Branch == 1}) were found.",
                LogLvl.LOG_NOTICE, this, StrItem(AppResxStr.STR_LOG_NO_UPDATE));
            if (eventArgs.Error != null)
            {
                _log.LogTracing("CheckForUpdate reported an error:", LogLvl.LOG_ERROR, this);
                _log.LogException(eventArgs.Error, this);
            }

            return;
        }

        var tag = eventArgs.LatestRelease.TagName;
        if (string.IsNullOrEmpty(tag) || tag.Length < 2 ||
            new Version(tag.Substring(1)) <= new Version(WinNutGlobals.ProgramVersion))
        {
            _log.LogTracing("No newer version available.", LogLvl.LOG_NOTICE, this, StrItem(AppResxStr.STR_LOG_NO_UPDATE));
            return;
        }

        if (WinNutGlobals.UpdateController.LatestReleaseAsset == null)
        {
            _log.LogTracing("New update was found on GitHub, but no valid Release Asset was attached.", LogLvl.LOG_ERROR,
                this, StrItem(AppResxStr.STR_LOG_NO_UPDATE));
            return;
        }

        _log.LogTracing($"New update found: {eventArgs.LatestRelease.Name}", LogLvl.LOG_NOTICE, this,
            string.Format(StrItem(AppResxStr.STR_LOG_UPDATE), eventArgs.LatestRelease.Name));

        new UpdateAvailableForm().Show();
        _hasFocus = false;
    }

    private void Menu_Persist_CheckedChanged(object? sender, EventArgs e)
    {
        _log.LogTracing("Menu_Persist checked state changing to " + Menu_Persist.Checked, LogLvl.LOG_DEBUG, this);
        Settings.Default.NUT_AutoReconnect = Menu_Persist.Checked;
    }
}
