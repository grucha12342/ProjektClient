using System;
using Android.App;
using Android.OS;
using Android.Widget;
using System.Net;
using Android.Content;

namespace App5
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Register);
            Button buttonBack = FindViewById<Button>(Resource.Id.buttonBack);
            Button buttonRegister = FindViewById<Button>(Resource.Id.buttonRegister);
            Button buttonImage = FindViewById<Button>(Resource.Id.buttonLoadImage);
            EditText loginText = FindViewById<EditText>(Resource.Id.editTextLogin);
            EditText passwordText = FindViewById<EditText>(Resource.Id.editTextPassword);
            EditText nameText = FindViewById<EditText>(Resource.Id.editTextName);
            EditText surnameText = FindViewById<EditText>(Resource.Id.editTextSurname);
            EditText phoneText = FindViewById<EditText>(Resource.Id.editTextPhone);
            EditText emailText = FindViewById<EditText>(Resource.Id.editTextEmail);
            TextView errorMessage = FindViewById<TextView>(Resource.Id.textError);

            buttonRegister.Click += delegate
            {
                try
                {
                    int resultxd;
                    if (string.IsNullOrEmpty(loginText.Text) || string.IsNullOrEmpty(passwordText.Text))
                    {
                        errorMessage.Text = "Login and password cannot be empty";
                        Console.WriteLine("Empty login or password");
                    }
                    else
                    {
                        if (int.TryParse(phoneText.Text, out resultxd) && phoneText.Length() == 9 && emailText.Text.Contains("@"))
                        {
                            string urlWithParam = Config.SERVER_ADDRESS + "insertuser.php?&login=" + loginText.Text + "&password=" + passwordText.Text +
                                    "&name=" + nameText.Text + "&surname=" + surnameText.Text + "&phone=" + phoneText.Text + "&email=" + emailText.Text;
                            Uri uri = new Uri(urlWithParam);
                            WebClient client = new WebClient();
                            string result = client.DownloadString(uri);
                            Console.WriteLine(result);
                            if (result.Equals("SUCCESS"))
                            {
                                errorMessage.Text = "User created";
                                var activity2 = new Intent(this, typeof(MainActivity));
                                activity2.PutExtra("response", "User created");
                                StartActivity(activity2);
                            }
                            else
                            {
                                errorMessage.Text = "User with that login already exist";
                            }
                        } 
                        else
                        {
                            errorMessage.Text = "Email must contain @ and phone number has 9 numbers";
                        }

                    }
                } catch (Exception ex)
                {
                    errorMessage.Text = "Server unreachable";
                }
            };

            buttonBack.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            buttonImage.Click += delegate {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                StartActivityForResult(
                    Intent.CreateChooser(imageIntent, "Select photo"), 0);
            };
        }

        public override void OnBackPressed() {
            StartActivity(typeof(MainActivity));
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