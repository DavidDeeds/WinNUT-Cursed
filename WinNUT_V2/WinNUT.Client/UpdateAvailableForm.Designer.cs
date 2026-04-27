#nullable enable
using WinNUT_Client.Controls;

namespace WinNUT_Client;

partial class UpdateAvailableForm
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateAvailableForm));
        GB1 = new GroupBox();
        Title = new Label();
        TB_ChgLog = new TextBox();
        Update_Btn = new Button();
        Close_Btn = new Button();
        ButtonsPanel = new Panel();
        VisitPageButton = new Button();
        DownloadProgressPanel = new Panel();
        DownloadProgressPanelLabel = new Label();
        DownloadProgressBar = new CProgressBar();
        GB1.SuspendLayout();
        ButtonsPanel.SuspendLayout();
        DownloadProgressPanel.SuspendLayout();
        SuspendLayout();
        //
        // GB1
        //
        resources.ApplyResources(GB1, "GB1");
        GB1.Controls.Add(Title);
        GB1.Controls.Add(TB_ChgLog);
        GB1.Name = "GB1";
        GB1.TabStop = false;
        //
        // Title
        //
        resources.ApplyResources(Title, "Title");
        Title.Name = "Title";
        //
        // TB_ChgLog
        //
        resources.ApplyResources(TB_ChgLog, "TB_ChgLog");
        TB_ChgLog.Name = "TB_ChgLog";
        //
        // Update_Btn
        //
        resources.ApplyResources(Update_Btn, "Update_Btn");
        Update_Btn.Name = "Update_Btn";
        Update_Btn.UseVisualStyleBackColor = true;
        //
        // Close_Btn
        //
        resources.ApplyResources(Close_Btn, "Close_Btn");
        Close_Btn.Name = "Close_Btn";
        Close_Btn.UseVisualStyleBackColor = true;
        //
        // ButtonsPanel
        //
        resources.ApplyResources(ButtonsPanel, "ButtonsPanel");
        ButtonsPanel.Controls.Add(VisitPageButton);
        ButtonsPanel.Controls.Add(Update_Btn);
        ButtonsPanel.Controls.Add(Close_Btn);
        ButtonsPanel.Name = "ButtonsPanel";
        //
        // VisitPageButton
        //
        resources.ApplyResources(VisitPageButton, "VisitPageButton");
        VisitPageButton.Name = "VisitPageButton";
        VisitPageButton.UseVisualStyleBackColor = true;
        //
        // DownloadProgressPanel
        //
        resources.ApplyResources(DownloadProgressPanel, "DownloadProgressPanel");
        DownloadProgressPanel.Controls.Add(DownloadProgressPanelLabel);
        DownloadProgressPanel.Controls.Add(DownloadProgressBar);
        DownloadProgressPanel.Name = "DownloadProgressPanel";
        //
        // DownloadProgressPanelLabel
        //
        resources.ApplyResources(DownloadProgressPanelLabel, "DownloadProgressPanelLabel");
        DownloadProgressPanelLabel.Name = "DownloadProgressPanelLabel";
        //
        // DownloadProgressBar
        //
        DownloadProgressBar.ForeColor = SystemColors.HighlightText;
        resources.ApplyResources(DownloadProgressBar, "DownloadProgressBar");
        DownloadProgressBar.Name = "DownloadProgressBar";
        //
        // UpdateAvailableForm
        //
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(GB1);
        Controls.Add(DownloadProgressPanel);
        Controls.Add(ButtonsPanel);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "UpdateAvailableForm";
        GB1.ResumeLayout(false);
        GB1.PerformLayout();
        ButtonsPanel.ResumeLayout(false);
        DownloadProgressPanel.ResumeLayout(false);
        DownloadProgressPanel.PerformLayout();
        ResumeLayout(false);
    }

    internal GroupBox GB1;
    internal Button Close_Btn;
    internal Button Update_Btn;
    internal TextBox TB_ChgLog;
    internal Label Title;
    internal Panel ButtonsPanel;
    internal Panel DownloadProgressPanel;
    internal Label DownloadProgressPanelLabel;
    internal Button VisitPageButton;
    internal CProgressBar DownloadProgressBar;
}
