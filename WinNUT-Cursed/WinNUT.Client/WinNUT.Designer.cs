#nullable enable
using System.ComponentModel;
using System.Drawing;
using WinNUT_Client.Controls;

namespace WinNUT_Client;

partial class WinNUT
{
    private IContainer? components;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        var resources = new ComponentResourceManager(typeof(WinNUT));
        NotifyIcon = new NotifyIcon(components);
        ContextMenu_Systray = new ContextMenuStrip(components);
        Menu_Sys_Settings = new ToolStripMenuItem();
        Menu_Sys_Sep1 = new ToolStripSeparator();
        Menu_Sys_Update = new ToolStripMenuItem();
        Menu_Sys_Sep2 = new ToolStripSeparator();
        Menu_Sys_About = new ToolStripMenuItem();
        Menu_Sys_Sep3 = new ToolStripSeparator();
        Menu_Sys_Exit = new ToolStripMenuItem();
        Main_Menu = new MenuStrip();
        Menu_File = new ToolStripMenuItem();
        Menu_UPS_Var = new ToolStripMenuItem();
        ManageOldPrefsToolStripMenuItem = new ToolStripMenuItem();
        Menu_Quit = new ToolStripMenuItem();
        Menu_Connection = new ToolStripMenuItem();
        Menu_Persist = new ToolStripMenuItem();
        ToolStripSeparator1 = new ToolStripSeparator();
        Menu_Connect = new ToolStripMenuItem();
        Menu_Disconnect = new ToolStripMenuItem();
        Menu_Settings = new ToolStripMenuItem();
        Menu_Help = new ToolStripMenuItem();
        Menu_About = new ToolStripMenuItem();
        Menu_Help_Sep1 = new ToolStripSeparator();
        Menu_Update = new ToolStripMenuItem();
        GB_Status = new GroupBox();
        Lbl_VSerial = new Label();
        Lbl_VFirmware = new Label();
        Lbl_VName = new Label();
        Lbl_VMfr = new Label();
        Lbl_VRTime = new Label();
        Lbl_VOB = new Label();
        Lbl_VOLoad = new Label();
        Lbl_VBL = new Label();
        Lbl_VOL = new Label();
        Lbl_Firmware = new Label();
        Lbl_Serial = new Label();
        Lbl_Name = new Label();
        Lbl_Mfr = new Label();
        Lbl_RTime = new Label();
        Lbl_BL = new Label();
        Lbl_OLoad = new Label();
        Lbl_OB = new Label();
        Lbl_OL = new Label();
        GB_InV_Dial = new GroupBox();
        AG_InV = new UPSVarGauge();
        Lbl_InV_Dial = new Label();
        GB_OutV_Dial = new GroupBox();
        AG_OutV = new UPSVarGauge();
        Lbl_OutV_Dial = new Label();
        GB_BattCh_Dial = new GroupBox();
        PBox_Battery_State = new PictureBox();
        Lbl_BattCh_Dial = new Label();
        AG_BattCh = new UPSVarGauge();
        GB_Load_Dial = new GroupBox();
        AG_Load = new UPSVarGauge();
        Lbl_Load_Dial = new Label();
        GB_BattV_Dial = new GroupBox();
        AG_BattV = new UPSVarGauge();
        Lbl_BattV_Dial = new Label();
        GB_InF_Dial = new GroupBox();
        AG_InF = new UPSVarGauge();
        Lbl_InF_Dial = new Label();
        CB_CurrentLog = new ComboBox();
        ContextMenu_Systray.SuspendLayout();
        Main_Menu.SuspendLayout();
        GB_Status.SuspendLayout();
        GB_InV_Dial.SuspendLayout();
        GB_OutV_Dial.SuspendLayout();
        GB_BattCh_Dial.SuspendLayout();
        ((ISupportInitialize)PBox_Battery_State).BeginInit();
        GB_Load_Dial.SuspendLayout();
        GB_BattV_Dial.SuspendLayout();
        GB_InF_Dial.SuspendLayout();
        SuspendLayout();
        //
        // NotifyIcon
        //
        NotifyIcon.ContextMenuStrip = ContextMenu_Systray;
        resources.ApplyResources(NotifyIcon, "NotifyIcon");
        //
        // ContextMenu_Systray
        //
        ContextMenu_Systray.Items.AddRange(new ToolStripItem[]
        {
            Menu_Sys_Settings, Menu_Sys_Sep1, Menu_Sys_Update, Menu_Sys_Sep2, Menu_Sys_About, Menu_Sys_Sep3, Menu_Sys_Exit
        });
        ContextMenu_Systray.Name = "ContextMenuStrip1";
        resources.ApplyResources(ContextMenu_Systray, "ContextMenu_Systray");
        //
        // Menu_Sys_Settings
        //
        Menu_Sys_Settings.Name = "Menu_Sys_Settings";
        resources.ApplyResources(Menu_Sys_Settings, "Menu_Sys_Settings");
        //
        // Menu_Sys_Sep1
        //
        Menu_Sys_Sep1.Name = "Menu_Sys_Sep1";
        resources.ApplyResources(Menu_Sys_Sep1, "Menu_Sys_Sep1");
        //
        // Menu_Sys_Update
        //
        Menu_Sys_Update.Name = "Menu_Sys_Update";
        resources.ApplyResources(Menu_Sys_Update, "Menu_Sys_Update");
        //
        // Menu_Sys_Sep2
        //
        Menu_Sys_Sep2.Name = "Menu_Sys_Sep2";
        resources.ApplyResources(Menu_Sys_Sep2, "Menu_Sys_Sep2");
        //
        // Menu_Sys_About
        //
        Menu_Sys_About.Name = "Menu_Sys_About";
        resources.ApplyResources(Menu_Sys_About, "Menu_Sys_About");
        //
        // Menu_Sys_Sep3
        //
        Menu_Sys_Sep3.Name = "Menu_Sys_Sep3";
        resources.ApplyResources(Menu_Sys_Sep3, "Menu_Sys_Sep3");
        //
        // Menu_Sys_Exit
        //
        Menu_Sys_Exit.Name = "Menu_Sys_Exit";
        resources.ApplyResources(Menu_Sys_Exit, "Menu_Sys_Exit");
        //
        // Main_Menu
        //
        resources.ApplyResources(Main_Menu, "Main_Menu");
        Main_Menu.Items.AddRange(new ToolStripItem[] { Menu_File, Menu_Connection, Menu_Settings, Menu_Help });
        Main_Menu.Name = "Main_Menu";
        //
        // Menu_File
        //
        Menu_File.DropDownItems.AddRange(new ToolStripItem[] { Menu_UPS_Var, ManageOldPrefsToolStripMenuItem, Menu_Quit });
        Menu_File.Name = "Menu_File";
        resources.ApplyResources(Menu_File, "Menu_File");
        //
        // Menu_UPS_Var
        //
        resources.ApplyResources(Menu_UPS_Var, "Menu_UPS_Var");
        Menu_UPS_Var.Name = "Menu_UPS_Var";
        //
        // ManageOldPrefsToolStripMenuItem
        //
        resources.ApplyResources(ManageOldPrefsToolStripMenuItem, "ManageOldPrefsToolStripMenuItem");
        ManageOldPrefsToolStripMenuItem.Image = Properties.Resources.regedit_exe_14_100_0;
        ManageOldPrefsToolStripMenuItem.Name = "ManageOldPrefsToolStripMenuItem";
        //
        // Menu_Quit
        //
        Menu_Quit.Name = "Menu_Quit";
        resources.ApplyResources(Menu_Quit, "Menu_Quit");
        //
        // Menu_Connection
        //
        Menu_Connection.DropDownItems.AddRange(new ToolStripItem[] { Menu_Persist, ToolStripSeparator1, Menu_Connect, Menu_Disconnect });
        Menu_Connection.Name = "Menu_Connection";
        resources.ApplyResources(Menu_Connection, "Menu_Connection");
        //
        // Menu_Persist
        //
        Menu_Persist.Checked = Properties.Settings.Default.NUT_AutoReconnect;
        Menu_Persist.CheckOnClick = true;
        Menu_Persist.Image = Properties.Resources.RepeatHS;
        Menu_Persist.Name = "Menu_Persist";
        resources.ApplyResources(Menu_Persist, "Menu_Persist");
        //
        // ToolStripSeparator1
        //
        ToolStripSeparator1.Name = "ToolStripSeparator1";
        resources.ApplyResources(ToolStripSeparator1, "ToolStripSeparator1");
        //
        // Menu_Connect
        //
        Menu_Connect.Image = Properties.Resources.internetconnection;
        Menu_Connect.Name = "Menu_Connect";
        resources.ApplyResources(Menu_Connect, "Menu_Connect");
        //
        // Menu_Disconnect
        //
        resources.ApplyResources(Menu_Disconnect, "Menu_Disconnect");
        Menu_Disconnect.Image = Properties.Resources.disconnect2;
        Menu_Disconnect.Name = "Menu_Disconnect";
        //
        // Menu_Settings
        //
        Menu_Settings.Name = "Menu_Settings";
        resources.ApplyResources(Menu_Settings, "Menu_Settings");
        //
        // Menu_Help
        //
        Menu_Help.DropDownItems.AddRange(new ToolStripItem[] { Menu_About, Menu_Help_Sep1, Menu_Update });
        Menu_Help.Name = "Menu_Help";
        resources.ApplyResources(Menu_Help, "Menu_Help");
        //
        // Menu_About
        //
        Menu_About.Name = "Menu_About";
        resources.ApplyResources(Menu_About, "Menu_About");
        //
        // Menu_Help_Sep1
        //
        Menu_Help_Sep1.Name = "Menu_Help_Sep1";
        resources.ApplyResources(Menu_Help_Sep1, "Menu_Help_Sep1");
        //
        // Menu_Update
        //
        Menu_Update.Name = "Menu_Update";
        resources.ApplyResources(Menu_Update, "Menu_Update");
        //
        // GB_Status
        //
        resources.ApplyResources(GB_Status, "GB_Status");
        GB_Status.Controls.Add(Lbl_VSerial);
        GB_Status.Controls.Add(Lbl_VFirmware);
        GB_Status.Controls.Add(Lbl_VName);
        GB_Status.Controls.Add(Lbl_VMfr);
        GB_Status.Controls.Add(Lbl_VRTime);
        GB_Status.Controls.Add(Lbl_VOB);
        GB_Status.Controls.Add(Lbl_VOLoad);
        GB_Status.Controls.Add(Lbl_VBL);
        GB_Status.Controls.Add(Lbl_VOL);
        GB_Status.Controls.Add(Lbl_Firmware);
        GB_Status.Controls.Add(Lbl_Serial);
        GB_Status.Controls.Add(Lbl_Name);
        GB_Status.Controls.Add(Lbl_Mfr);
        GB_Status.Controls.Add(Lbl_RTime);
        GB_Status.Controls.Add(Lbl_BL);
        GB_Status.Controls.Add(Lbl_OLoad);
        GB_Status.Controls.Add(Lbl_OB);
        GB_Status.Controls.Add(Lbl_OL);
        GB_Status.Name = "GB_Status";
        GB_Status.TabStop = false;
        //
        // Lbl_VSerial
        //
        Lbl_VSerial.CausesValidation = false;
        resources.ApplyResources(Lbl_VSerial, "Lbl_VSerial");
        Lbl_VSerial.Name = "Lbl_VSerial";
        //
        // Lbl_VFirmware
        //
        Lbl_VFirmware.CausesValidation = false;
        resources.ApplyResources(Lbl_VFirmware, "Lbl_VFirmware");
        Lbl_VFirmware.Name = "Lbl_VFirmware";
        Lbl_VFirmware.UseCompatibleTextRendering = true;
        //
        // Lbl_VName
        //
        Lbl_VName.CausesValidation = false;
        resources.ApplyResources(Lbl_VName, "Lbl_VName");
        Lbl_VName.Name = "Lbl_VName";
        //
        // Lbl_VMfr
        //
        Lbl_VMfr.CausesValidation = false;
        resources.ApplyResources(Lbl_VMfr, "Lbl_VMfr");
        Lbl_VMfr.Name = "Lbl_VMfr";
        //
        // Lbl_VRTime
        //
        Lbl_VRTime.CausesValidation = false;
        resources.ApplyResources(Lbl_VRTime, "Lbl_VRTime");
        Lbl_VRTime.Name = "Lbl_VRTime";
        //
        // Lbl_VOB
        //
        Lbl_VOB.BackColor = Color.White;
        Lbl_VOB.BorderStyle = BorderStyle.Fixed3D;
        Lbl_VOB.CausesValidation = false;
        resources.ApplyResources(Lbl_VOB, "Lbl_VOB");
        Lbl_VOB.Name = "Lbl_VOB";
        //
        // Lbl_VOLoad
        //
        Lbl_VOLoad.BackColor = Color.White;
        Lbl_VOLoad.BorderStyle = BorderStyle.Fixed3D;
        Lbl_VOLoad.CausesValidation = false;
        resources.ApplyResources(Lbl_VOLoad, "Lbl_VOLoad");
        Lbl_VOLoad.Name = "Lbl_VOLoad";
        //
        // Lbl_VBL
        //
        Lbl_VBL.BackColor = Color.White;
        Lbl_VBL.BorderStyle = BorderStyle.Fixed3D;
        Lbl_VBL.CausesValidation = false;
        resources.ApplyResources(Lbl_VBL, "Lbl_VBL");
        Lbl_VBL.Name = "Lbl_VBL";
        //
        // Lbl_VOL
        //
        Lbl_VOL.BackColor = Color.White;
        Lbl_VOL.BorderStyle = BorderStyle.Fixed3D;
        Lbl_VOL.CausesValidation = false;
        resources.ApplyResources(Lbl_VOL, "Lbl_VOL");
        Lbl_VOL.Name = "Lbl_VOL";
        //
        // Lbl_Firmware
        //
        Lbl_Firmware.CausesValidation = false;
        resources.ApplyResources(Lbl_Firmware, "Lbl_Firmware");
        Lbl_Firmware.Name = "Lbl_Firmware";
        Lbl_Firmware.UseCompatibleTextRendering = true;
        //
        // Lbl_Serial
        //
        Lbl_Serial.CausesValidation = false;
        resources.ApplyResources(Lbl_Serial, "Lbl_Serial");
        Lbl_Serial.Name = "Lbl_Serial";
        //
        // Lbl_Name
        //
        Lbl_Name.CausesValidation = false;
        resources.ApplyResources(Lbl_Name, "Lbl_Name");
        Lbl_Name.Name = "Lbl_Name";
        //
        // Lbl_Mfr
        //
        Lbl_Mfr.CausesValidation = false;
        resources.ApplyResources(Lbl_Mfr, "Lbl_Mfr");
        Lbl_Mfr.Name = "Lbl_Mfr";
        //
        // Lbl_RTime
        //
        Lbl_RTime.CausesValidation = false;
        resources.ApplyResources(Lbl_RTime, "Lbl_RTime");
        Lbl_RTime.Name = "Lbl_RTime";
        //
        // Lbl_BL
        //
        Lbl_BL.CausesValidation = false;
        resources.ApplyResources(Lbl_BL, "Lbl_BL");
        Lbl_BL.Name = "Lbl_BL";
        //
        // Lbl_OLoad
        //
        Lbl_OLoad.CausesValidation = false;
        resources.ApplyResources(Lbl_OLoad, "Lbl_OLoad");
        Lbl_OLoad.Name = "Lbl_OLoad";
        //
        // Lbl_OB
        //
        Lbl_OB.CausesValidation = false;
        resources.ApplyResources(Lbl_OB, "Lbl_OB");
        Lbl_OB.Name = "Lbl_OB";
        //
        // Lbl_OL
        //
        Lbl_OL.CausesValidation = false;
        resources.ApplyResources(Lbl_OL, "Lbl_OL");
        Lbl_OL.Name = "Lbl_OL";
        //
        // GB_InV_Dial
        //
        resources.ApplyResources(GB_InV_Dial, "GB_InV_Dial");
        GB_InV_Dial.Controls.Add(AG_InV);
        GB_InV_Dial.Controls.Add(Lbl_InV_Dial);
        GB_InV_Dial.Name = "GB_InV_Dial";
        GB_InV_Dial.TabStop = false;
        //
        // AG_InV
        //
        AG_InV.BaseArcRadius = 45;
        AG_InV.BaseArcWidth = 5;
        AG_InV.GradientOrientation = UPSVarGauge.GradientOrientationEnum.BottomToTop;
        AG_InV.GradientType = UPSVarGauge.GradientTypeEnum.RedGreen;
        resources.ApplyResources(AG_InV, "AG_InV");
        AG_InV.MaxValue = 100;
        AG_InV.MinValue = 0;
        AG_InV.Name = "AG_InV";
        AG_InV.NeedleRadius = 32;
        AG_InV.ScaleLinesInterInnerRadius = 40;
        AG_InV.ScaleLinesInterOuterRadius = 48;
        AG_InV.ScaleLinesMajorInnerRadius = 40;
        AG_InV.ScaleLinesMajorOuterRadius = 48;
        AG_InV.ScaleLinesMinorInnerRadius = 42;
        AG_InV.ScaleLinesMinorOuterRadius = 48;
        AG_InV.ScaleNumbersFormat = null;
        AG_InV.ScaleNumbersRadius = 60;
        AG_InV.UnitValue1 = UPSVarGauge.UnitValueEnum.Volts;
        AG_InV.UnitValue2 = UPSVarGauge.UnitValueEnum.None;
        AG_InV.Value = 0f;
        AG_InV.Value1 = 0f;
        AG_InV.Value2 = 0f;
        //
        // Lbl_InV_Dial
        //
        resources.ApplyResources(Lbl_InV_Dial, "Lbl_InV_Dial");
        Lbl_InV_Dial.Name = "Lbl_InV_Dial";
        //
        // GB_OutV_Dial
        //
        resources.ApplyResources(GB_OutV_Dial, "GB_OutV_Dial");
        GB_OutV_Dial.Controls.Add(AG_OutV);
        GB_OutV_Dial.Controls.Add(Lbl_OutV_Dial);
        GB_OutV_Dial.Name = "GB_OutV_Dial";
        GB_OutV_Dial.TabStop = false;
        //
        // AG_OutV
        //
        AG_OutV.BaseArcRadius = 45;
        AG_OutV.BaseArcWidth = 5;
        AG_OutV.GradientOrientation = UPSVarGauge.GradientOrientationEnum.BottomToTop;
        AG_OutV.GradientType = UPSVarGauge.GradientTypeEnum.RedGreen;
        resources.ApplyResources(AG_OutV, "AG_OutV");
        AG_OutV.MaxValue = 100;
        AG_OutV.MinValue = 0;
        AG_OutV.Name = "AG_OutV";
        AG_OutV.NeedleRadius = 32;
        AG_OutV.ScaleLinesInterInnerRadius = 40;
        AG_OutV.ScaleLinesInterOuterRadius = 48;
        AG_OutV.ScaleLinesMajorInnerRadius = 40;
        AG_OutV.ScaleLinesMajorOuterRadius = 48;
        AG_OutV.ScaleLinesMinorInnerRadius = 42;
        AG_OutV.ScaleLinesMinorOuterRadius = 48;
        AG_OutV.ScaleNumbersFormat = null;
        AG_OutV.ScaleNumbersRadius = 60;
        AG_OutV.UnitValue1 = UPSVarGauge.UnitValueEnum.Volts;
        AG_OutV.UnitValue2 = UPSVarGauge.UnitValueEnum.None;
        AG_OutV.Value = 0f;
        AG_OutV.Value1 = 0f;
        AG_OutV.Value2 = 0f;
        //
        // Lbl_OutV_Dial
        //
        resources.ApplyResources(Lbl_OutV_Dial, "Lbl_OutV_Dial");
        Lbl_OutV_Dial.Name = "Lbl_OutV_Dial";
        //
        // GB_BattCh_Dial
        //
        resources.ApplyResources(GB_BattCh_Dial, "GB_BattCh_Dial");
        GB_BattCh_Dial.Controls.Add(PBox_Battery_State);
        GB_BattCh_Dial.Controls.Add(Lbl_BattCh_Dial);
        GB_BattCh_Dial.Controls.Add(AG_BattCh);
        GB_BattCh_Dial.Name = "GB_BattCh_Dial";
        GB_BattCh_Dial.TabStop = false;
        //
        // PBox_Battery_State
        //
        resources.ApplyResources(PBox_Battery_State, "PBox_Battery_State");
        PBox_Battery_State.Name = "PBox_Battery_State";
        PBox_Battery_State.TabStop = false;
        //
        // Lbl_BattCh_Dial
        //
        resources.ApplyResources(Lbl_BattCh_Dial, "Lbl_BattCh_Dial");
        Lbl_BattCh_Dial.Name = "Lbl_BattCh_Dial";
        //
        // AG_BattCh
        //
        AG_BattCh.BaseArcRadius = 45;
        AG_BattCh.BaseArcWidth = 5;
        AG_BattCh.GradientOrientation = UPSVarGauge.GradientOrientationEnum.LeftToRight;
        AG_BattCh.GradientType = UPSVarGauge.GradientTypeEnum.RedGreen;
        resources.ApplyResources(AG_BattCh, "AG_BattCh");
        AG_BattCh.MaxValue = 100;
        AG_BattCh.MinValue = 0;
        AG_BattCh.Name = "AG_BattCh";
        AG_BattCh.NeedleRadius = 32;
        AG_BattCh.ScaleLinesInterInnerRadius = 40;
        AG_BattCh.ScaleLinesInterOuterRadius = 48;
        AG_BattCh.ScaleLinesMajorInnerRadius = 40;
        AG_BattCh.ScaleLinesMajorOuterRadius = 48;
        AG_BattCh.ScaleLinesMinorInnerRadius = 42;
        AG_BattCh.ScaleLinesMinorOuterRadius = 48;
        AG_BattCh.ScaleNumbersFormat = null;
        AG_BattCh.ScaleNumbersRadius = 60;
        AG_BattCh.UnitValue1 = UPSVarGauge.UnitValueEnum.Percent;
        AG_BattCh.UnitValue2 = UPSVarGauge.UnitValueEnum.None;
        AG_BattCh.Value = 0f;
        AG_BattCh.Value1 = 0f;
        AG_BattCh.Value2 = 0f;
        //
        // GB_Load_Dial
        //
        resources.ApplyResources(GB_Load_Dial, "GB_Load_Dial");
        GB_Load_Dial.Controls.Add(AG_Load);
        GB_Load_Dial.Controls.Add(Lbl_Load_Dial);
        GB_Load_Dial.Name = "GB_Load_Dial";
        GB_Load_Dial.TabStop = false;
        //
        // AG_Load
        //
        AG_Load.BaseArcRadius = 45;
        AG_Load.BaseArcWidth = 5;
        AG_Load.GradientOrientation = UPSVarGauge.GradientOrientationEnum.RightToLeft;
        AG_Load.GradientType = UPSVarGauge.GradientTypeEnum.RedGreen;
        resources.ApplyResources(AG_Load, "AG_Load");
        AG_Load.MaxValue = 100;
        AG_Load.MinValue = 0;
        AG_Load.Name = "AG_Load";
        AG_Load.NeedleRadius = 32;
        AG_Load.ScaleLinesInterInnerRadius = 40;
        AG_Load.ScaleLinesInterOuterRadius = 48;
        AG_Load.ScaleLinesMajorInnerRadius = 40;
        AG_Load.ScaleLinesMajorOuterRadius = 48;
        AG_Load.ScaleLinesMinorInnerRadius = 42;
        AG_Load.ScaleLinesMinorOuterRadius = 48;
        AG_Load.ScaleNumbersFormat = null;
        AG_Load.ScaleNumbersRadius = 60;
        AG_Load.UnitValue1 = UPSVarGauge.UnitValueEnum.Percent;
        AG_Load.UnitValue2 = UPSVarGauge.UnitValueEnum.Watts;
        AG_Load.Value = 0f;
        AG_Load.Value1 = 0f;
        AG_Load.Value2 = 0f;
        //
        // Lbl_Load_Dial
        //
        resources.ApplyResources(Lbl_Load_Dial, "Lbl_Load_Dial");
        Lbl_Load_Dial.Name = "Lbl_Load_Dial";
        //
        // GB_BattV_Dial
        //
        resources.ApplyResources(GB_BattV_Dial, "GB_BattV_Dial");
        GB_BattV_Dial.Controls.Add(AG_BattV);
        GB_BattV_Dial.Controls.Add(Lbl_BattV_Dial);
        GB_BattV_Dial.Name = "GB_BattV_Dial";
        GB_BattV_Dial.TabStop = false;
        //
        // AG_BattV
        //
        AG_BattV.BaseArcRadius = 45;
        AG_BattV.BaseArcWidth = 5;
        AG_BattV.GradientOrientation = UPSVarGauge.GradientOrientationEnum.BottomToTop;
        AG_BattV.GradientType = UPSVarGauge.GradientTypeEnum.RedGreen;
        resources.ApplyResources(AG_BattV, "AG_BattV");
        AG_BattV.MaxValue = 100;
        AG_BattV.MinValue = 0;
        AG_BattV.Name = "AG_BattV";
        AG_BattV.NeedleRadius = 32;
        AG_BattV.ScaleLinesInterInnerRadius = 40;
        AG_BattV.ScaleLinesInterOuterRadius = 48;
        AG_BattV.ScaleLinesMajorInnerRadius = 40;
        AG_BattV.ScaleLinesMajorOuterRadius = 48;
        AG_BattV.ScaleLinesMinorInnerRadius = 42;
        AG_BattV.ScaleLinesMinorOuterRadius = 48;
        AG_BattV.ScaleNumbersFormat = null;
        AG_BattV.ScaleNumbersRadius = 60;
        AG_BattV.UnitValue1 = UPSVarGauge.UnitValueEnum.Volts;
        AG_BattV.UnitValue2 = UPSVarGauge.UnitValueEnum.None;
        AG_BattV.Value = 0f;
        AG_BattV.Value1 = 0f;
        AG_BattV.Value2 = 0f;
        //
        // Lbl_BattV_Dial
        //
        resources.ApplyResources(Lbl_BattV_Dial, "Lbl_BattV_Dial");
        Lbl_BattV_Dial.Name = "Lbl_BattV_Dial";
        //
        // GB_InF_Dial
        //
        resources.ApplyResources(GB_InF_Dial, "GB_InF_Dial");
        GB_InF_Dial.Controls.Add(AG_InF);
        GB_InF_Dial.Controls.Add(Lbl_InF_Dial);
        GB_InF_Dial.Name = "GB_InF_Dial";
        GB_InF_Dial.TabStop = false;
        //
        // AG_InF
        //
        AG_InF.BaseArcRadius = 45;
        AG_InF.BaseArcWidth = 5;
        AG_InF.GradientOrientation = UPSVarGauge.GradientOrientationEnum.BottomToTop;
        AG_InF.GradientType = UPSVarGauge.GradientTypeEnum.RedGreen;
        resources.ApplyResources(AG_InF, "AG_InF");
        AG_InF.MaxValue = 100;
        AG_InF.MinValue = 0;
        AG_InF.Name = "AG_InF";
        AG_InF.NeedleRadius = 32;
        AG_InF.ScaleLinesInterInnerRadius = 40;
        AG_InF.ScaleLinesInterOuterRadius = 48;
        AG_InF.ScaleLinesMajorInnerRadius = 40;
        AG_InF.ScaleLinesMajorOuterRadius = 48;
        AG_InF.ScaleLinesMinorInnerRadius = 42;
        AG_InF.ScaleLinesMinorOuterRadius = 48;
        AG_InF.ScaleNumbersFormat = null;
        AG_InF.ScaleNumbersRadius = 60;
        AG_InF.UnitValue1 = UPSVarGauge.UnitValueEnum.Hertz;
        AG_InF.UnitValue2 = UPSVarGauge.UnitValueEnum.None;
        AG_InF.Value = 0f;
        AG_InF.Value1 = 0f;
        AG_InF.Value2 = 0f;
        //
        // Lbl_InF_Dial
        //
        resources.ApplyResources(Lbl_InF_Dial, "Lbl_InF_Dial");
        Lbl_InF_Dial.Name = "Lbl_InF_Dial";
        //
        // CB_CurrentLog
        //
        CB_CurrentLog.CausesValidation = false;
        CB_CurrentLog.DropDownStyle = ComboBoxStyle.DropDownList;
        resources.ApplyResources(CB_CurrentLog, "CB_CurrentLog");
        CB_CurrentLog.Name = "CB_CurrentLog";
        //
        // WinNUT
        //
        AutoScaleMode = AutoScaleMode.None;
        AutoValidate = AutoValidate.Disable;
        resources.ApplyResources(this, "$this");
        Controls.Add(CB_CurrentLog);
        Controls.Add(GB_InF_Dial);
        Controls.Add(GB_InV_Dial);
        Controls.Add(GB_BattV_Dial);
        Controls.Add(GB_Load_Dial);
        Controls.Add(GB_OutV_Dial);
        Controls.Add(GB_Status);
        Controls.Add(GB_BattCh_Dial);
        Controls.Add(Main_Menu);
        DoubleBuffered = true;
        MainMenuStrip = Main_Menu;
        MaximizeBox = false;
        Name = "WinNUT";
        ContextMenu_Systray.ResumeLayout(false);
        Main_Menu.ResumeLayout(false);
        Main_Menu.PerformLayout();
        GB_Status.ResumeLayout(false);
        GB_InV_Dial.ResumeLayout(false);
        GB_InV_Dial.PerformLayout();
        GB_OutV_Dial.ResumeLayout(false);
        GB_OutV_Dial.PerformLayout();
        GB_BattCh_Dial.ResumeLayout(false);
        GB_BattCh_Dial.PerformLayout();
        ((ISupportInitialize)PBox_Battery_State).EndInit();
        GB_Load_Dial.ResumeLayout(false);
        GB_Load_Dial.PerformLayout();
        GB_BattV_Dial.ResumeLayout(false);
        GB_BattV_Dial.PerformLayout();
        GB_InF_Dial.ResumeLayout(false);
        GB_InF_Dial.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    internal NotifyIcon NotifyIcon;
    internal MenuStrip Main_Menu;
    internal ToolStripMenuItem Menu_File;
    internal ToolStripMenuItem Menu_UPS_Var;
    internal ToolStripMenuItem Menu_Quit;
    internal ToolStripMenuItem Menu_Connection;
    internal ToolStripMenuItem Menu_Settings;
    internal ToolStripMenuItem Menu_Help;
    internal ToolStripMenuItem Menu_About;
    internal ToolStripSeparator Menu_Help_Sep1;
    internal ToolStripMenuItem Menu_Update;
    internal ToolStripMenuItem Menu_Connect;
    internal ToolStripMenuItem Menu_Disconnect;
    internal ContextMenuStrip ContextMenu_Systray;
    internal ToolStripMenuItem Menu_Sys_Settings;
    internal ToolStripSeparator Menu_Sys_Sep1;
    internal ToolStripMenuItem Menu_Sys_Update;
    internal ToolStripSeparator Menu_Sys_Sep2;
    internal ToolStripMenuItem Menu_Sys_About;
    internal ToolStripSeparator Menu_Sys_Sep3;
    internal ToolStripMenuItem Menu_Sys_Exit;
    internal GroupBox GB_Status;
    internal Label Lbl_VSerial;
    internal Label Lbl_VFirmware;
    internal Label Lbl_VName;
    internal Label Lbl_VMfr;
    internal Label Lbl_VRTime;
    internal Label Lbl_VOB;
    internal Label Lbl_VOLoad;
    internal Label Lbl_VBL;
    internal Label Lbl_VOL;
    internal Label Lbl_Firmware;
    internal Label Lbl_Serial;
    internal Label Lbl_Name;
    internal Label Lbl_Mfr;
    internal Label Lbl_RTime;
    internal Label Lbl_BL;
    internal Label Lbl_OLoad;
    internal Label Lbl_OB;
    internal Label Lbl_OL;
    internal GroupBox GB_BattCh_Dial;
    internal Label Lbl_BattCh_Dial;
    internal GroupBox GB_OutV_Dial;
    internal Label Lbl_OutV_Dial;
    internal GroupBox GB_Load_Dial;
    internal Label Lbl_Load_Dial;
    internal GroupBox GB_BattV_Dial;
    internal Label Lbl_BattV_Dial;
    internal GroupBox GB_InV_Dial;
    internal Label Lbl_InV_Dial;
    internal UPSVarGauge AG_InV;
    internal UPSVarGauge AG_OutV;
    internal UPSVarGauge AG_BattCh;
    internal UPSVarGauge AG_Load;
    internal UPSVarGauge AG_BattV;
    internal GroupBox GB_InF_Dial;
    internal UPSVarGauge AG_InF;
    internal Label Lbl_InF_Dial;
    internal PictureBox PBox_Battery_State;
    internal ToolStripMenuItem ManageOldPrefsToolStripMenuItem;
    internal ToolStripSeparator ToolStripSeparator1;
    internal ToolStripMenuItem Menu_Persist;
    private ComboBox CB_CurrentLog;
}
