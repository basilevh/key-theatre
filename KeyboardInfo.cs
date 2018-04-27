// Started 04-11-2016, Basile Van Hoorick

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyDecorator
{
    /// <summary>
    /// Utility class with my key positions (FR layout)
    /// </summary>
    public static class KeyboardInfo
    {
        // Constants
        public const int HorizKeyCount = 22; // for my own matrix
        public const int VertKeyCount = 6;
        public const int BmpWidth = 21; // for calls to bitmap
        public const int BmpHeight = 6;

        // Qwerty matrix (access: y, x)
        public static readonly MyKey[,] KeyMatrix = new MyKey[,] {
        { MyKey.ESC,      0,               MyKey.F1,       MyKey.F2,       MyKey.F3,       MyKey.F4,       MyKey.F5,       MyKey.F6,       MyKey.F7,       MyKey.F8,       0,
          MyKey.F9,       MyKey.F10,      MyKey.F11,      MyKey.F12,      MyKey.PRINT_SCR, MyKey.SCROLL_LK, MyKey.PAUSE_BRK, 0,            0,               0,               0 },
        { MyKey.TILDE,    MyKey.ONE,      MyKey.TWO,      MyKey.THREE,    MyKey.FOUR,     MyKey.FIVE,     MyKey.SIX,      MyKey.SEVEN,    MyKey.EIGHT,    MyKey.NINE,     MyKey.ZERO,
          MyKey.MINUS,    MyKey.EQUALS,   MyKey.BACKSPACE, MyKey.BACKSPACE, MyKey.INSERT, MyKey.HOME,     MyKey.PAGE_UP,  MyKey.NUM_LOCK, MyKey.NUM_SLASH, MyKey.NUM_AST, MyKey.NUM_MINUS },
        { MyKey.TAB,      MyKey.TAB,      MyKey.Q,        MyKey.W,        MyKey.E,        MyKey.R,        MyKey.T,        MyKey.Y,        MyKey.U,        MyKey.I,        MyKey.O,
          MyKey.P,        MyKey.OPEN_BRACK, MyKey.CLOSE_BRACK, MyKey.ENTER, MyKey.DELETE, MyKey.END,      MyKey.PAGE_DOWN, MyKey.NUM_SEVEN, MyKey.NUM_EIGHT, MyKey.NUM_NINE, MyKey.NUM_PLUS },
        { MyKey.CAPS_LOCK, MyKey.CAPS_LOCK, MyKey.A,      MyKey.S,        MyKey.D,        MyKey.F,        MyKey.G,        MyKey.H,        MyKey.J,        MyKey.K,        MyKey.L,
          MyKey.SEMICOLON, MyKey.APOSTROPHE, MyKey.MY_MUAST, MyKey.ENTER, 0,               0,               0,               MyKey.NUM_FOUR, MyKey.NUM_FIVE, MyKey.NUM_SIX,  MyKey.NUM_PLUS },
        { MyKey.LEFT_SHIFT, MyKey.MY_GTLT, MyKey.Z,       MyKey.X,        MyKey.C,        MyKey.V,        MyKey.B,        MyKey.N,        MyKey.M,        MyKey.COMMA,    MyKey.PERIOD,
          MyKey.FORWARD_SLASH, MyKey.RIGHT_SHIFT, MyKey.RIGHT_SHIFT, MyKey.RIGHT_SHIFT, 0, MyKey.ARROW_UP, 0,               MyKey.NUM_ONE,  MyKey.NUM_TWO,  MyKey.NUM_THREE, MyKey.NUM_ENTER },
        { MyKey.LEFT_CTRL, MyKey.LEFT_WIN, MyKey.LEFT_ALT, MyKey.SPACE,   MyKey.SPACE,    MyKey.SPACE,    MyKey.SPACE,    MyKey.SPACE,    MyKey.SPACE,    MyKey.SPACE,    MyKey.RIGHT_ALT,
          MyKey.RIGHT_WIN, MyKey.APP_SELECT, MyKey.RIGHT_CTRL, MyKey.RIGHT_CTRL, MyKey.ARROW_LEFT, MyKey.ARROW_DOWN, MyKey.ARROW_RIGHT, MyKey.NUM_ZERO, MyKey.NUM_ZERO, MyKey.NUM_PERIOD, MyKey.NUM_ENTER } };

        // TODO more accurate key to coordinate mapping

        // Key position dictionary. Input: key, output: set of (x, y) values
        private static readonly Dictionary<MyKey, ISet<Tuple<int, int>>> keyPosDict
            = new Dictionary<MyKey, ISet<Tuple<int, int>>>();

        // Static initialization
        static KeyboardInfo()
        {
            // Map key names to matrix position(s)
            for (int i = 0; i < HorizKeyCount; i++)
                for (int j = 0; j < VertKeyCount; j++)
                {
                    MyKey curKey = KeyMatrix[j, i];
                    if (KeyMatrix[j, i] != 0)
                    {
                        if (!keyPosDict.ContainsKey(curKey))
                            keyPosDict[curKey] = new HashSet<Tuple<int, int>>();
                        keyPosDict[curKey].Add(new Tuple<int, int>(i, j));
                    }
                }
        }
        
        /// <returns>Set of matrix positions that would activate the specified keycode</returns>
        public static ISet<Tuple<int, int>> GetPositions(MyKey key)
            => keyPosDict[key];
    }
}
