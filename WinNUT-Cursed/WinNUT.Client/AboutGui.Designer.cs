#nullable enable
namespace WinNUT_Client;

partial class AboutGui
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutGui));
        GBox = new GroupBox();
        Lbl_Preamble = new Label();
        LkLbl_Github = new LinkLabel();
        Lbl_Github = new Label();
        Lbl_Copyright_2019 = new Label();
        Lbl_Copyright_2006 = new Label();
        Lbl_ProgNameVersion = new Label();
        Btn_OK = new Button();
        Label_License = new Label();
        GBox.SuspendLayout();
        SuspendLayout();
        //
        // GBox
        //
        GBox.Controls.Add(Lbl_Preamble);
        GBox.Controls.Add(Label_License);
        GBox.Controls.Add(LkLbl_Github);
        GBox.Controls.Add(Lbl_Github);
        GBox.Controls.Add(Lbl_Copyright_2019);
        GBox.Controls.Add(Lbl_Copyright_2006);
        GBox.Controls.Add(Lbl_ProgNameVersion);
        GBox.Location = new Point(4, 4);
        GBox.Name = "GBox";
        GBox.Size = new Size(452, 392);
        GBox.TabIndex = 0;
        GBox.TabStop = false;
        //
        // Lbl_Preamble
        //
        Lbl_Preamble.Location = new Point(8, 54);
        Lbl_Preamble.Name = "Lbl_Preamble";
        Lbl_Preamble.Size = new Size(436, 42);
        Lbl_Preamble.TabIndex = 2;
        Lbl_Preamble.Text = "WinNUT-Cursed preamble";
        //
        // LkLbl_Github
        //
        LkLbl_Github.AutoSize = true;
        LkLbl_Github.Location = new Point(8, 197);
        LkLbl_Github.Name = "LkLbl_Github";
        LkLbl_Github.Size = new Size(250, 13);
        LkLbl_Github.TabIndex = 6;
        LkLbl_Github.TabStop = true;
        LkLbl_Github.Text = "https://github.com/DavidDeeds/WinNUT-Cursed";
        //
        // Lbl_Github
        //
        Lbl_Github.AutoSize = true;
        Lbl_Github.Location = new Point(8, 181);
        Lbl_Github.Name = "Lbl_Github";
        Lbl_Github.Size = new Size(96, 13);
        Lbl_Github.TabIndex = 5;
        Lbl_Github.Text = "Project repository";
        //
        // Lbl_Copyright_2019
        //
        Lbl_Copyright_2019.Location = new Point(8, 136);
        Lbl_Copyright_2019.Name = "Lbl_Copyright_2019";
        Lbl_Copyright_2019.Size = new Size(436, 36);
        Lbl_Copyright_2019.TabIndex = 4;
        Lbl_Copyright_2019.Text = "Modern WinNUT lineage";
        //
        // Lbl_Copyright_2006
        //
        Lbl_Copyright_2006.Location = new Point(8, 99);
        Lbl_Copyright_2006.Name = "Lbl_Copyright_2006";
        Lbl_Copyright_2006.Size = new Size(436, 36);
        Lbl_Copyright_2006.TabIndex = 3;
        Lbl_Copyright_2006.Text = "Original WinNUT lineage";
        //
        // Lbl_ProgNameVersion
        //
        Lbl_ProgNameVersion.AutoSize = true;
        Lbl_ProgNameVersion.Location = new Point(8, 20);
        Lbl_ProgNameVersion.Name = "Lbl_ProgNameVersion";
        Lbl_ProgNameVersion.Size = new Size(184, 13);
        Lbl_ProgNameVersion.TabIndex = 1;
        Lbl_ProgNameVersion.Text = "WinNUT_Globals.LongProgramName";
        //
        // Btn_OK
        //
        Btn_OK.Location = new Point(195, 404);
        Btn_OK.Name = "Btn_OK";
        Btn_OK.Size = new Size(70, 23);
        Btn_OK.TabIndex = 1;
        Btn_OK.Text = "OK";
        Btn_OK.UseVisualStyleBackColor = true;
        //
        // Label_License
        //
        Label_License.Location = new Point(8, 225);
        Label_License.Name = "Label_License";
        Label_License.Size = new Size(436, 156);
        Label_License.TabIndex = 7;
        Label_License.Text = resources.GetString("Label_License.Text") ?? "";
        //
        // AboutGui
        //
        AutoScaleDimensions = new SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(460, 433);
        Controls.Add(Btn_OK);
        Controls.Add(GBox);
        Icon = (Icon)resources.GetObject("$this.Icon")!;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutGui";
        Text = "About";
        GBox.ResumeLayout(false);
        GBox.PerformLayout();
        ResumeLayout(false);
    }

    internal GroupBox GBox;
    internal Label Lbl_ProgNameVersion;
    internal Label Lbl_Copyright_2019;
    internal Label Lbl_Copyright_2006;
    internal Label Lbl_Github;
    internal Label Lbl_Preamble;
    internal LinkLabel LkLbl_Github;
    internal Button Btn_OK;
    internal Label Label_License;
}
