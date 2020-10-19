using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Org.Apache.Http.Util;
using Tweetinvi.Core.Events;

namespace XamarinPong.Multiplayer
{
    class Message
    {
        public enum Type:byte { SyncSettings = 0, SyncBall = 1, MovePlayer = 2, Score = 4, Finish = 5};

        public static byte[] SettingsMsg(byte hostPaddle, byte difficulty, byte gamePoints)
        {
            return new byte[] { (byte)Type.SyncSettings, hostPaddle, difficulty, gamePoints };
        }

        public static byte[] BallMsg(byte Xdir, byte Ydir)
        {
            return new byte[]{ (byte)Type.SyncBall, Xdir, Ydir };
        }

        public static byte[] MovePlayersMsg(byte displacement)
        {
            return new byte[] { (byte)Type.MovePlayer,  displacement };
        }

        public static byte[] ScoreMsg(byte player) //0 is host player 1 other
        {
            return new byte[] { (byte)Type.Score, player };
        }

        public static byte[] FinishMsg()
        {
            return new byte[] { (byte)Type.Finish };
        }
    }
}