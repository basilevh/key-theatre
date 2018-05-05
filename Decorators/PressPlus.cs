// Started 08-11-2016, Basile Van Hoorick
// Shows horizontal projectiles from keypresses

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyDecorator.Decorators
{
    public class PressPlus : Decorator
    {
        public enum Mode
        {
            Horizontal, Radial
        }

        /// <param name="backClr">The default background color for unpressed keys.</param>
        /// <param name="mode">The path of divergence from pressed keys.</param>
        /// <param name="distance">If the mode is Horizontal: maximum projectile distance, if Radial: radius (0 for infinite)</param>
        public PressPlus(Color backClr, Mode mode = Mode.Horizontal, int distance = 3)
            : base(25, true, backClr)
        {
            this.mode = mode;
            this.distance = distance;
        }

        private readonly Mode mode;
        private readonly int distance;

        protected override void OnKeyDown(MyKey key)
        {
            // Get random fully saturated and bright colour
            // Color clr = ColorUtil.GetRandom().WithSaturation(1f).WithBrightness(1f);
            float hue = random.Next(360);
            Color clr = ColorUtil.GetFromHSB(hue, 1f, 1f);
            var keyPos = KeyboardInfo.GetPositions(key);

            switch (mode)
            {
                case Mode.Horizontal:
                    // Lit pressed key
                    const int fadeInH = 200;
                    const int stayH = 700;
                    const int fadeOutH = 500;
                    ledCont.ClearActions(key);
                    ledCont.LightKey(key, clr, new Envelope(0, fadeInH, stayH, fadeOutH));

                    // Loop over y values
                    foreach (int y in keyPos.Select(p => p.Item2))
                    {
                        var leftPos = keyPos.OrderBy(p => p.Item1).First();
                        var rightPos = keyPos.OrderBy(p => p.Item1).Last();
                        int i = 0;
                        int delay = 0;

                        // Loop over x values
                        while (true)
                        {
                            i++;
                            if (distance > 0)
                            {
                                if (i > distance)
                                    break;
                                clr = clr.WithBrightness(b => b - 1f / (distance + 1));
                            }
                            delay += 50;

                            // Get new projectile positions
                            int leftX = leftPos.Item1 - i;
                            int rightX = rightPos.Item1 + i;
                            bool leftLit = (leftX >= 0);
                            bool rightLit = (rightX < KeyboardInfo.HorizKeyCount);
                            if (!leftLit && !rightLit)
                                break;

                            // Increase hue by 15°
                            clr = clr.WithHue(h => h + 15f);

                            if (leftLit)
                            {
                                MyKey leftKey = KeyboardInfo.KeyMatrix[y, leftX];
                                if (leftKey != 0)
                                    ledCont.LightKey(leftKey, clr,
                                        new Envelope(delay, fadeInH, stayH, fadeOutH));
                            }
                            if (rightLit)
                            {
                                MyKey rightKey = KeyboardInfo.KeyMatrix[y, rightX];
                                if (rightKey != 0)
                                    ledCont.LightKey(rightKey, clr,
                                        new Envelope(delay, fadeInH, stayH, fadeOutH));
                            }
                        }
                    }
                    break;

                case Mode.Radial:
                    // Lit pressed key
                    const int fadeInC = 150;
                    const int stayC = 400;
                    const int fadeOutC = 850;
                    const int delayC = 50;
                    ledCont.ClearActions(key);
                    ledCont.LightKey(key, clr, new Envelope(0, fadeInC, stayC, fadeOutC));

                    // TODO account for radius

                    {
                        var leftX = keyPos.Min(p => p.Item1) - 1;
                        var rightX = keyPos.Max(p => p.Item1) + 1;
                        var upY = keyPos.Min(p => p.Item2) - 1;
                        var downY = keyPos.Max(p => p.Item2) + 1;
                        bool leftLit = (leftX >= 0);
                        bool rightLit = (rightX < KeyboardInfo.HorizKeyCount);
                        bool upLit = (upY >= 0);
                        bool downLit = (downY < KeyboardInfo.VertKeyCount);

                        // Vertical borders loop (discount corners)
                        for (int y = Math.Max(upY + 1, 0); y <= Math.Min(downY - 1, KeyboardInfo.VertKeyCount - 1); y++)
                        {
                            if (leftLit)
                            {
                                MyKey leftKey = KeyboardInfo.KeyMatrix[y, leftX];
                                if (leftKey != 0)
                                {
                                    Color leftClr = clr.WithHue(h => h + random.Next(-30, 31));
                                    ledCont.LightKey(leftKey, leftClr,
                                        new Envelope(delayC, fadeInC, stayC, fadeOutC));
                                }
                            }
                            if (rightLit)
                            {
                                MyKey rightKey = KeyboardInfo.KeyMatrix[y, rightX];
                                if (rightKey != 0)
                                {
                                    Color rightClr = clr.WithHue(h => h + random.Next(-30, 31));
                                    ledCont.LightKey(rightKey, rightClr,
                                        new Envelope(delayC, fadeInC, stayC, fadeOutC));
                                }
                            }
                        }

                        // Horizontal borders loop (discount corners)
                        for (int x = Math.Max(leftX + 1, 0); x <= Math.Min(rightX - 1, KeyboardInfo.HorizKeyCount - 1); x++)
                        {
                            if (upLit)
                            {
                                MyKey upKey = KeyboardInfo.KeyMatrix[upY, x];
                                if (upKey != 0)
                                {
                                    Color upClr = clr.WithHue(h => h + random.Next(-30, 31));
                                    ledCont.LightKey(upKey, upClr,
                                        new Envelope(delayC, fadeInC, stayC, fadeOutC));
                                }
                            }
                            if (downLit)
                            {
                                MyKey downKey = KeyboardInfo.KeyMatrix[downY, x];
                                if (downKey != 0)
                                {
                                    Color downClr = clr.WithHue(h => h + random.Next(-30, 31));
                                    ledCont.LightKey(downKey, downClr,
                                        new Envelope(delayC, fadeInC, stayC, fadeOutC));
                                }
                            }
                        }
                    }

                    break;
            }
        }

        protected override void OnKeyUp(MyKey key)
        {

        }

        protected override void Tick(long totalMs, long deltaMs)
        {

        }
    }
}
