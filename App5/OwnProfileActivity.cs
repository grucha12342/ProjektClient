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
    [Activity(Label = "OwnProfileActivity")]
    public class OwnProfileActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string userPassed = Intent.GetStringExtra("user");
            string passPassed = Intent.GetStringExtra("password");
            string loginPassed = Intent.GetStringExtra("login");
            SetContentView(Resource.Layout.OwnProfileEdit);
            Button buttonBack = FindViewById<Button>(Resource.Id.buttonBack);
            Button buttonUpdate = FindViewById<Button>(Resource.Id.buttonUpdate);
            Button buttonLoadImage = FindViewById<Button>(Resource.Id.buttonLoadImage);
            TextView login = FindViewById<TextView>(Resource.Id.textViewLogin);
            TextView name = FindViewById<TextView>(Resource.Id.textViewName);
            TextView surname = FindViewById<TextView>(Resource.Id.textViewSurname);
            TextView email = FindViewById<TextView>(Resource.Id.textViewMail);
            TextView phone = FindViewById<TextView>(Resource.Id.textViewPhone);
            TextView error = FindViewById<TextView>(Resource.Id.textError);
            EditText passwordText = FindViewById<EditText>(Resource.Id.editTextPassword);
            EditText nameText = FindViewById<EditText>(Resource.Id.editTextName);
            EditText surnameText = FindViewById<EditText>(Resource.Id.editTextSurname);
            EditText phoneText = FindViewById<EditText>(Resource.Id.editTextPhone);
            EditText emailText = FindViewById<EditText>(Resource.Id.editTextEmail);
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

            buttonBack.Click += delegate
            {
                var activity2 = new Intent(this, typeof(LoggedActivity));
                activity2.PutExtra("user", userPassed);
                activity2.PutExtra("password", passPassed);
                StartActivity(activity2);
            };

            buttonLoadImage.Click += delegate
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };

            buttonUpdate.Click += delegate
            {
                try
                {
                    int resultxd;
                    if (string.IsNullOrEmpty(nameText.Text) || string.IsNullOrEmpty(passwordText.Text) || string.IsNullOrEmpty(surnameText.Text))
                    {
                        error.Text = "All fields must be fill";
                        Console.WriteLine("Empty login or password");
                    }
                    else
                    {
                        if (int.TryParse(phoneText.Text, out resultxd) && phoneText.Length() == 9 && emailText.Text.Contains("@"))
                        {
                            urlWithParam = Config.SERVER_ADDRESS + "updateuser.php?&login=" + loginPassed + "&password=" + passwordText.Text +
                                    "&name=" + nameText.Text + "&surname=" + surnameText.Text + "&phone=" + phoneText.Text + "&email=" + emailText.Text;
                            uri = new Uri(urlWithParam);
                            client = new WebClient();
                            result = client.DownloadString(uri);
                            Console.WriteLine(result);
                            if (result.Equals("SUCCESS"))
                            {
                                error.Text = "User updated";
                                var activity2 = new Intent(this, typeof(LoggedActivity));
                                activity2.PutExtra("user", userPassed);
                                activity2.PutExtra("password", passPassed);
                                StartActivity(activity2);
                            }
                            else
                            {
                                error.Text = "Error with database";
                            }
                        }
                        else
                        {
                            error.Text = "Email must contain @ and phone number has 9 numbers";
                        }

                    }
                }
                catch (Exception ex)
                {
                    error.Text = "Server unreachable";
                }
            };
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                var imageView =
                    FindViewById<ImageView>(Resource.Id.imageView1);
                imageView.SetImageURI(data.Data);
                Console.WriteLine("Path " + data.Data.Path);
            }
        }
    }
}