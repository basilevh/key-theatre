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

        private static byte limit(int value)
            => (value < 0 ? (byte)0 : value >= 256 ? (byte)255 : (byte)value);

        // hue: 0-360, saturation: 0-1, brightness: 0-1
        private static void HsbToRgb(float hue, float sat, float bri,
            out int red, out int green, out int blue)
        {
            float hf = ((hue + 360f) % 360f) / 60f; // [0,6)
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
            else // hue undefined
            { r = 0f; g = 0f; b = 0f; }

            // Compensate for brightness and return converted values
            float add = bri - chroma;
            red = (int)(256 * (r + add));
            green = (int)(256 * (g + add));
            blue = (int)(256 * (b + add));
        }

        public static void RgbToHsb(int red, int green, int blue,
            out float hue, out float sat, out float bri)
        {
            // Color's conversion methods are HSL, not HSB/HSV!
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

        public static Color GetFromRGB(int red, int green, int blue)
            => Color.FromArgb(limit(red), limit(green), limit(blue));

        public static Color GetFromHSB(float hue, float sat, float bri)
        {
            int red, green, blue;
            HsbToRgb(hue, sat, bri, out red, out green, out blue);
            return GetFromRGB(red, green, blue);
        }

        public static Color GetRandom()
            => Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

        // Fixed RGB assignment
        public static Color WithRed(this Color clr, int red)
            => Color.FromArgb(limit(red), clr.G, clr.B);

        public static Color WithGreen(this Color clr, int green)
            => Color.FromArgb(clr.R, limit(green), clr.B);

        public static Color WithBlue(this Color clr, int blue)
            => Color.FromArgb(clr.R, clr.G, limit(blue));

        // Dependant RGB assignment
        public static Color WithRed(this Color clr, Func<int, int> redProj)
            => Color.FromArgb(limit(redProj(clr.R)), clr.G, clr.B);

        public static Color WithGreen(this Color clr, Func<int, int> greenProj)
            => Color.FromArgb(clr.R, limit(greenProj(clr.G)), clr.B);

        public static Color WithBlue(this Color clr, Func<int, int> blueProj)
            => Color.FromArgb(clr.R, clr.G, limit(blueProj(clr.B)));

        public static Color WithRGB(this Color clr, Func<int, int> redProj, Func<int, int> greenProj, Func<int, int> blueProj)
            => Color.FromArgb(limit(redProj(clr.R)), limit(greenProj(clr.G)), limit(blueProj(clr.B)));

        // Fixed HSB assignment
        public static Color WithHue(this Color clr, float hue) // 0-360
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(hue, oldSat, oldBri);
        }

        public static Color WithSaturation(this Color clr, float sat) // 0-360
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(oldHue, sat, oldBri);
        }

        public static Color WithBrightness(this Color clr, float bri) // 0-360
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(oldHue, oldSat, bri);
        }

        // Dependant HSB assignment
        public static Color WithHue(this Color clr, Func<float, float> hueProj) // 0-360
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(hueProj(oldHue), oldSat, oldBri);
        }

        public static Color WithSaturation(this Color clr, Func<float, float> satProj) // 0-1
        {
            float oldHue, oldSat, oldBri;
            RgbToHsb(clr.R, clr.G, clr.B, out oldHue, out oldSat, out oldBri);
            return GetFromHSB(oldHue, satProj(oldSat), oldBri);
        }

        public static Color WithBrightness(this Color clr, Func<float, float> briProj) // 0-1
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
