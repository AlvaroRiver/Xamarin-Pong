using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Graphics.Drawable;
using Android.Views;
using Android.Widget;

namespace MonoGameTest
{
	[Activity(Label = "Settings", Theme = "@style/AppTheme")]
	public class SettingsActivity : Activity
	{
		RadioButton easy, normal, hard;
		ToggleButton audioToggle;
		SeekBar redBar, greenBar, blueBar;
		ImageView imgPreview;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.settings);
			easy = FindViewById<RadioButton>(Resource.Id.radioEasy);
			normal = FindViewById<RadioButton>(Resource.Id.radioNormal);
			hard = FindViewById<RadioButton>(Resource.Id.radioHard);
			audioToggle = FindViewById<ToggleButton>(Resource.Id.toggleAudio);
			redBar = FindViewById<SeekBar>(Resource.Id.seekBarRed);
			greenBar = FindViewById<SeekBar>(Resource.Id.seekBarGreen);
			blueBar = FindViewById<SeekBar>(Resource.Id.seekBlue);
			imgPreview = FindViewById<ImageView>(Resource.Id.imgThemePreview);


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

			audioToggle.CheckedChange += (e, o) =>
			{
				if (audioToggle.Checked)
					Settings.Audio = true;
				else Settings.Audio = false;
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

		}
	}
}