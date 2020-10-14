using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Widget;

namespace XamarinPong
{
    [Activity(Label = "HostActivity")]
    public class HostActivity : Activity
    {
        //Items
        TextView topText, IPview;
        EditText portText;
        Button hostButton;
        TcpClient client;
        TcpListener server;

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "NULL";
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.host);

            topText = FindViewById<TextView>(Resource.Id.textViewHosting);
            IPview = FindViewById<TextView>(Resource.Id.textViewIP);
            portText = FindViewById<EditText>(Resource.Id.editPort);
            hostButton = FindViewById<Button>(Resource.Id.btnStartHost);
            IPview.Text = "IP:     " + GetLocalIPAddress();

            hostButton.Click += async (e, o) =>
            {
                var cts = new CancellationTokenSource();
                cts.Token.Register(() => server.Stop());
                
                AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
                alertDiag.SetTitle("Hosting.");
                alertDiag.SetMessage(" Waiting for player to connect...");
                alertDiag.SetNegativeButton("Cancel", (senderAlert, args) => {
                    cts.Cancel();
                });
                server = new TcpListener(IPAddress.Any, int.Parse(portText.Text));
                server.Start();
                Dialog diag = alertDiag.Create();
                diag.Show();

                try
                {
                    client = await Task.Run(() => server.AcceptTcpClientAsync(), cts.Token);
                }
                catch (ObjectDisposedException) when (cts.IsCancellationRequested)
                {
                    Finish();
                }
                alertDiag.Dispose();

                MultiplyerPong.NetStream = client.GetStream();
                MultiplyerPong.isHost = true;
                Intent multiplayer = new Intent(this, typeof(MultiplyerPong));
                StartActivity(multiplayer);
            };
        }
    }
}