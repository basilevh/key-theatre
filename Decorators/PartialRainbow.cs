// Started 27-04-2018, Basile Van Hoorick

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeyDecorator.Decorators
{
    /// <summary>
    /// Decorator that display a calming, slowly waving red-orange-magenta gradient.
    /// </summary>
    public class PartialRainbow : Decorator
    {
        private const double TWO_PI = 2.0 * Math.PI;

        public PartialRainbow(float hueCenter, float hueRange)
            : base(25, false, Color.Gray)
        {
            this.hueCenter = hueCenter;
            this.hueRange = hueRange;
            ledCont.StopEffects(); // otherwise pulsing keys would prevent bitmap from working
        }

        private readonly float hueCenter;
        private readonly float hueRange;

        // No keylogger is active, so these methods are never called
        protected override void OnKeyDown(MyKey key) { }
        protected override void OnKeyUp(MyKey key) { }

        protected override void Tick(long totalMs, long deltaMs)
        {
            long arg = totalMs / 8;

            // TODO: document
            float hueOffset = (float)Math.Sin(arg * TWO_PI / 7890.0) * 3f;
            float huePeriod = 18f + (float) Math.Sin(arg * TWO_PI / 8901.0) * 6f;
            float hueAmpl = (float)Math.Sin(arg * TWO_PI / 5678.0) / 3f;
            float hueStep = (float)Math.Sin(arg * TWO_PI / 6789.0) / 3f;

            fillWavyRows(hueOffset, huePeriod, hueAmpl, hueStep);
        }

        private void fillWavyRows(float offset, float period, float ampl, float step)
        {
            Color[,] bitmap = new Color[KeyboardInfo.BmpWidth, KeyboardInfo.BmpHeight];

            // Fill in rows of hue-shifting keys
            for (int j = 0; j < KeyboardInfo.BmpHeight; j++)
            {
                float startPhase = (float)Math.Sin(j) * ampl + offset + j * step;
                for (int i = 0; i < KeyboardInfo.BmpWidth; i++)
                {
                    float phase = startPhase + i * (float)TWO_PI / period;
                    float hue = hueCenter + (float)Math.Sin(phase) * hueRange / 2f;
                    bitmap[i, j] = ColorUtil.GetFromHSB(hue, 1f, 1f);
                }
            }

            // Apply bitmap
            ledCont.SetBitmap(bitmap);
        }
    }
}
