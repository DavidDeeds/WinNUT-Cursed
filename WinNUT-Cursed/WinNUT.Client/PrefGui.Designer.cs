#nullable enable
using WinNUT_Client.Properties;

namespace WinNUT_Client;

partial class PrefGui
{
    private System.ComponentModel.IContainer? components;

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
        }
        finally
        {
            base.Dispose(disposing);
        }
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(PrefGui));
        TabControl_Options = new TabControl();
        Tab_Connexion = new TabPage();
        pollingIntervalUnitLabel = new Label();
        pollingIntervalValue = new NumericUpDown();
        Tb_Pwd_Nut = new TextBox();
        Tb_Login_Nut = new TextBox();
        Label3 = new Label();
        Lbl_Pwd_Nut = new Label();
        Lbl_Login_Nut = new Label();
        Tb_UPS_Name = new TextBox();
        Tb_Port = new TextBox();
        Tb_Server_IP = new TextBox();
        Cb_Reconnect = new CheckBox();
        Lbl_Delay_Com = new Label();
        Lbl_Name_UPS = new Label();
        Lbl_Port = new Label();
        Lbl_Server_IP = new Label();
        Tab_Calibrage = new TabPage();
        Cbx_Freq_Input = new ComboBox();
        Tb_BattV_Max = new TextBox();
        Tb_OutV_Max = new TextBox();
        Tb_InF_Max = new TextBox();
        Tb_BattV_Min = new TextBox();
        Tb_OutV_Min = new TextBox();
        Tb_InF_Min = new TextBox();
        Tb_InV_Max = new TextBox();
        Tb_InV_Min = new TextBox();
        Lbl_Maxi = new Label();
        Lbl_Mini = new Label();
        Lbl_BattV = new Label();
        Lbl_LoadUPS = new Label();
        Lbl_OutputV = new Label();
        Lbl_InputF = new Label();
        Lbl_PowerF = new Label();
        Lbl_InputV = new Label();
        Tab_Miscellanous = new TabPage();
        Cbx_LogLevel = new ComboBox();
        Btn_DeleteLog = new Button();
        Btn_ViewLog = new Button();
        Lbl_LevelLog = new Label();
        CB_Use_Logfile = new CheckBox();
        CB_Start_W_Win = new CheckBox();
        CB_Close_Tray = new CheckBox();
        CB_Start_Mini = new CheckBox();
        CB_Systray = new CheckBox();
        Tab_Shutdown = new TabPage();
        CB_Follow_FSD = new CheckBox();
        Lbl_Percent = new Label();
        Tb_GraceTime = new TextBox();
        Cb_ExtendTime = new CheckBox();
        Tb_Delay_Stop = new TextBox();
        Cbx_TypeStop = new ComboBox();
        Cb_ImmediateStop = new CheckBox();
        Tb_BattLimit_Time = new TextBox();
        Tb_BattLimit_Load = new TextBox();
        Lbl_GraceTime = new Label();
        Lbl_Delay_Stop = new Label();
        Lbl_StopType = new Label();
        Lbl_BattLimit_Time = new Label();
        Lbl_BattLimit_Load = new Label();
        Tab_Update = new TabPage();
        Cbx_Branch_Update = new ComboBox();
        Cbx_Delay_Verif = new ComboBox();
        Lbl_Branch_Update = new Label();
        Lbl_Delay_Verif = new Label();
        Cb_Update_At_Start = new CheckBox();
        Btn_Ok = new Button();
        Btn_Apply = new Button();
        Btn_Cancel = new Button();
        Pref_TlTip = new ToolTip(components);
        TabControl_Options.SuspendLayout();
        Tab_Connexion.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pollingIntervalValue).BeginInit();
        Tab_Calibrage.SuspendLayout();
        Tab_Miscellanous.SuspendLayout();
        Tab_Shutdown.SuspendLayout();
        Tab_Update.SuspendLayout();
        SuspendLayout();
        //
        // TabControl_Options
        //
        TabControl_Options.Controls.Add(Tab_Connexion);
        TabControl_Options.Controls.Add(Tab_Calibrage);
        TabControl_Options.Controls.Add(Tab_Miscellanous);
        TabControl_Options.Controls.Add(Tab_Shutdown);
        TabControl_Options.Controls.Add(Tab_Update);
        resources.ApplyResources(TabControl_Options, "TabControl_Options");
        TabControl_Options.Name = "TabControl_Options";
        TabControl_Options.SelectedIndex = 0;
        //
        // Tab_Connexion
        //
        Tab_Connexion.Controls.Add(pollingIntervalUnitLabel);
        Tab_Connexion.Controls.Add(pollingIntervalValue);
        Tab_Connexion.Controls.Add(Tb_Pwd_Nut);
        Tab_Connexion.Controls.Add(Tb_Login_Nut);
        Tab_Connexion.Controls.Add(Label3);
        Tab_Connexion.Controls.Add(Lbl_Pwd_Nut);
        Tab_Connexion.Controls.Add(Lbl_Login_Nut);
        Tab_Connexion.Controls.Add(Tb_UPS_Name);
        Tab_Connexion.Controls.Add(Tb_Port);
        Tab_Connexion.Controls.Add(Tb_Server_IP);
        Tab_Connexion.Controls.Add(Cb_Reconnect);
        Tab_Connexion.Controls.Add(Lbl_Delay_Com);
        Tab_Connexion.Controls.Add(Lbl_Name_UPS);
        Tab_Connexion.Controls.Add(Lbl_Port);
        Tab_Connexion.Controls.Add(Lbl_Server_IP);
        resources.ApplyResources(Tab_Connexion, "Tab_Connexion");
        Tab_Connexion.Name = "Tab_Connexion";
        Tab_Connexion.UseVisualStyleBackColor = true;
        //
        // pollingIntervalUnitLabel
        //
        resources.ApplyResources(pollingIntervalUnitLabel, "pollingIntervalUnitLabel");
        pollingIntervalUnitLabel.Name = "pollingIntervalUnitLabel";
        //
        // pollingIntervalValue
        //
        pollingIntervalValue.DecimalPlaces = 1;
        pollingIntervalValue.Increment = new decimal(new[] { 1, 0, 0, 65536 });
        resources.ApplyResources(pollingIntervalValue, "pollingIntervalValue");
        pollingIntervalValue.Minimum = new decimal(new[] { 1, 0, 0, 65536 });
        pollingIntervalValue.Name = "pollingIntervalValue";
        Pref_TlTip.SetToolTip(pollingIntervalValue, resources.GetString("pollingIntervalValue.ToolTip"));
        pollingIntervalValue.Value = new decimal(new[] { 1, 0, 0, 65536 });
        //
        // Tb_Pwd_Nut
        //
        resources.ApplyResources(Tb_Pwd_Nut, "Tb_Pwd_Nut");
        Tb_Pwd_Nut.Name = "Tb_Pwd_Nut";
        Pref_TlTip.SetToolTip(Tb_Pwd_Nut, resources.GetString("Tb_Pwd_Nut.ToolTip"));
        Tb_Pwd_Nut.UseSystemPasswordChar = true;
        //
        // Tb_Login_Nut
        //
        resources.ApplyResources(Tb_Login_Nut, "Tb_Login_Nut");
        Tb_Login_Nut.Name = "Tb_Login_Nut";
        Pref_TlTip.SetToolTip(Tb_Login_Nut, resources.GetString("Tb_Login_Nut.ToolTip"));
        //
        // Label3
        //
        resources.ApplyResources(Label3, "Label3");
        Label3.Name = "Label3";
        //
        // Lbl_Pwd_Nut
        //
        resources.ApplyResources(Lbl_Pwd_Nut, "Lbl_Pwd_Nut");
        Lbl_Pwd_Nut.Name = "Lbl_Pwd_Nut";
        //
        // Lbl_Login_Nut
        //
        resources.ApplyResources(Lbl_Login_Nut, "Lbl_Login_Nut");
        Lbl_Login_Nut.Name = "Lbl_Login_Nut";
        //
        // Tb_UPS_Name
        //
        resources.ApplyResources(Tb_UPS_Name, "Tb_UPS_Name");
        Tb_UPS_Name.Name = "Tb_UPS_Name";
        Pref_TlTip.SetToolTip(Tb_UPS_Name, resources.GetString("Tb_UPS_Name.ToolTip"));
        //
        // Tb_Port
        //
        resources.ApplyResources(Tb_Port, "Tb_Port");
        Tb_Port.Name = "Tb_Port";
        Pref_TlTip.SetToolTip(Tb_Port, resources.GetString("Tb_Port.ToolTip"));
        //
        // Tb_Server_IP
        //
        resources.ApplyResources(Tb_Server_IP, "Tb_Server_IP");
        Tb_Server_IP.Name = "Tb_Server_IP";
        Pref_TlTip.SetToolTip(Tb_Server_IP, resources.GetString("Tb_Server_IP.ToolTip"));
        //
        // Cb_Reconnect
        //
        resources.ApplyResources(Cb_Reconnect, "Cb_Reconnect");
        Cb_Reconnect.Name = "Cb_Reconnect";
        Pref_TlTip.SetToolTip(Cb_Reconnect, resources.GetString("Cb_Reconnect.ToolTip"));
        Cb_Reconnect.UseVisualStyleBackColor = true;
        //
        // Lbl_Delay_Com
        //
        resources.ApplyResources(Lbl_Delay_Com, "Lbl_Delay_Com");
        Lbl_Delay_Com.Name = "Lbl_Delay_Com";
        //
        // Lbl_Name_UPS
        //
        resources.ApplyResources(Lbl_Name_UPS, "Lbl_Name_UPS");
        Lbl_Name_UPS.Name = "Lbl_Name_UPS";
        //
        // Lbl_Port
        //
        resources.ApplyResources(Lbl_Port, "Lbl_Port");
        Lbl_Port.Name = "Lbl_Port";
        //
        // Lbl_Server_IP
        //
        resources.ApplyResources(Lbl_Server_IP, "Lbl_Server_IP");
        Lbl_Server_IP.Name = "Lbl_Server_IP";
        //
        // Tab_Calibrage
        //
        Tab_Calibrage.Controls.Add(Cbx_Freq_Input);
        Tab_Calibrage.Controls.Add(Tb_BattV_Max);
        Tab_Calibrage.Controls.Add(Tb_OutV_Max);
        Tab_Calibrage.Controls.Add(Tb_InF_Max);
        Tab_Calibrage.Controls.Add(Tb_BattV_Min);
        Tab_Calibrage.Controls.Add(Tb_OutV_Min);
        Tab_Calibrage.Controls.Add(Tb_InF_Min);
        Tab_Calibrage.Controls.Add(Tb_InV_Max);
        Tab_Calibrage.Controls.Add(Tb_InV_Min);
        Tab_Calibrage.Controls.Add(Lbl_Maxi);
        Tab_Calibrage.Controls.Add(Lbl_Mini);
        Tab_Calibrage.Controls.Add(Lbl_BattV);
        Tab_Calibrage.Controls.Add(Lbl_LoadUPS);
        Tab_Calibrage.Controls.Add(Lbl_OutputV);
        Tab_Calibrage.Controls.Add(Lbl_InputF);
        Tab_Calibrage.Controls.Add(Lbl_PowerF);
        Tab_Calibrage.Controls.Add(Lbl_InputV);
        resources.ApplyResources(Tab_Calibrage, "Tab_Calibrage");
        Tab_Calibrage.Name = "Tab_Calibrage";
        Tab_Calibrage.UseVisualStyleBackColor = true;
        //
        // Cbx_Freq_Input
        //
        Cbx_Freq_Input.FormattingEnabled = true;
        Cbx_Freq_Input.Items.AddRange(new object[]
        {
            resources.GetString("Cbx_Freq_Input.Items"),
            resources.GetString("Cbx_Freq_Input.Items1")
        });
        resources.ApplyResources(Cbx_Freq_Input, "Cbx_Freq_Input");
        Cbx_Freq_Input.Name = "Cbx_Freq_Input";
        Pref_TlTip.SetToolTip(Cbx_Freq_Input, resources.GetString("Cbx_Freq_Input.ToolTip"));
        //
        // Tb_BattV_Max
        //
        resources.ApplyResources(Tb_BattV_Max, "Tb_BattV_Max");
        Tb_BattV_Max.Name = "Tb_BattV_Max";
        Pref_TlTip.SetToolTip(Tb_BattV_Max, resources.GetString("Tb_BattV_Max.ToolTip"));
        //
        // Tb_OutV_Max
        //
        resources.ApplyResources(Tb_OutV_Max, "Tb_OutV_Max");
        Tb_OutV_Max.Name = "Tb_OutV_Max";
        Pref_TlTip.SetToolTip(Tb_OutV_Max, resources.GetString("Tb_OutV_Max.ToolTip"));
        //
        // Tb_InF_Max
        //
        resources.ApplyResources(Tb_InF_Max, "Tb_InF_Max");
        Tb_InF_Max.Name = "Tb_InF_Max";
        Pref_TlTip.SetToolTip(Tb_InF_Max, resources.GetString("Tb_InF_Max.ToolTip"));
        //
        // Tb_BattV_Min
        //
        resources.ApplyResources(Tb_BattV_Min, "Tb_BattV_Min");
        Tb_BattV_Min.Name = "Tb_BattV_Min";
        Pref_TlTip.SetToolTip(Tb_BattV_Min, resources.GetString("Tb_BattV_Min.ToolTip"));
        //
        // Tb_OutV_Min
        //
        resources.ApplyResources(Tb_OutV_Min, "Tb_OutV_Min");
        Tb_OutV_Min.Name = "Tb_OutV_Min";
        Pref_TlTip.SetToolTip(Tb_OutV_Min, resources.GetString("Tb_OutV_Min.ToolTip"));
        //
        // Tb_InF_Min
        //
        resources.ApplyResources(Tb_InF_Min, "Tb_InF_Min");
        Tb_InF_Min.Name = "Tb_InF_Min";
        Pref_TlTip.SetToolTip(Tb_InF_Min, resources.GetString("Tb_InF_Min.ToolTip"));
        //
        // Tb_InV_Max
        //
        resources.ApplyResources(Tb_InV_Max, "Tb_InV_Max");
        Tb_InV_Max.Name = "Tb_InV_Max";
        Pref_TlTip.SetToolTip(Tb_InV_Max, resources.GetString("Tb_InV_Max.ToolTip"));
        //
        // Tb_InV_Min
        //
        resources.ApplyResources(Tb_InV_Min, "Tb_InV_Min");
        Tb_InV_Min.Name = "Tb_InV_Min";
        Pref_TlTip.SetToolTip(Tb_InV_Min, resources.GetString("Tb_InV_Min.ToolTip"));
        //
        // Lbl_Maxi
        //
        resources.ApplyResources(Lbl_Maxi, "Lbl_Maxi");
        Lbl_Maxi.Name = "Lbl_Maxi";
        //
        // Lbl_Mini
        //
        resources.ApplyResources(Lbl_Mini, "Lbl_Mini");
        Lbl_Mini.Name = "Lbl_Mini";
        //
        // Lbl_BattV
        //
        resources.ApplyResources(Lbl_BattV, "Lbl_BattV");
        Lbl_BattV.Name = "Lbl_BattV";
        //
        // Lbl_LoadUPS
        //
        resources.ApplyResources(Lbl_LoadUPS, "Lbl_LoadUPS");
        Lbl_LoadUPS.ForeColor = SystemColors.GrayText;
        Lbl_LoadUPS.Name = "Lbl_LoadUPS";
        //
        // Lbl_OutputV
        //
        resources.ApplyResources(Lbl_OutputV, "Lbl_OutputV");
        Lbl_OutputV.Name = "Lbl_OutputV";
        //
        // Lbl_InputF
        //
        resources.ApplyResources(Lbl_InputF, "Lbl_InputF");
        Lbl_InputF.Name = "Lbl_InputF";
        //
        // Lbl_PowerF
        //
        resources.ApplyResources(Lbl_PowerF, "Lbl_PowerF");
        Lbl_PowerF.Name = "Lbl_PowerF";
        //
        // Lbl_InputV
        //
        resources.ApplyResources(Lbl_InputV, "Lbl_InputV");
        Lbl_InputV.Name = "Lbl_InputV";
        //
        // Tab_Miscellanous
        //
        Tab_Miscellanous.Controls.Add(Cbx_LogLevel);
        Tab_Miscellanous.Controls.Add(Btn_DeleteLog);
        Tab_Miscellanous.Controls.Add(Btn_ViewLog);
        Tab_Miscellanous.Controls.Add(Lbl_LevelLog);
        Tab_Miscellanous.Controls.Add(CB_Use_Logfile);
        Tab_Miscellanous.Controls.Add(CB_Start_W_Win);
        Tab_Miscellanous.Controls.Add(CB_Close_Tray);
        Tab_Miscellanous.Controls.Add(CB_Start_Mini);
        Tab_Miscellanous.Controls.Add(CB_Systray);
        resources.ApplyResources(Tab_Miscellanous, "Tab_Miscellanous");
        Tab_Miscellanous.Name = "Tab_Miscellanous";
        Tab_Miscellanous.UseVisualStyleBackColor = true;
        //
        // Cbx_LogLevel
        //
        Cbx_LogLevel.FormattingEnabled = true;
        Cbx_LogLevel.Items.AddRange(new object[]
        {
            resources.GetString("Cbx_LogLevel.Items"),
            resources.GetString("Cbx_LogLevel.Items1"),
            resources.GetString("Cbx_LogLevel.Items2"),
            resources.GetString("Cbx_LogLevel.Items3")
        });
        resources.ApplyResources(Cbx_LogLevel, "Cbx_LogLevel");
        Cbx_LogLevel.Name = "Cbx_LogLevel";
        Pref_TlTip.SetToolTip(Cbx_LogLevel, resources.GetString("Cbx_LogLevel.ToolTip"));
        //
        // Btn_DeleteLog
        //
        Btn_DeleteLog.Image = Resources.Delete_LogFile_24x24;
        resources.ApplyResources(Btn_DeleteLog, "Btn_DeleteLog");
        Btn_DeleteLog.Name = "Btn_DeleteLog";
        Pref_TlTip.SetToolTip(Btn_DeleteLog, resources.GetString("Btn_DeleteLog.ToolTip"));
        Btn_DeleteLog.UseVisualStyleBackColor = true;
        //
        // Btn_ViewLog
        //
        Btn_ViewLog.Image = Resources.ViewLogFile_24x24;
        resources.ApplyResources(Btn_ViewLog, "Btn_ViewLog");
        Btn_ViewLog.Name = "Btn_ViewLog";
        Pref_TlTip.SetToolTip(Btn_ViewLog, resources.GetString("Btn_ViewLog.ToolTip"));
        Btn_ViewLog.UseVisualStyleBackColor = true;
        //
        // Lbl_LevelLog
        //
        resources.ApplyResources(Lbl_LevelLog, "Lbl_LevelLog");
        Lbl_LevelLog.Name = "Lbl_LevelLog";
        Pref_TlTip.SetToolTip(Lbl_LevelLog, resources.GetString("Lbl_LevelLog.ToolTip"));
        //
        // CB_Use_Logfile
        //
        resources.ApplyResources(CB_Use_Logfile, "CB_Use_Logfile");
        CB_Use_Logfile.Name = "CB_Use_Logfile";
        Pref_TlTip.SetToolTip(CB_Use_Logfile, resources.GetString("CB_Use_Logfile.ToolTip"));
        CB_Use_Logfile.UseVisualStyleBackColor = true;
        //
        // CB_Start_W_Win
        //
        resources.ApplyResources(CB_Start_W_Win, "CB_Start_W_Win");
        CB_Start_W_Win.Name = "CB_Start_W_Win";
        Pref_TlTip.SetToolTip(CB_Start_W_Win, resources.GetString("CB_Start_W_Win.ToolTip"));
        CB_Start_W_Win.UseVisualStyleBackColor = true;
        //
        // CB_Close_Tray
        //
        resources.ApplyResources(CB_Close_Tray, "CB_Close_Tray");
        CB_Close_Tray.Name = "CB_Close_Tray";
        Pref_TlTip.SetToolTip(CB_Close_Tray, resources.GetString("CB_Close_Tray.ToolTip"));
        CB_Close_Tray.UseVisualStyleBackColor = true;
        //
        // CB_Start_Mini
        //
        resources.ApplyResources(CB_Start_Mini, "CB_Start_Mini");
        CB_Start_Mini.Name = "CB_Start_Mini";
        Pref_TlTip.SetToolTip(CB_Start_Mini, resources.GetString("CB_Start_Mini.ToolTip"));
        CB_Start_Mini.UseVisualStyleBackColor = true;
        //
        // CB_Systray
        //
        resources.ApplyResources(CB_Systray, "CB_Systray");
        CB_Systray.Name = "CB_Systray";
        Pref_TlTip.SetToolTip(CB_Systray, resources.GetString("CB_Systray.ToolTip"));
        CB_Systray.UseVisualStyleBackColor = true;
        //
        // Tab_Shutdown
        //
        Tab_Shutdown.Controls.Add(CB_Follow_FSD);
        Tab_Shutdown.Controls.Add(Lbl_Percent);
        Tab_Shutdown.Controls.Add(Tb_GraceTime);
        Tab_Shutdown.Controls.Add(Cb_ExtendTime);
        Tab_Shutdown.Controls.Add(Tb_Delay_Stop);
        Tab_Shutdown.Controls.Add(Cbx_TypeStop);
        Tab_Shutdown.Controls.Add(Cb_ImmediateStop);
        Tab_Shutdown.Controls.Add(Tb_BattLimit_Time);
        Tab_Shutdown.Controls.Add(Tb_BattLimit_Load);
        Tab_Shutdown.Controls.Add(Lbl_GraceTime);
        Tab_Shutdown.Controls.Add(Lbl_Delay_Stop);
        Tab_Shutdown.Controls.Add(Lbl_StopType);
        Tab_Shutdown.Controls.Add(Lbl_BattLimit_Time);
        Tab_Shutdown.Controls.Add(Lbl_BattLimit_Load);
        resources.ApplyResources(Tab_Shutdown, "Tab_Shutdown");
        Tab_Shutdown.Name = "Tab_Shutdown";
        Tab_Shutdown.UseVisualStyleBackColor = true;
        //
        // CB_Follow_FSD
        //
        resources.ApplyResources(CB_Follow_FSD, "CB_Follow_FSD");
        CB_Follow_FSD.Name = "CB_Follow_FSD";
        Pref_TlTip.SetToolTip(CB_Follow_FSD, resources.GetString("CB_Follow_FSD.ToolTip"));
        CB_Follow_FSD.UseVisualStyleBackColor = true;
        //
        // Lbl_Percent
        //
        resources.ApplyResources(Lbl_Percent, "Lbl_Percent");
        Lbl_Percent.Name = "Lbl_Percent";
        //
        // Tb_GraceTime
        //
        resources.ApplyResources(Tb_GraceTime, "Tb_GraceTime");
        Tb_GraceTime.Name = "Tb_GraceTime";
        Pref_TlTip.SetToolTip(Tb_GraceTime, resources.GetString("Tb_GraceTime.ToolTip"));
        //
        // Cb_ExtendTime
        //
        resources.ApplyResources(Cb_ExtendTime, "Cb_ExtendTime");
        Cb_ExtendTime.Name = "Cb_ExtendTime";
        Pref_TlTip.SetToolTip(Cb_ExtendTime, resources.GetString("Cb_ExtendTime.ToolTip"));
        Cb_ExtendTime.UseVisualStyleBackColor = true;
        //
        // Tb_Delay_Stop
        //
        resources.ApplyResources(Tb_Delay_Stop, "Tb_Delay_Stop");
        Tb_Delay_Stop.Name = "Tb_Delay_Stop";
        Pref_TlTip.SetToolTip(Tb_Delay_Stop, resources.GetString("Tb_Delay_Stop.ToolTip"));
        //
        // Cbx_TypeStop
        //
        Cbx_TypeStop.FormattingEnabled = true;
        Cbx_TypeStop.Items.AddRange(new object[]
        {
            resources.GetString("Cbx_TypeStop.Items"),
            resources.GetString("Cbx_TypeStop.Items1"),
            resources.GetString("Cbx_TypeStop.Items2")
        });
        resources.ApplyResources(Cbx_TypeStop, "Cbx_TypeStop");
        Cbx_TypeStop.Name = "Cbx_TypeStop";
        Pref_TlTip.SetToolTip(Cbx_TypeStop, resources.GetString("Cbx_TypeStop.ToolTip"));
        //
        // Cb_ImmediateStop
        //
        resources.ApplyResources(Cb_ImmediateStop, "Cb_ImmediateStop");
        Cb_ImmediateStop.Name = "Cb_ImmediateStop";
        Pref_TlTip.SetToolTip(Cb_ImmediateStop, resources.GetString("Cb_ImmediateStop.ToolTip"));
        Cb_ImmediateStop.UseVisualStyleBackColor = true;
        //
        // Tb_BattLimit_Time
        //
        resources.ApplyResources(Tb_BattLimit_Time, "Tb_BattLimit_Time");
        Tb_BattLimit_Time.Name = "Tb_BattLimit_Time";
        Pref_TlTip.SetToolTip(Tb_BattLimit_Time, resources.GetString("Tb_BattLimit_Time.ToolTip"));
        //
        // Tb_BattLimit_Load
        //
        resources.ApplyResources(Tb_BattLimit_Load, "Tb_BattLimit_Load");
        Tb_BattLimit_Load.Name = "Tb_BattLimit_Load";
        Pref_TlTip.SetToolTip(Tb_BattLimit_Load, resources.GetString("Tb_BattLimit_Load.ToolTip"));
        //
        // Lbl_GraceTime
        //
        resources.ApplyResources(Lbl_GraceTime, "Lbl_GraceTime");
        Lbl_GraceTime.Name = "Lbl_GraceTime";
        //
        // Lbl_Delay_Stop
        //
        resources.ApplyResources(Lbl_Delay_Stop, "Lbl_Delay_Stop");
        Lbl_Delay_Stop.Name = "Lbl_Delay_Stop";
        //
        // Lbl_StopType
        //
        resources.ApplyResources(Lbl_StopType, "Lbl_StopType");
        Lbl_StopType.Name = "Lbl_StopType";
        //
        // Lbl_BattLimit_Time
        //
        resources.ApplyResources(Lbl_BattLimit_Time, "Lbl_BattLimit_Time");
        Lbl_BattLimit_Time.Name = "Lbl_BattLimit_Time";
        //
        // Lbl_BattLimit_Load
        //
        resources.ApplyResources(Lbl_BattLimit_Load, "Lbl_BattLimit_Load");
        Lbl_BattLimit_Load.Name = "Lbl_BattLimit_Load";
        //
        // Tab_Update
        //
        Tab_Update.Controls.Add(Cbx_Branch_Update);
        Tab_Update.Controls.Add(Cbx_Delay_Verif);
        Tab_Update.Controls.Add(Lbl_Branch_Update);
        Tab_Update.Controls.Add(Lbl_Delay_Verif);
        Tab_Update.Controls.Add(Cb_Update_At_Start);
        resources.ApplyResources(Tab_Update, "Tab_Update");
        Tab_Update.Name = "Tab_Update";
        Tab_Update.UseVisualStyleBackColor = true;
        //
        // Cbx_Branch_Update
        //
        Cbx_Branch_Update.FormattingEnabled = true;
        Cbx_Branch_Update.Items.AddRange(new object[]
        {
            resources.GetString("Cbx_Branch_Update.Items"),
            resources.GetString("Cbx_Branch_Update.Items1")
        });
        resources.ApplyResources(Cbx_Branch_Update, "Cbx_Branch_Update");
        Cbx_Branch_Update.Name = "Cbx_Branch_Update";
        Pref_TlTip.SetToolTip(Cbx_Branch_Update, resources.GetString("Cbx_Branch_Update.ToolTip"));
        //
        // Cbx_Delay_Verif
        //
        Cbx_Delay_Verif.FormattingEnabled = true;
        Cbx_Delay_Verif.Items.AddRange(new object[]
        {
            resources.GetString("Cbx_Delay_Verif.Items"),
            resources.GetString("Cbx_Delay_Verif.Items1"),
            resources.GetString("Cbx_Delay_Verif.Items2")
        });
        resources.ApplyResources(Cbx_Delay_Verif, "Cbx_Delay_Verif");
        Cbx_Delay_Verif.Name = "Cbx_Delay_Verif";
        Pref_TlTip.SetToolTip(Cbx_Delay_Verif, resources.GetString("Cbx_Delay_Verif.ToolTip"));
        //
        // Lbl_Branch_Update
        //
        resources.ApplyResources(Lbl_Branch_Update, "Lbl_Branch_Update");
        Lbl_Branch_Update.Name = "Lbl_Branch_Update";
        //
        // Lbl_Delay_Verif
        //
        resources.ApplyResources(Lbl_Delay_Verif, "Lbl_Delay_Verif");
        Lbl_Delay_Verif.Name = "Lbl_Delay_Verif";
        //
        // Cb_Update_At_Start
        //
        resources.ApplyResources(Cb_Update_At_Start, "Cb_Update_At_Start");
        Cb_Update_At_Start.Name = "Cb_Update_At_Start";
        Pref_TlTip.SetToolTip(Cb_Update_At_Start, resources.GetString("Cb_Update_At_Start.ToolTip"));
        Cb_Update_At_Start.UseVisualStyleBackColor = true;
        //
        // Btn_Ok
        //
        resources.ApplyResources(Btn_Ok, "Btn_Ok");
        Btn_Ok.Name = "Btn_Ok";
        Btn_Ok.UseVisualStyleBackColor = true;
        //
        // Btn_Apply
        //
        resources.ApplyResources(Btn_Apply, "Btn_Apply");
        Btn_Apply.Name = "Btn_Apply";
        Btn_Apply.UseVisualStyleBackColor = true;
        //
        // Btn_Cancel
        //
        resources.ApplyResources(Btn_Cancel, "Btn_Cancel");
        Btn_Cancel.Name = "Btn_Cancel";
        //
        // PrefGui
        //
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(Btn_Cancel);
        Controls.Add(Btn_Apply);
        Controls.Add(Btn_Ok);
        Controls.Add(TabControl_Options);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "PrefGui";
        ShowIcon = false;
        TabControl_Options.ResumeLayout(false);
        Tab_Connexion.ResumeLayout(false);
        Tab_Connexion.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pollingIntervalValue).EndInit();
        Tab_Calibrage.ResumeLayout(false);
        Tab_Calibrage.PerformLayout();
        Tab_Miscellanous.ResumeLayout(false);
        Tab_Miscellanous.PerformLayout();
        Tab_Shutdown.ResumeLayout(false);
        Tab_Shutdown.PerformLayout();
        Tab_Update.ResumeLayout(false);
        Tab_Update.PerformLayout();
        ResumeLayout(false);
    }

    internal TabControl TabControl_Options;
    internal TabPage Tab_Connexion;
    internal CheckBox Cb_Reconnect;
    internal Label Lbl_Delay_Com;
    internal Label Lbl_Name_UPS;
    internal Label Lbl_Port;
    internal Label Lbl_Server_IP;
    internal TabPage Tab_Calibrage;
    internal TabPage Tab_Miscellanous;
    internal TabPage Tab_Shutdown;
    internal TabPage Tab_Update;
    internal Button Btn_Ok;
    internal Button Btn_Apply;
    internal TextBox Tb_Server_IP;
    internal TextBox Tb_Port;
    internal TextBox Tb_UPS_Name;
    internal Label Lbl_InputV;
    internal Label Lbl_BattV;
    internal Label Lbl_LoadUPS;
    internal Label Lbl_OutputV;
    internal Label Lbl_InputF;
    internal Label Lbl_PowerF;
    internal TextBox Tb_InV_Min;
    internal Label Lbl_Maxi;
    internal Label Lbl_Mini;
    internal ComboBox Cbx_Freq_Input;
    internal TextBox Tb_BattV_Max;
    internal TextBox Tb_OutV_Max;
    internal TextBox Tb_InF_Max;
    internal TextBox Tb_BattV_Min;
    internal TextBox Tb_OutV_Min;
    internal TextBox Tb_InF_Min;
    internal TextBox Tb_InV_Max;
    internal Button Btn_DeleteLog;
    internal Button Btn_ViewLog;
    internal Label Lbl_LevelLog;
    internal CheckBox CB_Use_Logfile;
    internal CheckBox CB_Start_W_Win;
    internal CheckBox CB_Close_Tray;
    internal CheckBox CB_Start_Mini;
    internal CheckBox CB_Systray;
    internal ComboBox Cbx_LogLevel;
    internal Label Lbl_Delay_Stop;
    internal Label Lbl_StopType;
    internal Label Lbl_BattLimit_Time;
    internal Label Lbl_BattLimit_Load;
    internal Label Lbl_Percent;
    internal TextBox Tb_GraceTime;
    internal CheckBox Cb_ExtendTime;
    internal TextBox Tb_Delay_Stop;
    internal ComboBox Cbx_TypeStop;
    internal CheckBox Cb_ImmediateStop;
    internal TextBox Tb_BattLimit_Time;
    internal TextBox Tb_BattLimit_Load;
    internal Label Lbl_GraceTime;
    internal ToolTip Pref_TlTip;
    internal ComboBox Cbx_Branch_Update;
    internal ComboBox Cbx_Delay_Verif;
    internal Label Lbl_Branch_Update;
    internal Label Lbl_Delay_Verif;
    internal CheckBox Cb_Update_At_Start;
    internal TextBox Tb_Pwd_Nut;
    internal TextBox Tb_Login_Nut;
    internal Label Label3;
    internal Label Lbl_Pwd_Nut;
    internal Label Lbl_Login_Nut;
    internal CheckBox CB_Follow_FSD;
    internal Label pollingIntervalUnitLabel;
    internal NumericUpDown pollingIntervalValue;
    internal Button Btn_Cancel;
}
