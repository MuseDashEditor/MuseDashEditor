using osu.Framework.Graphics;

namespace MuseDashEditor.Game.Utils;

public static class MdeColors
{
    private const int base_hue = 317;

    public static Colour4 TopLaneColor => Colour4.FromHex("#00defe");
    public static Colour4 BottomLaneColor => Colour4.FromHex("#fe64ee");

    // Using the same color scheme as osu!, because it works well :D
    public static Colour4 Highlight1 => getColour(1, 0.7f);

    public static Colour4 Content1 => getColour(0.4f, 1);
    public static Colour4 Content2 => getColour(0.4f, 0.9f);

    public static Colour4 Light1 => getColour(0.4f, 0.8f);
    public static Colour4 Light2 => getColour(0.4f, 0.75f);
    public static Colour4 Light3 => getColour(0.4f, 0.7f);
    public static Colour4 Light4 => getColour(0.4f, 0.5f);

    public static Colour4 Dark1 => getColour(0.2f, 0.35f);
    public static Colour4 Dark2 => getColour(0.2f, 0.3f);
    public static Colour4 Dark3 => getColour(0.2f, 0.25f);
    public static Colour4 Dark4 => getColour(0.2f, 0.2f);
    public static Colour4 Dark5 => getColour(0.2f, 0.15f);
    public static Colour4 Dark6 => getColour(0.2f, 0.1f);

    public static Colour4 Foreground1 => getColour(0.1f, 0.6f);

    public static Colour4 Background1 => getColour(0.1f, 0.4f);
    public static Colour4 Background2 => getColour(0.1f, 0.3f);
    public static Colour4 Background3 => getColour(0.1f, 0.25f);
    public static Colour4 Background4 => getColour(0.1f, 0.2f);
    public static Colour4 Background5 => getColour(0.1f, 0.15f);
    public static Colour4 Background6 => getColour(0.1f, 0.1f);

    private static Colour4 getColour(float saturation, float lightness) =>
        Colour4.FromHSL(base_hue / 360f, saturation, lightness);
}
