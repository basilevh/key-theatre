// Started 15-11-2016, Basile Van Hoorick

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace KeyDecorator.Decorators
{
    /// <summary>
    /// Decorator that continuously and independently fades each key between two colours.
    /// Does not require a keylogger.
    /// </summary>
    public class FullPulse : Decorator
    {
        public FullPulse(float bri = 1f, float sat = 1f, int fadeMs = 4000)
            : base(25, false, Color.Gray)
        {
            foreach (MyKey key in Enum.GetValues(typeof(MyKey)))
            {
                Color clr1 = ColorUtil.GetFromHSB(random.Next(360), sat, bri);
                Color clr2 = ColorUtil.GetFromHSB(random.Next(360), sat, bri);
                // Color clr2 = clr1.WithHue(h => h + 90f);

                ledCont.PulseKey(key, clr1, clr2, fadeMs, true);
            }
        }

        // No keylogger is active, so these methods are never called
        protected override void OnKeyDown(MyKey key) { }
        protected override void OnKeyUp(MyKey key) { }

        private long totalDelta = 0;

        protected override void Tick(long totalMs, long deltaMs)
        {
            // TODO why necessary?
            totalDelta += deltaMs;
            if (totalDelta >= 250)
            {
                // Run once
                totalDelta = long.MinValue;
            }
        }
    }
}
