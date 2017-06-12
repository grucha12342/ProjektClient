using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace App5.Resources
{
    public class ViewHolder : Java.Lang.Object
    {
        public TextView txtName { get; set; }
        public TextView txtAge { get; set; }
        public TextView txtEmail { get; set; }
    }
    public class CustomAdapter : BaseAdapter
    {
        private Activity activity;
        private List<User> persons;

        public CustomAdapter(Activity activity, List<User> persons)
        {
            this.activity = activity;
            this.persons = persons;
        }
        public override int Count
        {
            get
            {
                return persons.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return persons[position].idUser;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? activity.LayoutInflater.Inflate(Resource.Layout.list_view_dataTemplate, parent, false);

            var txtName = view.FindViewById<TextView>(Resource.Id.textView1);
            var txtAge = view.FindViewById<TextView>(Resource.Id.textView2);
            var txtEmail = view.FindViewById<TextView>(Resource.Id.textView3);

            txtName.Text = persons[position].login;
            txtAge.Text = "" + persons[position].name;
            txtEmail.Text = persons[position].surname;

            return view;

        }
    }
}