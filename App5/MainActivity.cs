using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Net;

namespace App5
{
    [Activity(Label = "Chat Project", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        EditText loginText;
        EditText passwordText;
        TextView errorMessage;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            string response = Intent.GetStringExtra("response");
            SetContentView(Resource.Layout.Main);
            Button buttonRegister = FindViewById<Button>(Resource.Id.buttonRegister);
            Button buttonLogin = FindViewById<Button>(Resource.Id.buttonLogin);
            Button buttonFree = FindViewById<Button>(Resource.Id.buttonFree);
            loginText = FindViewById<EditText>(Resource.Id.editTextLogin);
            passwordText = FindViewById<EditText>(Resource.Id.editTextPassword);
            errorMessage = FindViewById<TextView>(Resource.Id.textViewError);
            errorMessage.Text = response;
            buttonRegister.Click += delegate {
                StartActivity(typeof(RegisterActivity));
            };

            buttonLogin.Click += async delegate
            {
                try
                {
                    if (String.IsNullOrEmpty(loginText.Text) || String.IsNullOrEmpty(passwordText.Text))
                    {
                        errorMessage.Text = "Login or password is empty";
                    }
                    else
                    {
                        string urlWithParam = Config.SERVER_ADDRESS + "checklogin.php?&login=" + loginText.Text + "&password=" + passwordText.Text;
                        Console.WriteLine("GET URL " + urlWithParam);
                        Uri uri = new Uri(urlWithParam);
                        WebClient client = new WebClient();
                        string result = await client.DownloadStringTaskAsync(uri);
                        Console.WriteLine(result);
                        if (result.Equals("SUCCESS"))
                        {
                            var activity2 = new Intent(this, typeof(LoggedActivity));
                            activity2.PutExtra("user", loginText.Text);
                            activity2.PutExtra("password", passwordText.Text);
                            StartActivity(activity2);
                        }
                        else
                        {
                            Console.WriteLine("Login was not succesfull");
                            errorMessage.Text = "Wrong login or password";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    errorMessage.Text = "Server unreachable";
                }
            };
            buttonFree.Click += delegate
            {
                if (String.IsNullOrEmpty(loginText.Text))
                {
                    errorMessage.Text = "Please fill your name (login)";
                }
                else
                {
                    var activity2 = new Intent(this, typeof(FreeLoggedActivity));
                    activity2.PutExtra("user", loginText.Text);
                    StartActivity(activity2);
                }
            };
        }
    }
}