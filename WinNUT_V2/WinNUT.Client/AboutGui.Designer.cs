#nullable enable
using WinNUT_Client.Properties;

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
        LkLbl_Github = new LinkLabel();
        Lbl_Github = new Label();
        Lbl_Sf = new Label();
        Lbl_Copyright_2019 = new Label();
        Lbl_Copyright_2006 = new Label();
        Lbl_ProgNameVersion = new Label();
        PictureBox1 = new PictureBox();
        Btn_OK = new Button();
        Label_License = new Label();
        GBox.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
        SuspendLayout();
        //
        // GBox
        //
        GBox.Controls.Add(Label_License);
        GBox.Controls.Add(LkLbl_Github);
        GBox.Controls.Add(Lbl_Github);
        GBox.Controls.Add(Lbl_Sf);
        GBox.Controls.Add(Lbl_Copyright_2019);
        GBox.Controls.Add(Lbl_Copyright_2006);
        GBox.Controls.Add(Lbl_ProgNameVersion);
        GBox.Controls.Add(PictureBox1);
        GBox.Location = new Point(4, 4);
        GBox.Name = "GBox";
        GBox.Size = new Size(360, 364);
        GBox.TabIndex = 0;
        GBox.TabStop = false;
        //
        // LkLbl_Github
        //
        LkLbl_Github.AutoSize = true;
        LkLbl_Github.Location = new Point(8, 167);
        LkLbl_Github.Name = "LkLbl_Github";
        LkLbl_Github.Size = new Size(215, 13);
        LkLbl_Github.TabIndex = 6;
        LkLbl_Github.TabStop = true;
        LkLbl_Github.Text = "https://github.com/nutdotnet/WinNUT-Client";
        //
        // Lbl_Github
        //
        Lbl_Github.AutoSize = true;
        Lbl_Github.Location = new Point(8, 154);
        Lbl_Github.Name = "Lbl_Github";
        Lbl_Github.Size = new Size(146, 13);
        Lbl_Github.TabIndex = 5;
        Lbl_Github.Text = "Source Available from GitHub";
        //
        // Lbl_Sf
        //
        Lbl_Sf.AutoSize = true;
        Lbl_Sf.Location = new Point(8, 119);
        Lbl_Sf.Name = "Lbl_Sf";
        Lbl_Sf.Size = new Size(220, 26);
        Lbl_Sf.TabIndex = 4;
        Lbl_Sf.Text = "Based from Winnut Sf\r\nhttps://sourceforge.net/projects/winnutclient";
        //
        // Lbl_Copyright_2019
        //
        Lbl_Copyright_2019.AutoSize = true;
        Lbl_Copyright_2019.Location = new Point(122, 90);
        Lbl_Copyright_2019.Name = "Lbl_Copyright_2019";
        Lbl_Copyright_2019.Size = new Size(179, 26);
        Lbl_Copyright_2019.TabIndex = 3;
        Lbl_Copyright_2019.Text = "Copyright Gawindx (Decaux Nicolas)\r\n2019-2020";
        //
        // Lbl_Copyright_2006
        //
        Lbl_Copyright_2006.AutoSize = true;
        Lbl_Copyright_2006.Location = new Point(122, 53);
        Lbl_Copyright_2006.Name = "Lbl_Copyright_2006";
        Lbl_Copyright_2006.Size = new Size(137, 26);
        Lbl_Copyright_2006.TabIndex = 2;
        Lbl_Copyright_2006.Text = "Copyright Michael Liberman\r\n©  2006-2007";
        //
        // Lbl_ProgNameVersion
        //
        Lbl_ProgNameVersion.AutoSize = true;
        Lbl_ProgNameVersion.Location = new Point(119, 20);
        Lbl_ProgNameVersion.Name = "Lbl_ProgNameVersion";
        Lbl_ProgNameVersion.Size = new Size(184, 13);
        Lbl_ProgNameVersion.TabIndex = 1;
        Lbl_ProgNameVersion.Text = "WinNUT_Globals.LongProgramName";
        //
        // PictureBox1
        //
        PictureBox1.Image = Resources.ups_104x104;
        PictureBox1.Location = new Point(8, 12);
        PictureBox1.Name = "PictureBox1";
        PictureBox1.Size = new Size(104, 104);
        PictureBox1.TabIndex = 0;
        PictureBox1.TabStop = false;
        //
        // Btn_OK
        //
        Btn_OK.Location = new Point(157, 372);
        Btn_OK.Name = "Btn_OK";
        Btn_OK.Size = new Size(70, 23);
        Btn_OK.TabIndex = 1;
        Btn_OK.Text = "OK";
        Btn_OK.UseVisualStyleBackColor = true;
        //
        // Label_License
        //
        Label_License.AutoSize = true;
        Label_License.Location = new Point(8, 200);
        Label_License.Name = "Label_License";
        Label_License.Size = new Size(344, 156);
        Label_License.TabIndex = 7;
        Label_License.Text = resources.GetString("Label_License.Text") ?? "";
        //
        // AboutGui
        //
        AutoScaleDimensions = new SizeF(6F, 13F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(368, 401);
        Controls.Add(Btn_OK);
        Controls.Add(GBox);
        Icon = (Icon)resources.GetObject("$this.Icon")!;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "AboutGui";
        Text = "About";
        GBox.ResumeLayout(false);
        GBox.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
        ResumeLayout(false);
    }

    internal GroupBox GBox;
    internal PictureBox PictureBox1;
    internal Label Lbl_ProgNameVersion;
    internal Label Lbl_Copyright_2019;
    internal Label Lbl_Copyright_2006;
    internal Label Lbl_Github;
    internal Label Lbl_Sf;
    internal LinkLabel LkLbl_Github;
    internal Button Btn_OK;
    internal Label Label_License;
}
