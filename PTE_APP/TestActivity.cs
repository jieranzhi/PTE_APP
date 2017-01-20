using System.IO;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Net;
using Android.Content.PM;

namespace PTE_APP
{
    [Activity(Label = "TestActivity", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class TestActivity : Activity
    {
        private string type, test_dir, test_name, adv_url;
        int top_lay_id, function_lay_id, adv_lay_id, adv_imagevew_id;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            type = Intent.GetStringExtra("type");
            test_dir = Intent.GetStringExtra("tdir");
            test_name = Intent.GetStringExtra("tname");
            adv_url = Intent.GetStringExtra("turl");
            SetContent();
            InitializeMainViews();
            LoadTestAdvInfo(test_dir);
        }

        public override void OnBackPressed()
        {
            this.Finish();
            OverridePendingTransition(Resource.Animation.Slide_in_from_left, Resource.Animation.Slide_out_to_right);
        }

        private void SetContent()
        {
            SetContentView(Resource.Layout.TestItemType);
            top_lay_id = Resource.Id.linearLayout_testitems_top;
            function_lay_id = Resource.Id.linearLayout_testitems_function;
            adv_lay_id = Resource.Id.linearLayout_testitems_bottom;
            adv_imagevew_id = Resource.Id.imageView_adv;
            LinearLayout ll_func = FindViewById<LinearLayout>(function_lay_id);
            DirectoryInfo dif = new DirectoryInfo(test_dir);
            DirectoryInfo[] difs = dif.GetDirectories();
            foreach (DirectoryInfo df in difs)
            {
                Button btn = new Button(this);
                SetButton(btn, df.Name, df.FullName);
                ll_func.AddView(btn);
            }
        }

        private void SetButton(Button btn, string txt, string tag)
        {
            btn.Text = txt;
            btn.Tag = tag;
            btn.Background = GetDrawable(Resource.Drawable.button_style);
            LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            lp.BottomMargin = 10;
            btn.LayoutParameters = lp;
            btn.SetPadding(10, 10, 10, 10);
        }

        private void InitializeMainViews()
        {
            int[] int_lay_id = new int[] { top_lay_id, function_lay_id, adv_lay_id};
            foreach (int id in int_lay_id)
            {
                LinearLayout ll_lay = FindViewById<LinearLayout>(id);
                for (int i = 0; i < ll_lay.ChildCount; i++)
                {
                    View v = ll_lay.GetChildAt(i);
                    if (v.GetType() == typeof(Button) || v.GetType() == typeof(ImageButton) || v.GetType() == typeof(ImageView))
                    {
                        v.Click += delegate
                        {
                            ButtonOnClick(v);
                        };
                    }
                }
            }
        }

        private void ButtonOnClick(View v)
        {
            if (v.GetType() == typeof(Button))
            {
                var activity2 = new Intent(this, typeof(ListeningTestActivity));
                activity2.PutExtra("tdir", (v as Button).Tag.ToString());
                activity2.PutExtra("tname", this.test_name);
                activity2.PutExtra("type", (v as Button).Text);
                StartActivity(activity2);
                OverridePendingTransition(Resource.Animation.Slide_in_from_right, Resource.Animation.Slide_out_to_left);
            }
            switch (v.Id)
            {
                case Resource.Id.imageView_testmain_logo:
                    {
                        this.Finish();
                        OverridePendingTransition(Resource.Animation.Slide_in_from_left, Resource.Animation.Slide_out_to_right);
                        break;
                    }
                case Resource.Id.imageView_adv:
                    {
                        var uri = Android.Net.Uri.Parse(FindViewById<ImageView>(adv_imagevew_id).Tag.ToString());
                        var intent = new Intent(Intent.ActionView, uri);
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.Slide_in_from_right, Resource.Animation.Slide_out_to_left);
                        break;
                    }
            }
        }

        private void LoadTestAdvInfo(string test_folder)
        {
            // load advertising infomation
            DirectoryInfo dif = new DirectoryInfo(test_folder);
            FileInfo[] fis = dif.GetFiles();
            string adv_image = "";
            foreach (FileInfo f in fis)
            {
                if (f.Extension.ToLower().Contains("png") || f.Extension.ToLower().Contains("jpg") || f.Extension.ToLower().Contains("jpeg")
                    || f.Extension.ToLower().Contains("gif") || f.Extension.ToLower().Contains("bmp"))
                {
                    adv_image = f.FullName;
                    break;
                }
            }
            if (adv_image != "")
            {
                Java.IO.File mf = new Java.IO.File(adv_image);
                Uri adv_uri = Uri.FromFile(mf);
                FindViewById<ImageView>(adv_imagevew_id).SetImageURI(adv_uri);
                FindViewById<ImageView>(adv_imagevew_id).Tag = adv_url;
            }
        }

    }
}