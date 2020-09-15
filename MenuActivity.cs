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

namespace MonoGameTest
{
    [Activity(Label = "Xamarin Pong", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MenuActivity : Android.Support.V7.App.AppCompatActivity
    {

        //Menu items
        Button playButton, settingsButton, exitButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.main_menu);

            playButton = FindViewById<Button>(Resource.Id.btnPlay);
            settingsButton = FindViewById<Button>(Resource.Id.btnSettings);
            exitButton = FindViewById<Button>(Resource.Id.btnExit);

            playButton.Click += (e, o) =>
            {

                Intent game = new Intent(this, typeof(GameActivity));
                StartActivity(game);
            };

            settingsButton.Click += (e, o) =>
            {
                Intent settings = new Intent(this, typeof(SettingsActivity));
                StartActivity(settings);
            };

            exitButton.Click += (exitButton, o) =>
            {
                var activity = (Activity)this;
                activity.FinishAffinity();
            };

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}