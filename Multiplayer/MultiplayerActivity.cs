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
using Android.Content.PM;
using Microsoft.Xna.Framework;

namespace XamarinPong.Multiplayer
{
    [Activity(
        Label = "@Xamarin Pong",
        Icon = "@drawable/icon",
        Theme = "@style/AppTheme",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Landscape,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class MultiplayerActivity : AndroidGameActivity
    {
        private MultiplyerPong pong;
        private View pongView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            pong = new MultiplyerPong();

            pongView = pong.Services.GetService<View>();
            SetContentView(pongView);
            pong.Run();
        }

        protected override void OnPause()
        {
            base.OnPause();
            MultiplyerPong.inBackground = true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            MultiplyerPong.inBackground = false;
        }

        protected override void OnStop()
        {
            base.OnStop();
            FinishAffinity();
        }
    }

}