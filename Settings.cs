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
using Tweetinvi;
using Android.Util;
using System.IO;
using Newtonsoft.Json;
using Java.Lang;

namespace XamarinPong
{
    class Settings
    {
        //Initialized at default values
        public static bool Audio = true, RightPaddle = false;
        public static int R = 100, G = 149, B = 247;
        public static int Difficulty = 2, Sensivity = 5;
        public static int player = 0, ball = 0, maxScore = 5, highScore = 0, DebugMode = 0;

        public static void loadSettings(string path)
        {
            //path	"/data/user/0/XamarinPong.XamarinPong/files/.local/share/local.txt"	string
            var json = File.ReadAllText(path);
            if (json.Length == 0) throw new System.Exception();
            var settings = JsonConvert.DeserializeObject<List<string>>(json);
            Audio = settings[0] == "0" ? false : true;
            RightPaddle = settings[1] == "0" ? false : true;
            R = int.Parse(settings[2]);
            G = int.Parse(settings[3]);
            B = int.Parse(settings[4]);
            Difficulty = int.Parse(settings[5]);
            Sensivity = int.Parse(settings[6]);
            player = int.Parse(settings[7]);
            ball = int.Parse(settings[8]);
            maxScore = int.Parse(settings[9]);
            highScore = int.Parse(settings[10]);
            DebugMode = int.Parse(settings[11]);
        }

        public static void saveSettings(string path)
        {
            var json = JsonConvert.SerializeObject(
                new List<int>() {
                    Audio == false ? 0 : 1 , RightPaddle == false ? 0 : 1 , R,G,B,Difficulty,Sensivity,player,ball,maxScore,highScore, DebugMode }
                );
            var writer = File.CreateText(path);
            writer.Write(json);
            writer.Flush();
            writer.Close();
        }
    }
}