#nullable enable
using WinNUT_Client.Properties;

namespace WinNUT_Client;

partial class ListVarGui
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(ListVarGui));
        TView_UPSVar = new TreeView();
        GB1 = new GroupBox();
        Lbl_D_Value = new Label();
        Lbl_V_Value = new Label();
        Lbl_N_Value = new Label();
        Lbl_D = new Label();
        Lbl_V = new Label();
        Lbl_Name = new Label();
        Btn_Reload = new Button();
        Btn_Close = new Button();
        Timer_Update_List = new System.Windows.Forms.Timer(components);
        Btn_Clip = new Button();
        ToolTip = new ToolTip(components);
        Btn_Save = new Button();
        GB1.SuspendLayout();
        SuspendLayout();
        //
        // TView_UPSVar
        //
        resources.ApplyResources(TView_UPSVar, "TView_UPSVar");
        TView_UPSVar.Name = "TView_UPSVar";
        TView_UPSVar.PathSeparator = ".";
        //
        // GB1
        //
        GB1.Controls.Add(Lbl_D_Value);
        GB1.Controls.Add(Lbl_V_Value);
        GB1.Controls.Add(Lbl_N_Value);
        GB1.Controls.Add(Lbl_D);
        GB1.Controls.Add(Lbl_V);
        GB1.Controls.Add(Lbl_Name);
        resources.ApplyResources(GB1, "GB1");
        GB1.Name = "GB1";
        GB1.TabStop = false;
        //
        // Lbl_D_Value
        //
        resources.ApplyResources(Lbl_D_Value, "Lbl_D_Value");
        Lbl_D_Value.BorderStyle = BorderStyle.Fixed3D;
        Lbl_D_Value.Name = "Lbl_D_Value";
        //
        // Lbl_V_Value
        //
        resources.ApplyResources(Lbl_V_Value, "Lbl_V_Value");
        Lbl_V_Value.BorderStyle = BorderStyle.Fixed3D;
        Lbl_V_Value.Name = "Lbl_V_Value";
        //
        // Lbl_N_Value
        //
        resources.ApplyResources(Lbl_N_Value, "Lbl_N_Value");
        Lbl_N_Value.BorderStyle = BorderStyle.Fixed3D;
        Lbl_N_Value.ForeColor = SystemColors.ControlText;
        Lbl_N_Value.Name = "Lbl_N_Value";
        //
        // Lbl_D
        //
        resources.ApplyResources(Lbl_D, "Lbl_D");
        Lbl_D.Name = "Lbl_D";
        //
        // Lbl_V
        //
        resources.ApplyResources(Lbl_V, "Lbl_V");
        Lbl_V.Name = "Lbl_V";
        //
        // Lbl_Name
        //
        resources.ApplyResources(Lbl_Name, "Lbl_Name");
        Lbl_Name.Name = "Lbl_Name";
        //
        // Btn_Reload
        //
        resources.ApplyResources(Btn_Reload, "Btn_Reload");
        Btn_Reload.Name = "Btn_Reload";
        ToolTip.SetToolTip(Btn_Reload, resources.GetString("Btn_Reload.ToolTip"));
        Btn_Reload.UseVisualStyleBackColor = true;
        //
        // Btn_Close
        //
        resources.ApplyResources(Btn_Close, "Btn_Close");
        Btn_Close.Name = "Btn_Close";
        ToolTip.SetToolTip(Btn_Close, resources.GetString("Btn_Close.ToolTip"));
        Btn_Close.UseVisualStyleBackColor = true;
        //
        // Timer_Update_List
        //
        Timer_Update_List.Interval = 1000;
        //
        // Btn_Clip
        //
        Btn_Clip.Image = Resources.CopyHS;
        resources.ApplyResources(Btn_Clip, "Btn_Clip");
        Btn_Clip.Name = "Btn_Clip";
        ToolTip.SetToolTip(Btn_Clip, resources.GetString("Btn_Clip.ToolTip"));
        Btn_Clip.UseVisualStyleBackColor = true;
        //
        // Btn_Save
        //
        Btn_Save.Image = Resources.saveHS;
        resources.ApplyResources(Btn_Save, "Btn_Save");
        Btn_Save.Name = "Btn_Save";
        ToolTip.SetToolTip(Btn_Save, resources.GetString("Btn_Save.ToolTip"));
        Btn_Save.UseVisualStyleBackColor = true;
        //
        // ListVarGui
        //
        resources.ApplyResources(this, "$this");
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(Btn_Save);
        Controls.Add(Btn_Clip);
        Controls.Add(Btn_Close);
        Controls.Add(Btn_Reload);
        Controls.Add(GB1);
        Controls.Add(TView_UPSVar);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ListVarGui";
        GB1.ResumeLayout(false);
        GB1.PerformLayout();
        ResumeLayout(false);
    }

    internal TreeView TView_UPSVar;
    internal GroupBox GB1;
    internal Label Lbl_D_Value;
    internal Label Lbl_V_Value;
    internal Label Lbl_N_Value;
    internal Label Lbl_D;
    internal Label Lbl_V;
    internal Label Lbl_Name;
    internal Button Btn_Reload;
    internal Button Btn_Close;
    internal System.Windows.Forms.Timer Timer_Update_List;
    internal Button Btn_Clip;
    internal ToolTip ToolTip;
    internal Button Btn_Save;
}
