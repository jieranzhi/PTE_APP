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
using Android.Util;

namespace PTE_APP
{
    class TestItemLayout : LinearLayout
    {
        public string test_name { get; set; }
        public string test_author { get; set; }
        public string test_dir { get; set; }
        public string root_type { get; set; }
        public TestItemLayout(Context context, string tname, string tauthor, string tlogodir, string tdir, string type)
            : base(context)
        {
            Initialize( tname, tauthor, tlogodir, tdir, type);
        }

        private void Initialize(string tname, string tauthor, string tlogodir, string tdir, string type)
        {
            this.test_name = tname;
            this.test_author = tauthor;
            this.test_dir = tdir;
            this.root_type = type; // reading; writing; listening; speaking

            SetBackgroundColor(Android.Graphics.Color.White);
            Inflate(Context, Resource.Layout.TestItem, this);

            var im_logo = FindViewById<ImageView>(Resource.Id.imageView_ti_logo);
            if (tlogodir.Trim() != "")
            {
                im_logo.SetImageURI(Android.Net.Uri.FromFile(new Java.IO.File(tlogodir)));
            }
            var tv_tname = FindViewById<TextView>(Resource.Id.textView_ti_tname);
            tv_tname.Text = tname;
            var tv_tauthor = FindViewById<TextView>(Resource.Id.textView_ti_tauthor);
            tv_tauthor.Text = tauthor;
        }

    }
}