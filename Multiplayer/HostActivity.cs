using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Android.App;
using Android.Content;
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

            hostButton.Click += (e, o) =>
            {
                AlertDialog.Builder alertDiag = new AlertDialog.Builder(this);
                alertDiag.SetTitle("Hosting.");
                alertDiag.SetMessage(" Waiting for player to connect...");

                TcpListener server = new TcpListener(IPAddress.Any, int.Parse(portText.Text));
                server.Start();
                System.Threading.Thread serverThread = new System.Threading.Thread(() =>
                {
                    client = server.AcceptTcpClient();
                    MultiplyerPong.NetStream = client.GetStream();
                    MultiplyerPong.isHost = true;
                    alertDiag.Dispose();
                });
                serverThread.Start();
 
                alertDiag.SetNegativeButton("Cancel", (senderAlert, args) => {
                    serverThread.Abort();
                    server.Stop();
                    alertDiag.Dispose();
                });
                Dialog diag = alertDiag.Create();
                diag.Show();
            };
        }
    }
}