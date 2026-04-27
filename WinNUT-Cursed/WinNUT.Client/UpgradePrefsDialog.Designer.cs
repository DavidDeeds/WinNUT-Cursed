#nullable enable
using WinNUT_Client.Models;
using WinNUT_Client.Properties;

namespace WinNUT_Client;

partial class UpgradePrefsDialog
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(UpgradePrefsDialog));
        PictureBox1 = new PictureBox();
        IntroMessage = new Label();
        OK_Button = new Button();
        UpgradePrefsDialogModelBindingSource = new BindingSource(components);
        Cancel_Button = new Button();
        ImportSettingsCheckBox = new CheckBox();
        DeleteSettingsCheckBox = new CheckBox();
        PrevSettngsGroupBox = new GroupBox();
        BackupSettingsCheckbox = new CheckBox();
        buttonPanel = new Panel();
        UpgradeProgressBar = new ProgressBar();
        TopContentPanel = new Panel();
        ToolTip1 = new ToolTip(components);
        ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)UpgradePrefsDialogModelBindingSource).BeginInit();
        PrevSettngsGroupBox.SuspendLayout();
        buttonPanel.SuspendLayout();
        TopContentPanel.SuspendLayout();
        SuspendLayout();
        //
        // PictureBox1
        //
        PictureBox1.Image = Resources.XP_Information;
        resources.ApplyResources(PictureBox1, "PictureBox1");
        PictureBox1.Name = "PictureBox1";
        PictureBox1.TabStop = false;
        //
        // IntroMessage
        //
        resources.ApplyResources(IntroMessage, "IntroMessage");
        IntroMessage.Name = "IntroMessage";
        //
        // OK_Button
        //
        resources.ApplyResources(OK_Button, "OK_Button");
        OK_Button.DataBindings.Add("Enabled", UpgradePrefsDialogModelBindingSource, "OKButtonEnabled", true,
            DataSourceUpdateMode.OnPropertyChanged);
        OK_Button.Name = "OK_Button";
        //
        // UpgradePrefsDialogModelBindingSource
        //
        UpgradePrefsDialogModelBindingSource.DataSource = typeof(UpgradePrefsDialogModel);
        //
        // Cancel_Button
        //
        resources.ApplyResources(Cancel_Button, "Cancel_Button");
        Cancel_Button.DialogResult = DialogResult.Cancel;
        Cancel_Button.Name = "Cancel_Button";
        //
        // ImportSettingsCheckBox
        //
        resources.ApplyResources(ImportSettingsCheckBox, "ImportSettingsCheckBox");
        ImportSettingsCheckBox.Checked = true;
        ImportSettingsCheckBox.CheckState = CheckState.Checked;
        ImportSettingsCheckBox.DataBindings.Add("Checked", UpgradePrefsDialogModelBindingSource,
            "ImportPreviousSettigns", true, DataSourceUpdateMode.OnPropertyChanged);
        ImportSettingsCheckBox.Name = "ImportSettingsCheckBox";
        ToolTip1.SetToolTip(ImportSettingsCheckBox, resources.GetString("ImportSettingsCheckBox.ToolTip"));
        ImportSettingsCheckBox.UseVisualStyleBackColor = true;
        //
        // DeleteSettingsCheckBox
        //
        resources.ApplyResources(DeleteSettingsCheckBox, "DeleteSettingsCheckBox");
        DeleteSettingsCheckBox.DataBindings.Add("Checked", UpgradePrefsDialogModelBindingSource,
            "DeletePreviousSettings", true, DataSourceUpdateMode.OnPropertyChanged);
        DeleteSettingsCheckBox.Name = "DeleteSettingsCheckBox";
        ToolTip1.SetToolTip(DeleteSettingsCheckBox, resources.GetString("DeleteSettingsCheckBox.ToolTip"));
        DeleteSettingsCheckBox.UseVisualStyleBackColor = true;
        //
        // PrevSettngsGroupBox
        //
        resources.ApplyResources(PrevSettngsGroupBox, "PrevSettngsGroupBox");
        PrevSettngsGroupBox.Controls.Add(ImportSettingsCheckBox);
        PrevSettngsGroupBox.Controls.Add(BackupSettingsCheckbox);
        PrevSettngsGroupBox.Controls.Add(DeleteSettingsCheckBox);
        PrevSettngsGroupBox.Name = "PrevSettngsGroupBox";
        PrevSettngsGroupBox.TabStop = false;
        //
        // BackupSettingsCheckbox
        //
        resources.ApplyResources(BackupSettingsCheckbox, "BackupSettingsCheckbox");
        BackupSettingsCheckbox.DataBindings.Add("Checked", UpgradePrefsDialogModelBindingSource,
            "BackupPreviousSettings", true, DataSourceUpdateMode.OnPropertyChanged);
        BackupSettingsCheckbox.Name = "BackupSettingsCheckbox";
        ToolTip1.SetToolTip(BackupSettingsCheckbox, resources.GetString("BackupSettingsCheckbox.ToolTip"));
        BackupSettingsCheckbox.UseVisualStyleBackColor = true;
        //
        // buttonPanel
        //
        buttonPanel.Controls.Add(Cancel_Button);
        buttonPanel.Controls.Add(OK_Button);
        resources.ApplyResources(buttonPanel, "buttonPanel");
        buttonPanel.Name = "buttonPanel";
        //
        // UpgradeProgressBar
        //
        resources.ApplyResources(UpgradeProgressBar, "UpgradeProgressBar");
        UpgradeProgressBar.DataBindings.Add("Value", UpgradePrefsDialogModelBindingSource, "ProgressPercent", true);
        UpgradeProgressBar.Name = "UpgradeProgressBar";
        //
        // TopContentPanel
        //
        TopContentPanel.Controls.Add(PictureBox1);
        TopContentPanel.Controls.Add(IntroMessage);
        resources.ApplyResources(TopContentPanel, "TopContentPanel");
        TopContentPanel.Name = "TopContentPanel";
        //
        // UpgradePrefsDialog
        //
        AcceptButton = OK_Button;
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = Cancel_Button;
        ControlBox = false;
        Controls.Add(TopContentPanel);
        Controls.Add(PrevSettngsGroupBox);
        Controls.Add(buttonPanel);
        Controls.Add(UpgradeProgressBar);
        DataBindings.Add("Enabled", UpgradePrefsDialogModelBindingSource, "FormEnabled", true,
            DataSourceUpdateMode.OnPropertyChanged);
        DataBindings.Add("Icon", UpgradePrefsDialogModelBindingSource, "Icon", true,
            DataSourceUpdateMode.OnPropertyChanged);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "UpgradePrefsDialog";
        SizeGripStyle = SizeGripStyle.Hide;
        ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
        ((System.ComponentModel.ISupportInitialize)UpgradePrefsDialogModelBindingSource).EndInit();
        PrevSettngsGroupBox.ResumeLayout(false);
        PrevSettngsGroupBox.PerformLayout();
        buttonPanel.ResumeLayout(false);
        TopContentPanel.ResumeLayout(false);
        TopContentPanel.PerformLayout();
        ResumeLayout(false);
    }

    internal Button OK_Button;
    internal Button Cancel_Button;
    internal PictureBox PictureBox1;
    internal Label IntroMessage;
    internal CheckBox ImportSettingsCheckBox;
    internal CheckBox DeleteSettingsCheckBox;
    internal GroupBox PrevSettngsGroupBox;
    internal Panel buttonPanel;
    internal ProgressBar UpgradeProgressBar;
    internal Panel TopContentPanel;
    internal ToolTip ToolTip1;
    internal BindingSource UpgradePrefsDialogModelBindingSource;
    internal CheckBox BackupSettingsCheckbox;
}
