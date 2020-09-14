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

namespace XamarinPong
{
	[Activity(Label = "Settings", Theme = "@style/AppTheme")]
	public class Settings : Activity
	{
		RadioButton easy, normal, hard;
		ToggleButton audioToggle;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.settings);
			easy = FindViewById<RadioButton>(Resource.Id.radioEasy);
			normal = FindViewById<RadioButton>(Resource.Id.radioNormal);
			hard = FindViewById<RadioButton>(Resource.Id.radioHard);
			audioToggle = FindViewById<ToggleButton>(Resource.Id.toggleAudio);




		}
	}
}