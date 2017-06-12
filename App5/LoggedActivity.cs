using Android.App;
using Android.Content;
using Android.OS;
using Android.Text.Method;
using Android.Widget;
using App5.Resources;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace App5
{
    [Activity(Label = "Chat Project")]
    public class LoggedActivity : Activity
    {
        TextView chatText;
        TextView nameText;
        HubConnection hubConnection;
        IHubProxy chatHubProxy;
        List<User> connectedUsers;
        ListView userList;
        private readonly static Dictionary<string, StringBuilder> allChats = new Dictionary<string, StringBuilder>();
        string who = "freeGroup";
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            string userPassed = Intent.GetStringExtra("user");
            string passPassed = Intent.GetStringExtra("password");
            Console.WriteLine("User from activity: " + userPassed);
            SetContentView(Resource.Layout.Logged);
            chatText = FindViewById<TextView>(Resource.Id.textChat);
            nameText = FindViewById<TextView>(Resource.Id.textView1);
            userList = FindViewById<ListView>(Resource.Id.listView);
            TextView groupInUse = FindViewById<TextView>(Resource.Id.textViewGroup);
            Button buttonLogout = FindViewById<Button>(Resource.Id.buttonLogout);
            chatText.MovementMethod = new ScrollingMovementMethod();
            chatText.Append("");
            ConnectToServer(userPassed, passPassed);
       
            userList.ItemClick += (sender, args) =>
            {
                string stringFromList = connectedUsers[args.Position].login;
                if(stringFromList.Contains("free"))
                {
                    groupInUse.Text = "You are already connected with freeGroup chat";
                    chatText.Text = allChats["freeGroup"].ToString();
                    who = "freeGroup";
                } else {
                    groupInUse.Text = "You are already connected with " + stringFromList + " chat";
                    chatText.Text = allChats[stringFromList].ToString();
                    who = stringFromList;

                }
            };

            userList.ItemLongClick += (sender, args) =>
            {
                string stringFromList = connectedUsers[args.Position].login;
                if (stringFromList.Equals(userPassed))
                {
                    var activity2 = new Intent(this, typeof(OwnProfileActivity));
                    activity2.PutExtra("user", userPassed);
                    activity2.PutExtra("password", passPassed);
                    activity2.PutExtra("login", stringFromList);
                    StartActivity(activity2);
                    Finish();
                }
                else
                {
                    var activity2 = new Intent(this, typeof(UserProfileActivity));
                    activity2.PutExtra("user", userPassed);
                    activity2.PutExtra("password", passPassed);
                    activity2.PutExtra("login", stringFromList);
                    StartActivity(activity2);
                    Finish();
                }
            };


            FindViewById<Button>(Resource.Id.buttonRefresh).Click += (o, args) =>
            {
                GetUserList();
            };

            buttonLogout.Click += delegate
            {
                try
                {
                    hubConnection.Dispose();
                } catch(Exception ex)
                {
                    Console.WriteLine("Null pointer");
                }
                allChats.Clear();
                var activity2 = new Intent(this, typeof(MainActivity));
                StartActivity(activity2);
            };

        }

        private async void ConnectToServer(string userLog, string passwordLog)
        {
            nameText.Text = "Username: " + userLog;
            hubConnection = new HubConnection(Config.SIGNALR_SERVER_ADDRESS+"signalr", "&name=" + userLog + "&password=" + passwordLog);
            chatHubProxy = hubConnection.CreateHubProxy("MyHub");
            chatHubProxy.On<string, string, string>("addMessage", (user, message, msgTo) =>
            {
                Console.WriteLine(message + " at console");
                if (userLog.Equals(user))
                {
                    chatHandler(msgTo, "You", message, msgTo);
                    chatText.Append("You to " + msgTo + ": " + message + "\n");
                }
                else
                {
                    if (userLog.Equals(msgTo))
                        chatHandler(user, user, message, msgTo);
                    else
                        chatHandler(msgTo, user, message, msgTo);
                    if (who.Equals(msgTo) || who.Equals(user))
                        chatText.Append(user + " to " + msgTo + ": " + message + "\n");
                    else
                        chatText.Append("New message from " + user + " to " + msgTo + "\n");
                }
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
            allChats.Add("freeGroup", new StringBuilder(""));
            GetUserList();
            
            FindViewById<Button>(Resource.Id.buttonSend).Click += async (o, args) =>
            {
                try
                {
                    var messages = FindViewById<EditText>(Resource.Id.editTextMessage).Text;
                    await chatHubProxy.Invoke("SendChatMessage", new object[] { who, userLog, messages });
                    if (!who.Equals("freeGroup"))
                    {
                        chatText.Append("You to " + who + ": " + messages + "\n");
                        chatHandler(who, "You", messages, who);
                    }
                } catch(Exception ex)
                {
                    Console.WriteLine("Server unreachable");
                    chatText.Append("Server unreachable\n");
                }
            };
        }

        private async void GetUserList()
        {
            try
            {
                connectedUsers = await chatHubProxy.Invoke<List<User>>("GetConnectedUsers");
                foreach (User user in connectedUsers)
                {
                    if (allChats.ContainsKey(user.login))
                        Console.WriteLine("Connection is mapped");
                    else
                    {
                        allChats.Add(user.login, new StringBuilder(""));
                        Console.WriteLine("Dictionary count " + allChats.Count);
                    }
                }
                var adapter = new CustomAdapter(this, connectedUsers);
                userList.Adapter = adapter;
            } catch(Exception ex)
            {
                Console.WriteLine("Server unreachable");
                chatText.Append("Server unreachable\n");
            }
        }

        private void chatHandler(string chatId, string user, string message, string who)
        {
             if(chatId.Equals("freeGroup"))
             {
                allChats["freeGroup"].Append(user + " to " + who + ": " + message + "\n");
             } else
             {
                allChats[chatId].Append(user + " to " + who + ": " + message + "\n");
             } 
        }
        protected override void OnDestroy()
        {
            hubConnection.Dispose();
            allChats.Clear();
            base.OnDestroy();
        }
    }
}

