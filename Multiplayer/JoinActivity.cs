using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace XamarinPong
{
    [Activity(Label = "JoinActivity")]
    public class JoinActivity : Activity
    {
        //Items
        TextView IPview;
        EditText portText, IPtext;
        Button joinButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.join);

            IPview = FindViewById<TextView>(Resource.Id.textViewIPJoin);
            portText = FindViewById<EditText>(Resource.Id.editPortJoin);
            IPtext = FindViewById<EditText>(Resource.Id.editIPJoin);
            joinButton = FindViewById<Button>(Resource.Id.btnJoinGame);
            IPview.Text = "IP: ";

            joinButton.Click += (e, o) =>
            {
                TcpClient client = new TcpClient();
                client.Connect(IPAddress.Parse(IPtext.Text), int.Parse(portText.Text));
                MultiplyerPong.NetStream = client.GetStream();
                MultiplyerPong.isHost = false;
                Intent multiplayer = new Intent(this, typeof(MultiplyerPong));
                StartActivity(multiplayer);
            };
        }
    }
}