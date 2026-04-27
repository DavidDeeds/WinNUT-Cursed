using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WinNUT_Client.Controls;

/// <summary>Progress bar that draws centered text over the bar.</summary>
public class CProgressBar : ProgressBar
{
    private const int WmPaint = 0x000F;

    protected override CreateParams CreateParams
    {
        get
        {
            var result = base.CreateParams;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major >= 6)
            {
                var vIn = new byte[] { 0, 2, 0, 0, 0, 0, 0, 0 };
                result.ExStyle |= BitConverter.ToInt32(vIn, 0); // WS_EX_COMPOSITED
            }

            return result;
        }
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);
        if (m.Msg != WmPaint)
        {
            return;
        }

        using var graphics = CreateGraphics();
        using var brush = new SolidBrush(ForeColor);
        var textSize = graphics.MeasureString(Text, Font);
        graphics.DrawString(Text, Font, brush, (Width - textSize.Width) / 2, (Height - textSize.Height) / 2);
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            Refresh();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    public override Font Font
    {
        get => base.Font;
        set
        {
            base.Font = value;
            Refresh();
        }
    }
}
