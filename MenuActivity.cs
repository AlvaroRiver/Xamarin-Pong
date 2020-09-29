using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace XamarinPong
{
    [Activity(Label = "Xamarin Pong", Theme = "@style/AppTheme", Icon = "@drawable/icon",
        ScreenOrientation =ScreenOrientation.Sensor , LaunchMode = LaunchMode.SingleInstance,
        MainLauncher = true)]
    public class MenuActivity : Android.Support.V7.App.AppCompatActivity
    {

        //Menu items
        Button playButton, settingsButton, exitButton, twitterButton;
        EditText twitterPIN;
        private string localData; 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            localData = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "local.json");
            //File.Delete(localData);
            if (File.Exists(localData))
                Settings.loadSettings(localData);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.main_menu);

            playButton = FindViewById<Button>(Resource.Id.btnPlay);
            settingsButton = FindViewById<Button>(Resource.Id.btnSettings);
            exitButton = FindViewById<Button>(Resource.Id.btnExit);
            twitterButton = FindViewById<Button>(Resource.Id.btnTwitter);
            twitterPIN = FindViewById<EditText>(Resource.Id.editPIN);
            UpdateHighscore(Settings.highScore);

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

            exitButton.Click += (e, o) =>
            {
                Settings.saveSettings(localData);
                FinishAffinity();
                FinishAndRemoveTask();
                System.Environment.Exit(0);
            };

            twitterButton.Click += (e, o) =>
            {
                if(Twitter.loggedIn)
                {
                    Twitter.Tweet("Hello there. I scored " + Settings.highScore + " points at #XamarinPong !");
                }
                else
                {
                    Twitter.LogIn();
                    twitterPIN.Visibility = ViewStates.Visible;
                }
            };

            twitterPIN.TextChanged += (e, o) =>
            {
                if (twitterPIN.Text.Length == 4)
                    Twitter.SetCredentials(twitterPIN.Text);
                twitterPIN.Visibility = ViewStates.Gone;
            };
        }

        //Also save settings when not finishing via Exit button
        protected override void OnStop()
        {
            Settings.saveSettings(localData);
            base.OnStop();
        }

        public void UpdateHighscore(int score)
        {
            if (score > 0)
                twitterButton.Text = "Share highscore(" + score + ")";
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateHighscore(Settings.highScore);
            if (Pong.inBackground)
                playButton.Text = "Resume";
            else
                playButton.Text = "Play";
        }
    }
}