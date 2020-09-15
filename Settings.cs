using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MonoGameTest
{
    class Settings
    {
        public static bool Audio = true;
        public static int R = 100, G = 149, B = 247;
        public static int Difficulty = 2;
        public static int player = 0, ball = 0;
    }
}