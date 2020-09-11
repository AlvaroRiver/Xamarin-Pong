using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content.PM;
using Android.Views;

namespace XamarinPong
{
    [Activity(Label = "Xamarin Pong", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : AppCompatActivity
    {

        //Menu items
        Button playButton, settingsButton, exitButton;

        //Pong items
        private ImageView leftPlayer, rightPlayer, ball, background;
        private TextView score;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.main_menu);

            playButton = FindViewById<Button>(Resource.Id.btnPlay);
            settingsButton = FindViewById<Button>(Resource.Id.btnSettings);
            exitButton = FindViewById<Button>(Resource.Id.btnExit);

            playButton.Click += (e, o) => SetContentView(Resource.Layout.game_screen);
               
          
             
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}