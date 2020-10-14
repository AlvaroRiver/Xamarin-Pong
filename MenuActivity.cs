using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using Tweetinvi.Exceptions;

namespace XamarinPong
{
    [Activity(Label = "Xamarin Pong", Theme = "@style/AppTheme", Icon = "@drawable/icon",
        ScreenOrientation =ScreenOrientation.Sensor , LaunchMode = LaunchMode.SingleInstance,
        MainLauncher = true)]
    public class MenuActivity : Android.Support.V7.App.AppCompatActivity
    {

        //Menu items
        Button playButton, settingsButton, exitButton, twitterButton, joinButton, hostButton, pinButton;
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
            joinButton = FindViewById<Button>(Resource.Id.btnJoin);
            hostButton = FindViewById<Button>(Resource.Id.btnHost);
            pinButton = FindViewById<Button>(Resource.Id.btnSendPIN);
            UpdateHighscore(Settings.highScore);

            playButton.Click += (e, o) =>
            {
                Intent game = new Intent(this, typeof(GameActivity));
                StartActivity(game);
            };

            hostButton.Click += (e, o) =>
            {
                Intent host = new Intent(this, typeof(HostActivity));
                StartActivity(host);
            };

            joinButton.Click += (e, o) =>
            {
                Intent join = new Intent(this, typeof(JoinActivity));
                StartActivity(join);
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
                var uri = Twitter.LogIn();
                twitterPIN.Visibility = ViewStates.Visible;
                pinButton.Visibility = ViewStates.Visible;
                StartActivity(new Intent(Intent.ActionView, uri));
            };

            pinButton.Click += (e,o) =>
            {
                if(Twitter.SendPIN(pinButton.Text))
                { 
                    twitterPIN.Text = "";
                    twitterPIN.Hint = "Score shared!";
                }
                else
                {
                    twitterPIN.Text = "";
                    twitterPIN.Hint = "PIN is 7 digits long";
                }
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