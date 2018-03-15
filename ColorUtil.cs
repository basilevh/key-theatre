// Started 15-11-2016, Basile Van Hoorick
// Helper methods for color manipulation
// All RGB values are automatically saturated to 0 or 255

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyDecorator
{
    public static class ColorUtil
    {
        private readonly static Random rnd = new Random();

        private static byte saturate(int value)
        {
            return (value < 0 ? (byte)0 : value >= 256 ? (byte)255 : (byte)value);
        }

        /// <summary>
        /// Converts an HSB representation of a colour to its corresponding RGB representation.
        /// </summary>
        /// <param name="hue">Input hue value (0-360)</param>
        /// <param name="sat">Input saturation value (0-1)</param>
        /// <param name="bri">Input brightness value (0-1)</param>
        /// <param name="red">Output red value (0-255)</param>
        /// <param name="green">Output green value (0-255)</param>
        /// <param name="blue">Output blue value (0-255)</param>
        public static void HsbToRgb(float hue, float sat, float bri,
            out int red, out int green, out int blue)
        {
            float hf = (((hue % 360f) + 360f) % 360f) / 60f; // in [0,6)
            float chroma = sat * bri;
            float sloped = chroma * (1f - Math.Abs(hf % 2f - 1f));

            // Get colour component of RGB in [0,1)
            // If saturation OR brightness is zero, these values will also be zero
            float r, g, b;
            if (0f <= hf && hf <= 1f)
            { r = chroma; g = sloped; b = 0f; }
            else if (1f <= hf && hf <= 2f)
            { r = sloped; g = chroma; b = 0f; }
            else if (2f <= hf && hf <= 3f)
            { r = 0f; g = chroma; b = sloped; }
            else if (3f <= hf && hf <= 4f)
            { r = 0f; g = sloped; b = chroma; }
            else if (4f <= hf && hf <= 5f)
            { r = sloped; g = 0f; b = chroma; }
            else if (5f <= hf && hf <= 6f)
            { r = chroma; g = 0f; b = sloped; }
            else // hue out of range
            { r = 0f; g = 0f; b = 0f; }

            // Compensate for brightness and return converted values
            float add = bri - chroma;
            red = saturate((int)(256 * (r + add)));
            green = saturate((int)(256 * (g + add)));
            blue = saturate((int)(256 * (b + add)));
        }

        /// <summary>
        /// Converts an RGB representation of a colour to its corresponding HSB representation.
        /// </summary>
        /// <param name="red">Input red value (0-255)</param>
        /// <param name="green">Input green value (0-255)</param>
        /// <param name="blue">Input blue value (0-255)</param>
        /// <param name="hue">Output hue value (0-360)</param>
        /// <param name="sat">Output saturation value (0-1)</param>
        /// <param name="bri">Output brightness value (0-1)</param>
        public static void RgbToHsb(int red, int green, int blue,
            out float hue, out float sat, out float bri)
        {
            // Color's built-in conversion methods are actually HSL, not HSB/HSV!
            float r = red / 256f;
            float g = green / 256f;
            float b = blue / 256f;
            float min = Math.Min(Math.Min(r, g), b);
            float max = Math.Max(Math.Max(r, g), b);
            float delta = max - min;

            // Brightness
            bri = max;

            // Saturation
            if (max <= 0f) // black
                sat = 0f;
            else
                sat = delta / max;

            // Hue
            if (delta == 0) // grayscale
                hue = 0f;
            else if (max == r)
                hue = (g - b) / delta;
            else if (max == g)
                hue = (b - r) / delta + 2f;
            else if (max == b)
                hue = (r - g) / delta + 4f;
            else // should never happen
                hue = 0f;
            hue = (hue + 6f) % 6f * 60f;
        }

        /// <summary>
        /// Creates a Color object starting from its RGB representation.
        /// </summary>
        public static Color GetFromRGB(int red, int green, int blue)
        {
            return Color.FromArgb(saturate(red), saturate(green), saturate(blue));
        }

        /// <summary>
        /// Creates a Color object starting from its HSB representation.
        /// </summary>
        public static Color GetFromHSB(float hue, float sat, float bri)
        {
            int red, green, blue;
            HsbToRgb(hue, sat, bri, out red, out green, out blue);
            return GetFromRGB(red, green, blue);
        }

        /// <summary>
        /// Creates a Color object with a uniformly random RGB representation.
        /// </summary>
        public static Color GetRandom()
        {
            return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }

        /// <summary>
        /// Creates a Color object with the same properties, but with a new fixed red value.
        /// </summary>
        public static Color WithRed(this Color clr, int red)
        {
            return Color.FromArgb(saturate(red), clr.G, clr.B);
        }

        /// <summary>
        /// Creates a Color object with the same properties, but with a new fixed green value.
        /// </summary>
        public static Color WithGreen(this Color clr, int green)
        {
            return Color.FromArgb(clr.R, saturate(green), clr.B);
        }

        /// <summary>
        /// Creates a Color object with the same properties, but with a new fixed blue value.
        /// </summary>
        public static Color WithBlue(this Color clr, int blue)
        {
            return Color.FromArgb(clr.R, clr.G, saturate(blue));
        }

        /// <summary>
        /// Creates a Color object with the same properties, but with a new red value that is a given function of the old value.
        /// </summary>
        public static Color WithRed(this Color clr, Func<int, int> redProj)
        {
            return Color.FromArgb(saturate(redProj(clr.R)), clr.G, clr.B);
        }

        /// <summary>
        /// Creates a Color object with the same properties, but with a new green value that is a given function of the old value.
        /// </summary>
        public static Color WithGreen(this Color clr, Func<int, int> greenProj)
        {
            return Color.FromArgb(clr.R, saturate(greenProj(clr.G)), clr.B);
        }

        /// <summary>
        /// Creates a Color object with the same properties, but with a new blue value that is a given function of the old value.
        /// </summary>
        public static Color WithBlue(this Color clr, Func<int, int> blueProj)
        {
            return Color.FromArgb(clr.R, clr.G, saturate(blueProj(clr.B)));
        }

        /// <summary>
        /// Creates a Color object with an RGB representation that are given functions of the old values.
        /// </summary>
        public static Color WithRGB(this Color clr, Func<int, int> redProj, Func<int, int> greenProj, Func<int, int> blueProj)
        {
            return Color.FromArgb(saturate(redProj(clr.R)), saturate(greenProj(clr.G)), saturate(blueProj(clr.B)));
        }

        // Fixed HSB assignment
        public static Color WithHue(this Color clr, float hue)
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(hue, oldSat, oldBri);
        }

        public static Color WithSaturation(this Color clr, float sat)
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(oldHue, sat, oldBri);
        }

        public static Color WithBrightness(this Color clr, float bri)
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(oldHue, oldSat, bri);
        }

        // Dependant HSB assignment
        public static Color WithHue(this Color clr, Func<float, float> hueProj)
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(hueProj(oldHue), oldSat, oldBri);
        }

        public static Color WithSaturation(this Color clr, Func<float, float> satProj)
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(oldHue, satProj(oldSat), oldBri);
        }

        public static Color WithBrightness(this Color clr, Func<float, float> briProj)
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(oldHue, oldSat, briProj(oldBri));
        }

        public static Color WithHSB(this Color clr, Func<float, float> hueProj, Func<float, float> satProj, Func<float, float> briProj)
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(hueProj(oldHue), satProj(oldSat), briProj(oldBri));
        }
    }
}
