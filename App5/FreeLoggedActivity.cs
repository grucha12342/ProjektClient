using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Microsoft.AspNet.SignalR.Client;
using Android.Text.Method;
using System.Collections.Generic;

namespace App5
{
    [Activity(Label = "FreeLoggedActivity")]
    public class FreeLoggedActivity : Activity
    {
        TextView chatText;
        TextView nameText;
        HubConnection hubConnection;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.FreeUser);
            string userPassed = Intent.GetStringExtra("user");
            chatText = FindViewById<TextView>(Resource.Id.textChat);
            nameText = FindViewById<TextView>(Resource.Id.textView1);
            chatText.MovementMethod = new ScrollingMovementMethod();
            chatText.Append("");
            Button buttonLogout = FindViewById<Button>(Resource.Id.buttonLogout);
            ConnectToServer(userPassed);
            buttonLogout.Click += delegate
            {
                try
                {
                    hubConnection.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Null pointer " + ex.Message);
                }
                var activity2 = new Intent(this, typeof(MainActivity));
                StartActivity(activity2);
            };
        }

        private async void ConnectToServer(string userLog)
        {
    
            nameText.Text = "Username: " + userLog;
            hubConnection = new HubConnection(Config.SIGNALR_SERVER_ADDRESS+"signalr", "&name=" + userLog + "&password=free");
            var chatHubProxy = hubConnection.CreateHubProxy("MyHub");
            chatHubProxy.On<string, string, string>("addMessage", (user, message, who) =>
            {
                Console.WriteLine(message + " at console");
                if (userLog.Equals(user))
                    chatText.Append("You to " + who + ": " + message + "\n");
                else
                    chatText.Append(user + " to " + who + ": " + message + "\n");
            });

            await hubConnection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    Console.WriteLine("Failed to start: {0}", task.Exception.GetBaseException());
                }
                else
                {
                    Console.WriteLine("Success! Connected with client connection id {0}", hubConnection.ConnectionId);
                }
            });
                

            FindViewById<Button>(Resource.Id.buttonSend).Click += async (o, args) =>
            {
                try
                {
                    var who = "freeGroup";
                    var messages = FindViewById<EditText>(Resource.Id.editTextMessage).Text;
                    await chatHubProxy.Invoke("SendChatMessage", new object[] { who, userLog, messages });
                } catch (Exception ex)
                {
                    Console.WriteLine("Server unreachable " + ex.Message);
                    chatText.Append("Server unreachable \n");
                }
            };
        }
    }
}