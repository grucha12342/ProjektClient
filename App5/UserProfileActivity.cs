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
using System.Net;
using Newtonsoft.Json;

namespace App5
{
    [Activity(Label = "UserProfileActivity")]
    public class UserProfileActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string userPassed = Intent.GetStringExtra("user");
            string passPassed = Intent.GetStringExtra("password");
            string loginPassed = Intent.GetStringExtra("login");
            SetContentView(Resource.Layout.UserProfile);
            Button buttonBack = FindViewById<Button>(Resource.Id.buttonBack);
            Button buttonCall = FindViewById<Button>(Resource.Id.buttonCall);
            Button buttonMail = FindViewById<Button>(Resource.Id.buttonMail);
            TextView login = FindViewById<TextView>(Resource.Id.textViewLogin);
            TextView name = FindViewById<TextView>(Resource.Id.textViewName);
            TextView surname = FindViewById<TextView>(Resource.Id.textViewSurname);
            TextView email = FindViewById<TextView>(Resource.Id.textViewEmail);
            TextView phone = FindViewById<TextView>(Resource.Id.textViewPhone);
            string urlWithParam = Config.SERVER_ADDRESS + "selectuser.php?&login=" + loginPassed;
            Console.WriteLine("GET URL " + urlWithParam);
            Uri uri = new Uri(urlWithParam);
            WebClient client = new WebClient();
            string result = await client.DownloadStringTaskAsync(uri);
            User user = JsonConvert.DeserializeObject<User>(result);
            login.Text = "Login: " + user.login;
            name.Text = "Name: " + user.name;
            surname.Text = "Surname: " + user.surname;
            email.Text = "Email: " + user.email;
            phone.Text = "Phone: " + user.phoneNumber;

            buttonMail.Click += (object sender, EventArgs e) =>
            {
                var emailIntent = new Intent(Android.Content.Intent.ActionSend);
                emailIntent.PutExtra(Android.Content.Intent.ExtraEmail, new[] { user.email});
                emailIntent.PutExtra(Android.Content.Intent.ExtraCc, new[] { user.email });
                emailIntent.PutExtra(Android.Content.Intent.ExtraSubject, "Messege from app: "+ userPassed);
                emailIntent.SetType("text/plain");
                emailIntent.PutExtra(Android.Content.Intent.ExtraText, "e-mail body");
                StartActivity(Intent.CreateChooser(emailIntent, "Send e-mail"));
            };

            buttonCall.Click += (object sender, EventArgs e) =>
            {
                // On "Call" button click, try to dial phone number.
                var callDialog = new AlertDialog.Builder(this);
                callDialog.SetMessage("Call " + user.phoneNumber + "?");
                callDialog.SetNeutralButton("Call", delegate {
                    // Create intent to dial phone
                    var callIntent = new Intent(Intent.ActionCall);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + user.phoneNumber));
                    StartActivity(callIntent);
                });
                callDialog.SetNegativeButton("Cancel", delegate { });

                // Show the alert dialog to the user and wait for response.
                callDialog.Show();
            };

            buttonBack.Click += delegate
            {
                var activity2 = new Intent(this, typeof(LoggedActivity));
                activity2.PutExtra("user", userPassed);
                activity2.PutExtra("password", passPassed);
                StartActivity(activity2);
            };
            // Create your application here
        }
    }
}