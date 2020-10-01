using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Graphics.Drawable;
using Android.Views;
using Android.Widget;

namespace XamarinPong
{
	[Activity(Label = "Settings", Theme = "@style/AppTheme", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleInstance, ScreenOrientation = ScreenOrientation.Sensor)]
	public class SettingsActivity : Activity
	{
		RadioButton easy, normal, hard;
		ToggleButton paddleToggle, debugToggle;
		SeekBar redBar, greenBar, blueBar, playerSelect, ballSelect, sensivityBar, maxScore;
		ImageView imgPreview;
		TextView txtPlayerSelect, txtBallSelect, txtMaxScore;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.settings);
			easy = FindViewById<RadioButton>(Resource.Id.radioEasy);
			normal = FindViewById<RadioButton>(Resource.Id.radioNormal);
			hard = FindViewById<RadioButton>(Resource.Id.radioHard);
			redBar = FindViewById<SeekBar>(Resource.Id.seekBarRed);
			greenBar = FindViewById<SeekBar>(Resource.Id.seekBarGreen);
			blueBar = FindViewById<SeekBar>(Resource.Id.seekBlue);
			imgPreview = FindViewById<ImageView>(Resource.Id.imgThemePreview);
			playerSelect = FindViewById<SeekBar>(Resource.Id.seekBarPlayer);
			ballSelect = FindViewById<SeekBar>(Resource.Id.seekBarBall);
			txtPlayerSelect = FindViewById<TextView>(Resource.Id.txtPlayerSelect);
			txtBallSelect = FindViewById<TextView>(Resource.Id.txtBallSelect);
			sensivityBar = FindViewById<SeekBar>(Resource.Id.seekBarSensivity);
			maxScore = FindViewById<SeekBar>(Resource.Id.seekBarMaxScore);
			txtMaxScore = FindViewById<TextView>(Resource.Id.txtMaxScore);
			paddleToggle = FindViewById<ToggleButton>(Resource.Id.togglePaddle);
			debugToggle = FindViewById<ToggleButton>(Resource.Id.toggleDebug);

			//Preserve settings after switching screens
			loadSettings();

			easy.CheckedChange += (e, o) =>
			{
				if (easy.Checked)
					Settings.Difficulty = 1;
			};

			normal.CheckedChange += (e, o) =>
			{
				if (normal.Checked)
					Settings.Difficulty = 2;
			};

			hard.CheckedChange += (e, o) =>
			{
				if (hard.Checked)
					Settings.Difficulty = 3;
			};

			paddleToggle.CheckedChange += (e, o) =>
			{
				Settings.RightPaddle = paddleToggle.Checked;
			};

			debugToggle.CheckedChange += (e, o) =>
			{
				Settings.DebugMode = debugToggle.Checked == false ? 0 : 1;
			};

			redBar.ProgressChanged += (e, o) =>
			{
				Settings.R = redBar.Progress;	
				imgPreview.SetColorFilter(new Color(redBar.Progress, greenBar.Progress, blueBar.Progress));
			};

			greenBar.ProgressChanged += (e, o) =>
			{
				Settings.G = greenBar.Progress;
				imgPreview.SetColorFilter(new Color(redBar.Progress, greenBar.Progress, blueBar.Progress));
			};

			blueBar.ProgressChanged += (e, o) =>
			{
				Settings.B = blueBar.Progress;
				imgPreview.SetColorFilter(new Color(redBar.Progress, greenBar.Progress, blueBar.Progress));
			};

			sensivityBar.ProgressChanged += (e, o) =>
			{
				Settings.Sensivity = sensivityBar.Progress;
			};

			playerSelect.ProgressChanged += (e, o) =>
			{
				Settings.player = playerSelect.Progress;
				switch (playerSelect.Progress)
				{
					case 0: { Settings.player = 0; txtPlayerSelect.Text = "Player model: Metallic"; } break;
					case 1: { Settings.player = 1; txtPlayerSelect.Text = "Player model: Laser"; } break;
					case 2: { Settings.player = 2; txtPlayerSelect.Text = "Player model: Plank"; } break;
					default: { Settings.player = 0; txtPlayerSelect.Text = "Player model: Metallic"; } break;
				}
			};

			ballSelect.ProgressChanged += (e, o) =>
			{
				Settings.ball = ballSelect.Progress;
				switch (ballSelect.Progress)
				{
					case 0: { Settings.ball = 0; txtBallSelect.Text = "Ball model: Meteor"; } break;
					case 1: { Settings.ball = 1; txtBallSelect.Text = "Ball model: Football"; } break;
					case 2: { Settings.ball = 2; txtBallSelect.Text = "Ball model: Cannonball"; } break;
					default: { Settings.ball = 0; txtBallSelect.Text = "Ball model: Meteor"; } break;
				}
			};

			maxScore.ProgressChanged += (e, o) =>
			{
				Settings.maxScore = maxScore.Progress;
				txtMaxScore.Text = "Max game points: " + maxScore.Progress;
			};
		}

		public void loadSettings()
        {
			switch (Settings.Difficulty)
			{
				case 1: easy.Checked = true; break;
				case 2: normal.Checked = true; break;
				case 3: hard.Checked = true; break;
				default: normal.Checked = true; break;
			}

			paddleToggle.Checked = Settings.RightPaddle;
			debugToggle.Checked = Settings.DebugMode == 0 ? false : true;

			redBar.Progress = Settings.R;
			greenBar.Progress = Settings.G;
			blueBar.Progress = Settings.B;
			playerSelect.Progress = Settings.player;
			ballSelect.Progress = Settings.ball;
			sensivityBar.Progress = Settings.Sensivity;
			maxScore.Progress = Settings.maxScore;

			txtMaxScore.Text = "Max game points: " + maxScore.Progress;

			switch (Settings.player)
			{
				case 0: { playerSelect.Progress = 0; txtPlayerSelect.Text = "Player model: Metallic"; } break;
				case 1: { playerSelect.Progress = 1; txtPlayerSelect.Text = "Player model: Laser"; } break;
				case 2: { playerSelect.Progress = 2; txtPlayerSelect.Text = "Player model: Plank"; } break;
				default: { playerSelect.Progress = 0; txtPlayerSelect.Text = "Player model: Metallic"; } break;
			}

			switch (Settings.ball)
			{
				case 0: { ballSelect.Progress = 0; txtBallSelect.Text = "Ball model: Meteor"; } break;
				case 1: { ballSelect.Progress = 1; txtBallSelect.Text = "Ball model: Football"; } break;
				case 2: { ballSelect.Progress = 2; txtBallSelect.Text = "Ball model: Cannonball"; } break;
				default: { ballSelect.Progress = 0; txtBallSelect.Text = "Ball model: Meteor"; } break;
			}

			imgPreview.SetColorFilter(new Color(redBar.Progress, greenBar.Progress, blueBar.Progress));
		}
	}
}