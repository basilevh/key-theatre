// Started 04-11-2016, Basile Van Hoorick
// Uses and interprets GetAsyncKeyState to provide real-time callbacks on key presses and releases

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace KeyDecorator
{
    public class KeyLogger
    {
        #region Key Codes Info

        // Info for FRA layout as of 05-07-2016
        private static Dictionary<MyKey, ISet<int>> nameTrigDict = new Dictionary<MyKey, ISet<int>>()
        {
            {MyKey.ESC, new HashSet<int>() {27} },
            {MyKey.F1, new HashSet<int>() {112} },
            {MyKey.F2, new HashSet<int>() {113} },
            {MyKey.F3, new HashSet<int>() {114} },
            {MyKey.F4, new HashSet<int>() {115} },
            {MyKey.F5, new HashSet<int>() {116} },
            {MyKey.F6, new HashSet<int>() {117} },
            {MyKey.F7, new HashSet<int>() {118} },
            {MyKey.F8, new HashSet<int>() {119} },
            {MyKey.F9, new HashSet<int>() {120} },
            {MyKey.F10, new HashSet<int>() {121} },
            {MyKey.F11, new HashSet<int>() {122} },
            {MyKey.F12, new HashSet<int>() {123} },
            {MyKey.PRINT_SCR, new HashSet<int>() {44/*, 255*/} }, // 255 was there previously..?
            {MyKey.SCROLL_LK, new HashSet<int>() {145} },
            {MyKey.PAUSE_BRK, new HashSet<int>() {19/*, 255*/} },
            {MyKey.TILDE, new HashSet<int>() {222} },
            {MyKey.ONE, new HashSet<int>() {49} },
            {MyKey.TWO, new HashSet<int>() {50} },
            {MyKey.THREE, new HashSet<int>() {51} },
            {MyKey.FOUR, new HashSet<int>() {52} },
            {MyKey.FIVE, new HashSet<int>() {53} },
            {MyKey.SIX, new HashSet<int>() {54} },
            {MyKey.SEVEN, new HashSet<int>() {55} },
            {MyKey.EIGHT, new HashSet<int>() {56} },
            {MyKey.NINE, new HashSet<int>() {57} },
            {MyKey.ZERO, new HashSet<int>() {48} },
            {MyKey.MINUS, new HashSet<int>() {219} },
            {MyKey.EQUALS, new HashSet<int>() {187} },
            {MyKey.BACKSPACE, new HashSet<int>() {8} },
            {MyKey.INSERT, new HashSet<int>() {45} },
            {MyKey.HOME, new HashSet<int>() {36} },
            {MyKey.PAGE_UP, new HashSet<int>() {33} },
            {MyKey.NUM_LOCK, new HashSet<int>() {144} },
            {MyKey.NUM_SLASH, new HashSet<int>() {111} },
            {MyKey.NUM_AST, new HashSet<int>() {106} },
            {MyKey.NUM_MINUS, new HashSet<int>() {109} },
            {MyKey.TAB, new HashSet<int>() {9} },
            {MyKey.Q, new HashSet<int>() {65} },
            {MyKey.W, new HashSet<int>() {90} },
            {MyKey.E, new HashSet<int>() {69} },
            {MyKey.R, new HashSet<int>() {82} },
            {MyKey.T, new HashSet<int>() {84} },
            {MyKey.Y, new HashSet<int>() {89} },
            {MyKey.U, new HashSet<int>() {85} },
            {MyKey.I, new HashSet<int>() {73} },
            {MyKey.O, new HashSet<int>() {79} },
            {MyKey.P, new HashSet<int>() {80} },
            {MyKey.OPEN_BRACK, new HashSet<int>() {221} },
            {MyKey.CLOSE_BRACK, new HashSet<int>() {186} },
            {MyKey.DELETE, new HashSet<int>() {46} },
            {MyKey.END, new HashSet<int>() {35} },
            {MyKey.PAGE_DOWN, new HashSet<int>() {34} },
            {MyKey.NUM_SEVEN, new HashSet<int>() {103} },
            {MyKey.NUM_EIGHT, new HashSet<int>() {104} },
            {MyKey.NUM_NINE, new HashSet<int>() {105} },
            {MyKey.NUM_PLUS, new HashSet<int>() {107} },
            {MyKey.CAPS_LOCK, new HashSet<int>() {20} },
            {MyKey.A, new HashSet<int>() {81} },
            {MyKey.S, new HashSet<int>() {83} },
            {MyKey.D, new HashSet<int>() {68} },
            {MyKey.F, new HashSet<int>() {70} },
            {MyKey.G, new HashSet<int>() {71} },
            {MyKey.H, new HashSet<int>() {72} },
            {MyKey.J, new HashSet<int>() {74} },
            {MyKey.K, new HashSet<int>() {75} },
            {MyKey.L, new HashSet<int>() {76} },
            {MyKey.SEMICOLON, new HashSet<int>() {77} },
            {MyKey.APOSTROPHE, new HashSet<int>() {192} },
            {MyKey.MY_MUAST, new HashSet<int>() {220} }, // not on qwerty
            {MyKey.ENTER, new HashSet<int>() {13} },
            {MyKey.NUM_FOUR, new HashSet<int>() {100} },
            {MyKey.NUM_FIVE, new HashSet<int>() {101} },
            {MyKey.NUM_SIX, new HashSet<int>() {102} },
            {MyKey.LEFT_SHIFT, new HashSet<int>() {16, 160} },
            {MyKey.MY_GTLT, new HashSet<int>() {226} }, // not on qwerty
            {MyKey.Z, new HashSet<int>() {87} },
            {MyKey.X, new HashSet<int>() {88} },
            {MyKey.C, new HashSet<int>() {67} },
            {MyKey.V, new HashSet<int>() {86} },
            {MyKey.B, new HashSet<int>() {66} },
            {MyKey.N, new HashSet<int>() {78} },
            {MyKey.M, new HashSet<int>() {188} },
            {MyKey.COMMA, new HashSet<int>() {190} },
            {MyKey.PERIOD, new HashSet<int>() {191} },
            {MyKey.FORWARD_SLASH, new HashSet<int>() {223} },
            {MyKey.RIGHT_SHIFT, new HashSet<int>() {16, 161} },
            {MyKey.ARROW_UP, new HashSet<int>() {38} },
            {MyKey.NUM_ONE, new HashSet<int>() {97} },
            {MyKey.NUM_TWO, new HashSet<int>() {98} },
            {MyKey.NUM_THREE, new HashSet<int>() {99} },
            {MyKey.NUM_ENTER, new HashSet<int>() {13} },
            {MyKey.LEFT_CTRL, new HashSet<int>() {17, 162} },
            {MyKey.LEFT_WIN, new HashSet<int>() {91} },
            {MyKey.LEFT_ALT, new HashSet<int>() {18, 164} },
            {MyKey.SPACE, new HashSet<int>() {32} },
            {MyKey.RIGHT_ALT, new HashSet<int>() {17, 18, 162, 165} },
            {MyKey.RIGHT_WIN, new HashSet<int>() {92} },
            {MyKey.APP_SELECT, new HashSet<int>() {93} },
            {MyKey.RIGHT_CTRL, new HashSet<int>() {17, 163} },
            {MyKey.ARROW_LEFT, new HashSet<int>() {37} },
            {MyKey.ARROW_DOWN, new HashSet<int>() {40} },
            {MyKey.ARROW_RIGHT, new HashSet<int>() {39} },
            {MyKey.NUM_ZERO, new HashSet<int>() {96} },
            {MyKey.NUM_PERIOD, new HashSet<int>() {110} }
        };

        #endregion

        private static Dictionary<int, ISet<MyKey>> codePosNameDict;

        static KeyLogger()
        {
            // Map all possible names a code could have been triggered by
            codePosNameDict = new Dictionary<int, ISet<MyKey>>();
            foreach (var kvp in nameTrigDict)
                foreach (int code in kvp.Value)
                {
                    if (!codePosNameDict.ContainsKey(code))
                        codePosNameDict[code] = new HashSet<MyKey>();
                    codePosNameDict[code].Add(kvp.Key);
                }
        }

        [DllImport("user32.dll")]
        extern static short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        public delegate void EventKey(MyKey key);
        public event EventKey OnKeyDown;
        public event EventKey OnKeyUp;

        public KeyLogger(int intervalMs)
        {
            this.intervalMs = intervalMs;
            this.lastPressedKeys = new HashSet<MyKey>();
        }

        private readonly int intervalMs;
        private Thread thd = null;
        private volatile bool running = false;
        private volatile bool toStop = false;

        public bool IsRunning => running;

        public void Start()
        {
            if (thd == null)
            {
                this.toStop = false;
                thd = new Thread(new ThreadStart(run));
                thd.Start();
                this.running = true;
            }
        }

        public void Stop()
        {
            this.toStop = true;
        }

        private void run()
        {
            while (true)
            {
                tick();
                Thread.Sleep(intervalMs);
                if (toStop)
                    break;
            }
            this.running = false;
            this.toStop = false;
        }

        private ISet<MyKey> lastPressedKeys;

        private void tick()
        {
            // Get all possibly pressed key names
            ISet<int> curPressedCodes = new HashSet<int>();
            ISet<MyKey> curPosNames = new HashSet<MyKey>();
            for (int i = 0; i < 256; i++)
                if (GetAsyncKeyState((System.Windows.Forms.Keys)i) != 0
                    && codePosNameDict.ContainsKey(i))
                {
                    curPressedCodes.Add(i);
                    curPosNames.UnionWith(codePosNameDict[i]);
                }

            // Get currently pressed keys
            ISet<MyKey> curPressedKeys = new HashSet<MyKey>();
            foreach (MyKey key in curPosNames)
            {
                var requiredCodes = nameTrigDict[key];
                bool pressed = requiredCodes.IsSubsetOf(curPressedCodes);
                // LEFT_CTRL codes are a subset of RIGHT_ALT codes,
                // which makes it impossible to detect LEFT_CTRL if RIGHT_ALT is pressed,
                // therefore assume LEFT_CTRL is not pressed if RIGHT_ALT is
                if (key == MyKey.LEFT_CTRL
                    && nameTrigDict[MyKey.RIGHT_ALT].IsSubsetOf(curPressedCodes))
                    pressed = false;
                // NUM_ENTER is indistinguishable from ENTER
                if (key == MyKey.NUM_ENTER
                    && nameTrigDict[MyKey.NUM_ENTER].IsSubsetOf(curPressedCodes))
                    pressed = false;

                if (pressed)
                    curPressedKeys.Add(key);
            }

            // Trigger events
            foreach (MyKey key in curPressedKeys.Except(lastPressedKeys))
                OnKeyDown?.Invoke(key);
            foreach (MyKey key in lastPressedKeys.Except(curPressedKeys))
                OnKeyUp?.Invoke(key);

            this.lastPressedKeys = curPressedKeys;
        }
    }
}
