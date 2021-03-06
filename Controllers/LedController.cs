﻿// Started 04-11-2016, Basile Van Hoorick
// Wrapper for LogitechGSDK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace KeyDecorator
{
    public class LedController
    {
        // Describes a future color change
        private struct DelayedAction
        {
            internal DelayedAction(long startMs, MyKey key, Color clr, int fadeMs)
            {
                this.StartTimeMs = startMs;
                this.Key = key;
                this.Color = clr;
                this.FadeMs = fadeMs;
            }

            internal long StartTimeMs;
            internal MyKey Key;
            internal Color Color;
            internal int FadeMs;
        }

        public LedController(int intervalMs, Color backClr)
        {
            this.intervalMs = intervalMs;
            this.backClr = backClr;
            this.keyColors = new Dictionary<MyKey, Color>();
            this.pendingActions = new HashSet<DelayedAction>();

            // Initialize all keys to background
            foreach (MyKey key in Enum.GetValues(typeof(MyKey)))
                FadeKey(key, backClr, 0);
        }

        private readonly int intervalMs;
        private readonly Color backClr;
        private readonly Dictionary<MyKey, Color> keyColors;
        private readonly HashSet<DelayedAction> pendingActions;
        private Thread thd = null;
        private volatile bool running = false;
        private volatile bool toStop = false;

        public bool IsRunning => running;

        public void Start()
        {
            if (thd == null)
            {
                this.toStop = false;
                thd = new Thread(new ThreadStart(Run));
                thd.Start();
                this.running = true;
            }
        }

        public void Stop()
        {
            this.toStop = true;
        }

        private void Run()
        {
            while (true)
            {
                Tick();
                Thread.Sleep(intervalMs);
                if (toStop)
                    break;
            }
            this.running = false;
            this.toStop = false;
        }

        private void Tick()
        {
            long curTimeMs = DateTime.Now.Ticks / 10000;

            // Execute pending actions & remove
            List<DelayedAction> toRun;
            lock (pendingActions)
                // TODO not thread-safe :(
                toRun = pendingActions.Where(da => da.StartTimeMs <= curTimeMs)
                    .ToList();
            foreach (var da in toRun)
            {
                RunAction(da);
                lock (pendingActions)
                    pendingActions.Remove(da);
            }
        }

        private void RunAction(DelayedAction da)
        {
            FadeKey(da.Key, da.Color, da.FadeMs);
        }

        // TODO: return interpolated color while fading
        // TODO: add time argument for lookup into future
        public Color GetKeyColor(MyKey key) => keyColors[key];

        // Pulses key
        /// <summary>
        /// Pulses key inbetween two values.
        /// </summary>
        /// <param name="infinite">TODO</param>
        public void PulseKey(MyKey key, Color clr1, Color clr2, int fadeMs, bool infinite = false)
        {
            LogitechGSDK.LogiLedPulseSingleKey(key,
                Pct(clr1.R), Pct(clr1.G), Pct(clr1.B),
                Pct(clr2.R), Pct(clr2.G), Pct(clr2.B), fadeMs, infinite);
            if (!clr1.Equals(clr2))
                Console.WriteLine(key + ": " + clr1 + " -> " + clr2);
            else
                Console.WriteLine(key + ": " + clr1);
            // TODO dictionary if infinite?
            keyColors[key] = clr2;
        }

        /// <summary>
        /// Sets key color right now, with optional fading.
        /// </summary>
        public void FadeKey(MyKey key, Color clr, int fadeMs = 0)
        {
            if (fadeMs <= 0 || !keyColors.ContainsKey(key))
            {
                // Now: set color
                // TODO ugly hack:
                /* The commented instruction gets ignored as long as an effect is
                 * running, so we force it using a "pulse" effect with only one
                 * colour. When an effect is finished, the key resets to the colour
                 * it had BEFORE this app started. Another possible workaround would
                 * be to stop effects & set the lighting every 5 ms during the next
                 * 100 ms or so, but flashes would often be visible. */
                /*LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(
                    key, pct(clr.R), pct(clr.G), pct(clr.B));*/
                PulseKey(key, clr, clr, 10000, true);
            }
            else
            {
                var origClr = keyColors[key];
                if (clr != origClr)
                {
                    // Now: fade origClr -> clr in fadeMs
                    PulseKey(key, origClr, clr, fadeMs, false);
                    // After fadeMs: set clr
                    AddDelAction(fadeMs, key, clr, 0);
                }
            }
            // Update dictionary
            keyColors[key] = clr;
        }

        /// <summary>
        /// Changes key color to 'on', then to background.
        /// </summary>
        public void LightKey(MyKey key, Color on, Envelope env)
        {
            ClearActions(key);
            LightKey(key, on, backClr, env);
            // LitKey(key, on, keyColors[key], env);
        }

        /// <summary>
        /// Changes key color to 'on', then to 'off'.
        /// </summary>
        public void LightKey(MyKey key, Color on, Color off, Envelope env)
        {
            ClearActions(key);
            // After Delay: fade origClr -> on in FadeIn
            AddDelAction(env.Delay, key, on, env.FadeIn);
            // After Delay+FadeIn+Stay: fade on -> off in FadeOut
            AddDelAction(env.Delay + env.FadeIn + env.Stay, key, off, env.FadeOut);
        }

        /// <summary>
        /// Clears all future actions on the specified key. Gets called whenever LitKey() is called.
        /// </summary>
        public void ClearActions(MyKey key)
        {
            pendingActions.RemoveWhere(da => da.Key == key);
        }

        /// <summary>
        /// Applies the given color array to the whole keyboard.
        /// Note that this method overwrites all previous color settings,
        /// and keys could be overwritten by a delayed action afterwards.
        /// </summary>
        /// <param name="bitmap">21 * 6 array of colors to paint on the keyboard</param>
        public void SetBitmap(Color[,] bitmap)
        {
            byte[] convert = new byte[KeyboardInfo.BmpWidth * KeyboardInfo.BmpHeight * 4]; // 4 bytes per key (BGRA)
            for (int i = 0; i < KeyboardInfo.BmpWidth; i++)
                for (int j = 0; j < KeyboardInfo.BmpHeight; j++)
                {
                    convert[(i + j * KeyboardInfo.BmpWidth) * 4] = bitmap[i, j].B;
                    convert[(i + j * KeyboardInfo.BmpWidth) * 4 + 1] = bitmap[i, j].G;
                    convert[(i + j * KeyboardInfo.BmpWidth) * 4 + 2] = bitmap[i, j].R;
                    convert[(i + j * KeyboardInfo.BmpWidth) * 4 + 3] = 255;
                }

            LogitechGSDK.LogiLedSetLightingFromBitmap(convert);
        }

        /// <summary>
        /// Clears all the effects currently running.
        /// This also means that the background color will not be enforced anymore,
        /// until delayed actions might modify keys again.
        /// </summary>
        public void StopEffects()
        {
            LogitechGSDK.LogiLedStopEffects();
        }

        // Adds delayed action with specified delay (NOT absolute time)
        private void AddDelAction(int addMs, MyKey key, Color clr, int fadeMs)
        {
            // Remove conflicting existing actions first
            long startMs = GetFutureTime(addMs);
            long endMs = startMs + fadeMs;

            lock (pendingActions)
            {
                // Remove previously added actions with overlapping ranges
                pendingActions.RemoveWhere(da =>
                key == da.Key && IsConflictRange(
                startMs, endMs, da.StartTimeMs, da.StartTimeMs + da.FadeMs));

                // Remove any older action that has effect after startMs
                /*pendingActions.RemoveWhere(da =>
                key == da.Key && da.StartTimeMs > startMs);*/

                // Add this action
                pendingActions.Add(new DelayedAction(startMs, key, clr, fadeMs));
            }
        }

        // Gets a specified time into the future
        private static long GetFutureTime(int addMs)
            => DateTime.Now.Ticks / 10000 + addMs;

        // Converts color byte value to percentage
        private static int Pct(byte value) => value * 100 / 255;

        private static bool IsConflictRange(long start1, long end1, long start2, long end2)
            => !((end1 <= start2) || (end2 <= start1));
    }
}
