using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;
using AGaugeClassic;

namespace WinNUT_Client.Controls;

internal sealed class UPSVarGauge : AGauge
{
    private float _value2;

    private readonly int _baseArcRadius = 45;
    private readonly int _baseArcStart = 135;
    private readonly int _baseArcSweep = 270;
    private readonly int _baseArcWidth = 5;
    private readonly int _scaleLinesMinorInnerRadius = 42;
    private readonly int _scaleLinesMinorOuterRadius = 48;
    private readonly int _scaleLinesMinorWidth = 1;
    private readonly int _scaleLinesInterInnerRadius = 40;
    private readonly int _scaleLinesInterOuterRadius = 48;
    private readonly int _scaleLinesInterWidth = 1;
    private readonly int _scaleLinesMajorInnerRadius = 40;
    private readonly int _scaleLinesMajorOuterRadius = 48;
    private readonly int _scaleLinesMajorWidth = 2;
    private readonly int _scaleNumbersRadius = 60;
    private readonly int _needleRadius = 32;
    private readonly AGaugeNeedleColor _needleColor1 = AGaugeNeedleColor.Gray;
    private readonly Color _needleColor2 = Color.DimGray;
    private readonly int _needleWidth = 2;

    private GradientTypeEnum _gradientType = GradientTypeEnum.RedGreen;
    private GradientOrientationEnum _gradientOrientation = GradientOrientationEnum.BottomToTop;
    private UnitValueEnum _unitvalue1 = UnitValueEnum.Volts;
    private UnitValueEnum _unitvalue2 = UnitValueEnum.None;

    [Browsable(true), Category("AGauge"), Description("First value to display.")]
    public float Value1
    {
        get => Value;
        set => Value = value;
    }

    [Browsable(true), Category("AGauge"), Description("Second value to display.")]
    public float Value2
    {
        get => _value2;
        set
        {
            if (Math.Abs(_value2 - value) > float.Epsilon)
            {
                _value2 = value;
                OnValueChanged(this, EventArgs.Empty);
                Refresh();
            }
        }
    }

    [Browsable(true), Category("AGauge"), Description("UseColor For Arc Base Color.")]
    public GradientTypeEnum GradientType
    {
        get => _gradientType;
        set => _gradientType = value;
    }

    [Browsable(true), Category("AGauge"), Description("Orientation Of Gradient Colors.")]
    public GradientOrientationEnum GradientOrientation
    {
        get => _gradientOrientation;
        set
        {
            if (_gradientOrientation != value)
            {
                _gradientOrientation = value;
                Refresh();
            }
        }
    }

    [Browsable(true), Category("AGauge"), Description("Units For Value 1")]
    public UnitValueEnum UnitValue1
    {
        get => _unitvalue1;
        set
        {
            if (_unitvalue1 != value)
            {
                _unitvalue1 = value;
                Refresh();
            }
        }
    }

    [Browsable(true), Category("AGauge"), Description("UseColor For Arc Base Color.")]
    public UnitValueEnum UnitValue2
    {
        get => _unitvalue2;
        set
        {
            if (_unitvalue2 != value)
            {
                _unitvalue2 = value;
                Refresh();
            }
        }
    }

    public enum GradientTypeEnum
    {
        None,
        RedGreen
    }

    public enum GradientOrientationEnum
    {
        TopToBottom,
        BottomToTop,
        RightToLeft,
        LeftToRight
    }

    public enum UnitValueEnum
    {
        None,
        Hertz,
        Percent,
        Volts,
        Watts
    }

    public UPSVarGauge()
    {
        Size = new Size(148, 130);
    }

    public override void RenderDefaultArc(Graphics graphics)
    {
        if (_baseArcRadius <= 0)
        {
            return;
        }

        var baseArcRadius = (int)(_baseArcRadius * centerFactor);

        if (_gradientType == GradientTypeEnum.None)
        {
            using var pnArc = new Pen(BaseArcColor, _baseArcWidth * centerFactor);
            graphics.DrawArc(pnArc,
                new Rectangle(Center.X - baseArcRadius, Center.Y - baseArcRadius, 2 * baseArcRadius, 2 * baseArcRadius),
                _baseArcStart, _baseArcSweep);
        }
        else
        {
            var gradientP1Brush = new Point(0, Center.X + baseArcRadius + _baseArcWidth + 2);
            var gradientP2Brush = new Point(0, Center.X - baseArcRadius - _baseArcWidth - 2);

            switch (_gradientOrientation)
            {
                case GradientOrientationEnum.TopToBottom:
                    gradientP1Brush = new Point(0, Center.Y - baseArcRadius - _baseArcWidth - 2);
                    gradientP2Brush = new Point(0, Center.Y + baseArcRadius + _baseArcWidth + 2);
                    break;
                case GradientOrientationEnum.BottomToTop:
                    gradientP1Brush = new Point(0, Center.Y + baseArcRadius + _baseArcWidth + 2);
                    gradientP2Brush = new Point(0, Center.Y - baseArcRadius - _baseArcWidth - 2);
                    break;
                case GradientOrientationEnum.RightToLeft:
                    gradientP1Brush = new Point(Center.X + baseArcRadius + _baseArcWidth + 2, 0);
                    gradientP2Brush = new Point(Center.X - baseArcRadius - _baseArcWidth - 2, 0);
                    break;
                case GradientOrientationEnum.LeftToRight:
                    gradientP1Brush = new Point(Center.X - baseArcRadius - _baseArcWidth - 2, 0);
                    gradientP2Brush = new Point(Center.X + baseArcRadius + _baseArcWidth + 2, 0);
                    break;
            }

            using var myArc1Gradient = new LinearGradientBrush(gradientP1Brush, gradientP2Brush, Color.Red, Color.Green);
            using var pnArc = new Pen(myArc1Gradient, _baseArcWidth * centerFactor);
            graphics.DrawArc(pnArc,
                new Rectangle(Center.X - baseArcRadius, Center.Y - baseArcRadius, 2 * baseArcRadius, 2 * baseArcRadius),
                _baseArcStart, _baseArcSweep);
        }
    }

    public override void PostRender(Graphics graphics)
    {
        using var stringPen = new SolidBrush(Color.Black);
        using var penFontV1 = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
        using var penFontV2 = new Font("Microsoft Sans Serif", 7, FontStyle.Bold);
        const int lineHeight = 15;
        var strPos = Center;
        strPos.Y += 5;

        if (UnitValue1 != UnitValueEnum.None)
        {
            var stringToDraw = ApplyUnit(Value1.ToString(CultureInfo.CurrentCulture), UnitValue1);
            var stringSize = TextRenderer.MeasureText(stringToDraw, penFontV1);
            strPos.Y += lineHeight;
            graphics.DrawString(stringToDraw, penFontV1, stringPen,
                (strPos.X - stringSize.Width / 2f + 5), strPos.Y);
        }

        if (UnitValue2 != UnitValueEnum.None)
        {
            var stringToDraw = ApplyUnit(_value2.ToString(CultureInfo.CurrentCulture), UnitValue2);
            var stringSize = TextRenderer.MeasureText(stringToDraw, penFontV2);
            strPos.Y += lineHeight;
            graphics.DrawString(stringToDraw, penFontV2, stringPen,
                (strPos.X - stringSize.Width / 2f + 7), strPos.Y);
        }
    }

    private static string ApplyUnit(string value, UnitValueEnum unit)
    {
        return unit switch
        {
            UnitValueEnum.Hertz => value + " Hz",
            UnitValueEnum.Percent => value + " %",
            UnitValueEnum.Volts => value + " V",
            UnitValueEnum.Watts => value + " W",
            _ => value
        };
    }
}
