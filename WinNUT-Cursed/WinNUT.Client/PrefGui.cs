#nullable enable
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using WinNUT_Client.Properties;
using WinNUT_Client_Common;
using Settings = WinNUT_Client.Properties.Settings;

namespace WinNUT_Client;

public partial class PrefGui : Form
{
    public static PrefGui Instance { get; } = new();

    public event EventHandler? SavedPreferences;

    private bool _isShowed;
    private bool _prefsModified;

    private PrefGui()
    {
        InitializeComponent();
        Btn_Cancel.Click += Btn_Cancel_Click;
        Btn_Apply.Click += Btn_Apply_Click;
        Btn_Ok.Click += Btn_Ok_Click;
        Shown += PrefGui_Shown;
        FormClosing += PrefGui_FormClosing;
        TabControl_Options.Selecting += TabControl_Options_Selecting;
        Load += PrefGui_Load;
        CB_Systray.CheckedChanged += CB_Systray_CheckedChanged;
        Cb_ImmediateStop.CheckedChanged += Cb_ImmediateStop_CheckedChanged;
        Cb_ExtendTime.CheckedChanged += Cb_ExtendTime_CheckedChanged;
        Cb_Update_At_Start.CheckedChanged += Cb_Update_At_Start_CheckedChanged;
        Tb_Port.Validating += Number_Validating;
        Tb_OutV_Min.Validating += Number_Validating;
        Tb_OutV_Max.Validating += Number_Validating;
        Tb_InV_Min.Validating += Number_Validating;
        Tb_InV_Max.Validating += Number_Validating;
        Tb_InF_Min.Validating += Number_Validating;
        Tb_InF_Max.Validating += Number_Validating;
        Tb_GraceTime.Validating += Number_Validating;
        Tb_Delay_Stop.Validating += Number_Validating;
        Tb_BattV_Min.Validating += Number_Validating;
        Tb_BattV_Max.Validating += Number_Validating;
        Tb_BattLimit_Time.Validating += Number_Validating;
        Tb_BattLimit_Load.Validating += Number_Validating;
        Tb_Server_IP.Validating += Correct_Ip_Validating;
        Btn_ViewLog.Click += Btn_ViewLog_Click;
        Btn_DeleteLog.Click += Btn_DeleteLog_Click;
    }

    private void Btn_Cancel_Click(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("Close Pref Gui from Button Cancel", LogLvl.LOG_DEBUG, this);
        Close();
    }

    private static int ParseFreqNomFromComboItem(object? item)
    {
        if (item == null)
        {
            return Settings.Default.CAL_FreqInNom;
        }

        var s = item.ToString() ?? "";
        var digits = new string(s.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var hz) ? hz : Settings.Default.CAL_FreqInNom;
    }

    private static void DisableUpdatePreferences()
    {
        var s = Settings.Default;
        s.UP_CheckAtStart = false;
        s.UP_AutoChkDelay = 0;
        s.UP_Branch = 0;
    }

    private void SaveParams()
    {
        try
        {
            WinNutGlobals.LogFile.LogTracing("Save Parameters.", LogLvl.LOG_DEBUG, this);
            var s = Settings.Default;
            s.NUT_ServerAddress = Tb_Server_IP.Text;
            s.NUT_ServerPort = int.Parse(Tb_Port.Text);
            s.NUT_UPSName = Tb_UPS_Name.Text;
            s.NUT_PollIntervalMsec = (int)(pollingIntervalValue.Value * 1000m);
            s.NUT_Username = new SerializedProtectedString(Tb_Login_Nut.Text);
            s.NUT_Password = new SerializedProtectedString(Tb_Pwd_Nut.Text);
            s.NUT_AutoReconnect = Cb_Reconnect.Checked;
            s.CAL_VoltInMin = int.Parse(Tb_InV_Min.Text);
            s.CAL_VoltInMax = int.Parse(Tb_InV_Max.Text);
            s.CAL_FreqInNom = ParseFreqNomFromComboItem(Cbx_Freq_Input.SelectedItem);
            s.CAL_FreqInMin = int.Parse(Tb_InF_Min.Text);
            s.CAL_FreqInMax = int.Parse(Tb_InF_Max.Text);
            s.CAL_VoltOutMin = int.Parse(Tb_OutV_Min.Text);
            s.CAL_VoltOutMax = int.Parse(Tb_OutV_Max.Text);
            s.CAL_BattVMin = int.Parse(Tb_BattV_Min.Text);
            s.CAL_BattVMax = int.Parse(Tb_BattV_Max.Text);
            s.MinimizeToTray = CB_Systray.Checked;
            s.MinimizeOnStart = CB_Start_Mini.Checked;
            s.CloseToTray = CB_Close_Tray.Checked;
            s.StartWithWindows = CB_Start_W_Win.Checked;
            s.LG_LogToFile = CB_Use_Logfile.Checked;
            s.LG_LogLevel = Cbx_LogLevel.SelectedIndex;
            s.PW_BattChrgFloor = int.Parse(Tb_BattLimit_Load.Text);
            s.PW_RuntimeFloor = int.Parse(Tb_BattLimit_Time.Text);
            s.PW_Immediate = Cb_ImmediateStop.Checked;
            s.PW_RespectFSD = CB_Follow_FSD.Checked;
            s.PW_StopType = Cbx_TypeStop.SelectedIndex;
            s.PW_StopDelaySec = int.Parse(Tb_Delay_Stop.Text);
            s.PW_UserExtendStopTimer = Cb_ExtendTime.Checked;
            s.PW_ExtendDelaySec = int.Parse(Tb_GraceTime.Text);
            DisableUpdatePreferences();

            s.Save();
            var runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            if (CB_Start_W_Win.Checked)
            {
                if (Registry.GetValue(@"HKEY_CURRENT_USER\" + runKey, Application.ProductName, null) == null)
                {
                    using var k = Registry.CurrentUser.OpenSubKey(runKey, true);
                    k?.SetValue(Application.ProductName, Application.ExecutablePath);
                    WinNutGlobals.LogFile.LogTracing("WinNUT Added to Startup.", LogLvl.LOG_DEBUG, this);
                }
            }
            else
            {
                if (Registry.GetValue(@"HKEY_CURRENT_USER\" + runKey, Application.ProductName, null) != null)
                {
                    using var k = Registry.CurrentUser.OpenSubKey(runKey, true);
                    k?.DeleteValue(Application.ProductName, false);
                    WinNutGlobals.LogFile.LogTracing("WinNUT Removed From Startup.", LogLvl.LOG_DEBUG, this);
                }
            }

            SavedPreferences?.Invoke(this, EventArgs.Empty);

            SetLogControlsStatus();
            WinNutGlobals.LogFile.LogTracing("WinNut Preferences Saved.", LogLvl.LOG_NOTICE, this,
                WinNutGlobals.StrLog[(int)AppResxStr.STR_LOG_PREFS]);

            _prefsModified = false;
        }
        catch (Exception ex)
        {
            WinNutGlobals.LogFile.LogTracing("Error when trying to save preferences.", LogLvl.LOG_ERROR, this);
            WinNutGlobals.LogFile.LogException(ex, this);
            MessageBox.Show(ex.ToString(), "Error while saving");
        }
    }

    private void Btn_Apply_Click(object? sender, EventArgs e)
    {
        SaveParams();
        if (!_prefsModified)
        {
            Btn_Apply.Enabled = false;
        }
    }

    private void Btn_Ok_Click(object? sender, EventArgs e)
    {
        if (_prefsModified)
        {
            SaveParams();
        }

        Close();
    }

    private void PrefGui_Shown(object? sender, EventArgs e)
    {
        try
        {
            _isShowed = false;
            var s = Settings.Default;
            Tb_Server_IP.Text = s.NUT_ServerAddress;
            Tb_Port.Text = s.NUT_ServerPort.ToString();
            Tb_UPS_Name.Text = s.NUT_UPSName;
            pollingIntervalValue.Value = Math.Max(pollingIntervalValue.Minimum, s.NUT_PollIntervalMsec / 1000m);
            Tb_Login_Nut.Text = s.NUT_Username?.ToString() ?? "";
            Tb_Pwd_Nut.Text = s.NUT_Password?.ToString() ?? "";
            Cb_Reconnect.Checked = s.NUT_AutoReconnect;
            Tb_InV_Min.Text = s.CAL_VoltInMin.ToString();
            Tb_InV_Max.Text = s.CAL_VoltInMax.ToString();
            var freqIdx = Cbx_Freq_Input.FindStringExact(s.CAL_FreqInNom.ToString());
            Cbx_Freq_Input.SelectedIndex = freqIdx >= 0 ? freqIdx : 0;
            Tb_InF_Min.Text = s.CAL_FreqInMin.ToString();
            Tb_InF_Max.Text = s.CAL_FreqInMax.ToString();
            Tb_OutV_Min.Text = s.CAL_VoltOutMin.ToString();
            Tb_OutV_Max.Text = s.CAL_VoltOutMax.ToString();
            Tb_BattV_Min.Text = s.CAL_BattVMin.ToString();
            Tb_BattV_Max.Text = s.CAL_BattVMax.ToString();
            CB_Systray.Checked = s.MinimizeToTray;
            CB_Start_Mini.Checked = s.MinimizeOnStart;
            CB_Close_Tray.Checked = s.CloseToTray;
            CB_Start_W_Win.Checked = s.StartWithWindows;
            CB_Use_Logfile.Checked = s.LG_LogToFile;
            Cbx_LogLevel.SelectedIndex = s.LG_LogLevel;
            Tb_BattLimit_Load.Text = s.PW_BattChrgFloor.ToString();
            Tb_BattLimit_Time.Text = s.PW_RuntimeFloor.ToString();
            Cb_ImmediateStop.Checked = s.PW_Immediate;
            CB_Follow_FSD.Checked = s.PW_RespectFSD;
            Cbx_TypeStop.SelectedIndex = s.PW_StopType;
            Tb_Delay_Stop.Text = s.PW_StopDelaySec.ToString();
            Cb_ExtendTime.Checked = s.PW_UserExtendStopTimer;
            Tb_GraceTime.Text = s.PW_ExtendDelaySec.ToString();
            DisableUpdatePreferences();
            Cb_Update_At_Start.Checked = false;
            Cbx_Delay_Verif.SelectedIndex = 0;
            Cbx_Branch_Update.SelectedIndex = 0;
            if (CB_Systray.Checked)
            {
                CB_Start_Mini.Enabled = true;
                CB_Close_Tray.Enabled = true;
            }
            else
            {
                CB_Start_Mini.Enabled = false;
                CB_Close_Tray.Enabled = false;
            }

            if (Cb_ImmediateStop.Checked)
            {
                Tb_Delay_Stop.Enabled = false;
            }
            else
            {
                Tb_Delay_Stop.Enabled = true;
            }

            if (Cb_ExtendTime.Checked)
            {
                Tb_GraceTime.Enabled = true;
            }
            else
            {
                Tb_GraceTime.Enabled = false;
            }

            Cb_Update_At_Start.Enabled = false;
            Cbx_Delay_Verif.Enabled = false;
            Cbx_Branch_Update.Enabled = false;
            Lbl_Delay_Verif.Enabled = false;
            Lbl_Branch_Update.Enabled = false;

            foreach (TabPage tabCtrl in TabControl_Options.TabPages)
            {
                foreach (var tBox in tabCtrl.Controls.OfType<TextBox>())
                {
                    tBox.TextChanged += Event_Ctrl_Value_Changed;
                }

                foreach (var chkBox in tabCtrl.Controls.OfType<CheckBox>())
                {
                    chkBox.CheckedChanged += Event_Ctrl_Value_Changed;
                }

                foreach (var cmbBox in tabCtrl.Controls.OfType<ComboBox>())
                {
                    cmbBox.SelectedIndexChanged += Event_Ctrl_Value_Changed;
                }
            }

            pollingIntervalValue.ValueChanged += Event_Ctrl_Value_Changed;

            SetLogControlsStatus();
            _isShowed = true;
            WinNutGlobals.LogFile.LogTracing("Pref Gui Opened.", LogLvl.LOG_DEBUG, this);
        }
        catch (Exception ex)
        {
            _isShowed = false;
            Close();
            WinNutGlobals.LogFile.LogTracing("Error on Opening Pref_Gui:" + Environment.NewLine + ex, LogLvl.LOG_ERROR,
                this);
        }
    }

    private void CB_Systray_CheckedChanged(object? sender, EventArgs e)
    {
        if (CB_Systray.Checked)
        {
            CB_Start_Mini.Enabled = true;
            CB_Close_Tray.Enabled = true;
        }
        else
        {
            CB_Start_Mini.Enabled = false;
            CB_Close_Tray.Enabled = false;
        }
    }

    private void Cb_ImmediateStop_CheckedChanged(object? sender, EventArgs e)
    {
        if (Cb_ImmediateStop.Checked)
        {
            Tb_Delay_Stop.Enabled = false;
        }
        else
        {
            Tb_Delay_Stop.Enabled = true;
            Number_Validating(Tb_Delay_Stop, new CancelEventArgs());
        }
    }

    private void Cb_ExtendTime_CheckedChanged(object? sender, EventArgs e)
    {
        if (Cb_ExtendTime.Checked)
        {
            Tb_GraceTime.Enabled = true;
            Number_Validating(Tb_GraceTime, new CancelEventArgs());
        }
        else
        {
            Tb_GraceTime.Enabled = false;
        }
    }

    private void Cb_Update_At_Start_CheckedChanged(object? sender, EventArgs e)
    {
        Cb_Update_At_Start.Checked = false;
        Cbx_Delay_Verif.Enabled = false;
        Cbx_Branch_Update.Enabled = false;
    }

    private void Number_Validating(object? sender, CancelEventArgs e)
    {
        if (!_isShowed || sender is not TextBox box)
        {
            return;
        }

        WinNutGlobals.LogFile.LogTracing(string.Format("Check that the value of {0} for {1} is correct.", box.Text,
            box.Name), LogLvl.LOG_DEBUG, this);
        int minValue;
        int maxValue;
        switch (box.Name)
        {
            case "Tb_Port":
                minValue = 1;
                maxValue = 65536;
                break;
            case "Tb_OutV_Min":
            case "Tb_OutV_Max":
            case "Tb_InV_Min":
            case "Tb_InV_Max":
            case "Tb_BattV_Min":
            case "Tb_BattV_Max":
                minValue = 0;
                maxValue = 999;
                break;
            case "Tb_InF_Min":
            case "Tb_InF_Max":
            case "Tb_BattLimit_Load":
                minValue = 0;
                maxValue = 100;
                break;
            case "Tb_BattLimit_Time":
                minValue = 0;
                maxValue = 3600;
                break;
            case "Tb_GraceTime":
            case "Tb_Delay_Stop":
                minValue = 1;
                maxValue = 3600;
                break;
            default:
                return;
        }

        if (box.Text == "")
        {
            box.Text = minValue.ToString();
        }

        if (int.TryParse(box.Text, out var result))
        {
            if (result >= minValue && result <= maxValue)
            {
                WinNutGlobals.LogFile.LogTracing(string.Format("Value of {0} for {1} is valid.", result, box.Name),
                    LogLvl.LOG_DEBUG, this);
                box.BackColor = Color.White;
            }
            else
            {
                WinNutGlobals.LogFile.LogTracing(string.Format("Value of {0} for {1} is invalid.", result, box.Name),
                    LogLvl.LOG_ERROR, this);
                e.Cancel = true;
                box.BackColor = Color.Red;
            }
        }
        else
        {
            e.Cancel = true;
            box.BackColor = Color.Red;
        }
    }

    private void Correct_Ip_Validating(object? sender, CancelEventArgs e)
    {
        if (sender is not TextBox box)
        {
            return;
        }

        WinNutGlobals.LogFile.LogTracing("Check that the Nut Host address is valid.", LogLvl.LOG_DEBUG, this);
        var isCorrect = false;
        const string ipv4 =
            @"^(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[0-1]?[0-9][0-9]?)$";
        if (Regex.IsMatch(box.Text, ipv4))
        {
            isCorrect = true;
            WinNutGlobals.LogFile.LogTracing("The Nut Host address is a valid IPV4 address.", LogLvl.LOG_WARNING, this);
        }

        const string ipv6 =
            @"^\s*((([0-9A-Fa-f]{1,4}:){7}([0-9A-Fa-f]{1,4}|:))|(([0-9A-Fa-f]{1,4}:){6}(:[0-9A-Fa-f]{1,4}|((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){5}(((:[0-9A-Fa-f]{1,4}){1,2})|:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3})|:))|(([0-9A-Fa-f]{1,4}:){4}(((:[0-9A-Fa-f]{1,4}){1,3})|((:[0-9A-Fa-f]{1,4})?:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){3}(((:[0-9A-Fa-f]{1,4}){1,4})|((:[0-9A-Fa-f]{1,4}){0,2}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){2}(((:[0-9A-Fa-f]{1,4}){1,5})|((:[0-9A-Fa-f]{1,4}){0,3}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(([0-9A-Fa-f]{1,4}:){1}(((:[0-9A-Fa-f]{1,4}){1,6})|((:[0-9A-Fa-f]{1,4}){0,4}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:))|(:(((:[0-9A-Fa-f]{1,4}){1,7})|((:[0-9A-Fa-f]{1,4}){0,5}:((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)(\.(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)){3}))|:)))(%.+)?\s*$";
        if (Regex.IsMatch(box.Text, ipv6) && !isCorrect)
        {
            isCorrect = true;
            WinNutGlobals.LogFile.LogTracing("The Nut Host address is a valid IPV6 address.", LogLvl.LOG_WARNING, this);
        }

        const string fqdn = @"^(?:(?!\d+\.|-)[a-zA-Z0-9_\-]{1,63}(?<!-)\.?)+(?:[a-zA-Z]{2,})$";
        if (Regex.IsMatch(box.Text, fqdn) && !isCorrect)
        {
            isCorrect = true;
            WinNutGlobals.LogFile.LogTracing("The Nut Host address is a valid FQDN address.", LogLvl.LOG_WARNING, this);
        }

        if (isCorrect)
        {
            box.BackColor = Color.White;
        }
        else
        {
            WinNutGlobals.LogFile.LogTracing("The Nut Host address is a invalid", LogLvl.LOG_ERROR, this);
            e.Cancel = true;
            box.BackColor = Color.Red;
        }
    }

    private void PrefGui_FormClosing(object? sender, FormClosingEventArgs e) => e.Cancel = false;

    private void TabControl_Options_Selecting(object? sender, TabControlCancelEventArgs e)
    {
        if (TabControl_Options.SelectedTab == Tab_Miscellanous)
        {
            SetLogControlsStatus();
        }
    }

    private void PrefGui_Load(object? sender, EventArgs e)
    {
        foreach (Form f in Application.OpenForms)
        {
            if (f is WinNUT main)
            {
                Icon = main.Icon;
                break;
            }
        }

        WinNutGlobals.LogFile.LogTracing("Load Pref Gui", LogLvl.LOG_DEBUG, this);
    }

    private void Event_Ctrl_Value_Changed(object? sender, EventArgs e)
    {
        if (_isShowed)
        {
            _prefsModified = true;
            Btn_Apply.Enabled = true;
        }
    }

    private void Btn_ViewLog_Click(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("User clicked ViewLog button.", LogLvl.LOG_DEBUG, this);
        try
        {
            Process.Start(new ProcessStartInfo(WinNutGlobals.LogFile.LogFilePath) { UseShellExecute = true });
            WinNutGlobals.LogFile.LogTracing("Opened UI window to log location.", LogLvl.LOG_NOTICE, this);
        }
        catch (Exception ex)
        {
            WinNutGlobals.LogFile.LogException(ex, this);
            SetLogControlsStatus();
        }
    }

    private void Btn_DeleteLog_Click(object? sender, EventArgs e)
    {
        WinNutGlobals.LogFile.LogTracing("User clicked DeleteLog button.", LogLvl.LOG_DEBUG, this);
        try
        {
            WinNutGlobals.LogFile.DeleteLogFile();
            _prefsModified = true;
        }
        catch (Exception ex)
        {
            WinNutGlobals.LogFile.LogException(ex, this);
        }

        SetLogControlsStatus();
    }

    private void SetLogControlsStatus()
    {
        if (WinNutGlobals.LogFile.IsWritingToFile)
        {
            Btn_ViewLog.Enabled = true;
            Btn_DeleteLog.Enabled = true;
        }
        else
        {
            Btn_ViewLog.Enabled = false;
            Btn_DeleteLog.Enabled = false;
        }
    }
}
