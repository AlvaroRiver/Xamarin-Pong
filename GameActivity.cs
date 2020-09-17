using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Microsoft.Xna.Framework;
using System;
using System.Threading.Tasks;

namespace MonoGameTest
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
    public class GameActivity : AndroidGameActivity
    {
        private Pong pong;
        private View pongView;
  
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            pong = new Pong();
         
            pongView = pong.Services.GetService<View>();
            SetContentView(pongView);
            pong.Run();

        }

        protected override void OnPause()
        {
            base.OnPause();
            Pong.inBackground = true;
        }

        protected override void OnResume()
        {
            base.OnResume();
            Pong.inBackground = false;
        }

        protected override void OnStop()
        {

            FinishAffinity();
            base.OnStop();

        }




    }

    
}
