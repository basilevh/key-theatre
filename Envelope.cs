// Started 04-11-2016, Basile Van Hoorick
// Level envelope containing delay, fade-in, stay, fade-out

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyDecorator
{
    public class Envelope
    {
        public Envelope(int delayMs, int fadeInMs, int stayMs, int fadeOutMs)
        {
            this.delayMs = delayMs;
            this.fadeInMs = fadeInMs;
            this.stayMs = stayMs;
            this.fadeOutMs = fadeOutMs;
        }

        private int delayMs, fadeInMs, stayMs, fadeOutMs;

        public int Delay => delayMs;
        public int FadeIn => fadeInMs;
        public int Stay => stayMs;
        public int FadeOut => fadeOutMs;
    }
}
