// Started 24-11-2016, Basile Van Hoorick
// Lits keypresses in random colour

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyDecorator.Decorators
{
    public class PressSimple : Decorator
    {
        public PressSimple(Color backClr)
            : base(25, true, backClr)
        {
        }

        protected override void OnKeyDown(MyKey key)
        {
            // Get random fully saturated and bright colour
            Color clr = ColorUtil.GetFromHSB(random.Next(360), 1f, 1f);

            // Lit pressed key (no delay)
            const int fadeIn = 100;
            const int stay = 100;
            const int fadeOut = 3800;
            ledCont.LightKey(key, clr, new Envelope(0, fadeIn, stay, fadeOut));
        }

        protected override void OnKeyUp(MyKey key)
        {

        }

        protected override void Tick(long totalMs, long deltaMs)
        {
            
        }
    }
}
