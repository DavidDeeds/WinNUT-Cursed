#nullable enable
namespace WinNUT_Client;

partial class ShutdownGui
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(ShutdownGui));
        Grace_Button = new Button();
        ShutDown_Btn = new Button();
        lbl_UPSStatus = new Label();
        Run_Timer = new System.Windows.Forms.Timer(components);
        SuspendLayout();
        //
        // Grace_Button
        //
        resources.ApplyResources(Grace_Button, "Grace_Button");
        Grace_Button.Name = "Grace_Button";
        Grace_Button.UseVisualStyleBackColor = true;
        Grace_Button.UseWaitCursor = true;
        //
        // ShutDown_Btn
        //
        resources.ApplyResources(ShutDown_Btn, "ShutDown_Btn");
        ShutDown_Btn.Name = "ShutDown_Btn";
        ShutDown_Btn.UseVisualStyleBackColor = true;
        ShutDown_Btn.UseWaitCursor = true;
        //
        // lbl_UPSStatus
        //
        resources.ApplyResources(lbl_UPSStatus, "lbl_UPSStatus");
        lbl_UPSStatus.ForeColor = SystemColors.ControlText;
        lbl_UPSStatus.Name = "lbl_UPSStatus";
        lbl_UPSStatus.UseWaitCursor = true;
        //
        // Run_Timer
        //
        Run_Timer.Enabled = true;
        Run_Timer.Interval = 1000;
        //
        // ShutdownGui
        //
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(lbl_UPSStatus);
        Controls.Add(ShutDown_Btn);
        Controls.Add(Grace_Button);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ShutdownGui";
        TopMost = true;
        UseWaitCursor = true;
        ResumeLayout(false);
        PerformLayout();
    }

    internal Button Grace_Button;
    internal Button ShutDown_Btn;
    internal Label lbl_UPSStatus;
    internal System.Windows.Forms.Timer Run_Timer;
}
