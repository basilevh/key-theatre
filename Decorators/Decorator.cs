// Started 04-11-2016, Basile Van Hoorick
// User-chosen object that operates LedController

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace KeyDecorator.Decorators
{
    /// <summary>
    /// Abstract key decorator class. Implementations operate the LED controller, using a keylogger if necessary.
    /// </summary>
    public abstract class Decorator
    {
        /// <summary>
        /// Pseudo-random generator for convenience.
        /// </summary>
        protected readonly Random rnd = new Random();
        
        /// <param name="intervalMs">Tick interval in milliseconds.</param>
        /// <param name="needsKeyLog">Decides whether to enable keylogging. If false, the keylogger object is left to null.</param>
        /// <param name="backClr">Ambient keyboard background colour, for keys on which no activity takes place.</param>
        public Decorator(int intervalMs, bool needsKeyLog, Color backClr)
        {
            this.intervalMs = intervalMs;
            this.ledCont = new LedController(25, backClr);
            if (needsKeyLog)
            {
                this.keyLogger = new KeyLogger(25);
                keyLogger.OnKeyDown += OnKeyDown;
                keyLogger.OnKeyUp += OnKeyUp;
            }
        }

        private readonly int intervalMs;
        private Thread thd = null;
        private volatile bool running = false;
        private volatile bool toStop = false;
        /// <summary>
        /// Led controller object.
        /// </summary>
        protected readonly LedController ledCont;
        /// <summary>
        /// Keylogger object, which is non-null only if needsKeyLog was true upon construction.
        /// </summary>
        protected readonly KeyLogger keyLogger;

        /// <summary>
        /// Returns whether the controller thread is currently active.
        /// </summary>
        public bool IsRunning => running;

        /// <summary>
        /// Starts the controller thread.
        /// </summary>
        public void Start()
        {
            if (thd == null)
            {
                this.toStop = false;
                thd = new Thread(new ThreadStart(run));
                thd.Start();
                this.running = true;
            }
            if (ledCont != null)
                ledCont.Start();
            if (keyLogger != null)
                keyLogger.Start();
        }


        /// <summary>
        /// Stops the controller thread.
        /// </summary>
        public void Stop()
        {
            this.toStop = true;
            if (ledCont != null)
                ledCont.Stop();
            if (keyLogger != null)
                keyLogger.Stop();
        }

        private void run()
        {
            long lastTimeMs = -1;
            while (true)
            {
                long curTimeMs = DateTime.Now.Ticks / 10000;
                if (lastTimeMs == -1)
                    lastTimeMs = curTimeMs;
                long deltaMs = curTimeMs - lastTimeMs;

                Tick(curTimeMs, curTimeMs - lastTimeMs);

                Thread.Sleep(intervalMs); // Tick is assumed to use negligible time
                if (toStop)
                    break;

                lastTimeMs = curTimeMs;
            }
            this.running = false;
            this.toStop = false;
        }

        /// <summary>
        /// Main tick method, called every intervalMs.
        /// </summary>
        /// <param name="totalMs">Total passed time since the thread was last started, in milliseconds.</param>
        /// <param name="deltaMs">Total passed time since the last tick, in milliseconds.</param>
        protected abstract void Tick(long totalMs, long deltaMs);

        /// <summary>
        /// Triggered whenever a key press event is detected by the keylogger (if enabled).
        /// </summary>
        protected abstract void OnKeyDown(MyKey key);

        /// <summary>
        /// Triggered whenever a key release event is detected by the keylogger (if enabled).
        /// </summary>
        protected abstract void OnKeyUp(MyKey key);
    }
}
