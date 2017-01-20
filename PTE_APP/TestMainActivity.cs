using System.IO;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Content.PM;

namespace PTE_APP
{
    [Activity(Label = "TestMainActivity", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class TestMainActivity : Activity
    {
        private int vid;
        private string root_type;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.TestMain);
            vid = Intent.GetIntExtra("vid", 0);
            LoadTestList();
        }

        public override void OnBackPressed()
        {
            this.Finish();
            OverridePendingTransition(Resource.Animation.Slide_in_from_left, Resource.Animation.Slide_out_to_right);
        }

        private void LoadTestList()
        {
            string app_folder = Environment.ExternalStorageDirectory.AbsolutePath + "/" + GetString(Resource.String.App_RootDir);
            string test_folder = "";
            string test_type_title = "PTE Academic Test";

            switch (vid)
            {
                case Resource.Id.imageButton_main_listening:
                    {
                        test_folder = app_folder + "/" + GetString(Resource.String.App_TestDir) + "/" + GetString(Resource.String.App_TestListening);
                        root_type = GetString(Resource.String.App_TestListening).Trim().ToLower();
                        test_type_title += " - " + GetString(Resource.String.App_TestListening);
                        break;
                    }
                case Resource.Id.imageButton_main_reading:
                    {
                        test_folder = app_folder + "/" + GetString(Resource.String.App_TestDir) + "/" + GetString(Resource.String.App_TestReading);
                        root_type = GetString(Resource.String.App_TestReading).Trim().ToLower();
                        test_type_title += " - " + GetString(Resource.String.App_TestReading);
                        break;
                    }
                case Resource.Id.imageButton_main_writting:
                    {
                        test_folder = app_folder + "/" + GetString(Resource.String.App_TestDir) + "/" + GetString(Resource.String.App_TestWritting);
                        root_type = GetString(Resource.String.App_TestWritting).Trim().ToLower();
                        test_type_title += " - " + GetString(Resource.String.App_TestWritting);
                        break;
                    }
                case Resource.Id.imageButton_main_speaking:
                    {
                        test_folder = app_folder + "/" + GetString(Resource.String.App_TestDir) + "/" + GetString(Resource.String.App_TestSpeaking);
                        root_type = GetString(Resource.String.App_TestSpeaking).Trim().ToLower();
                        test_type_title += " - " + GetString(Resource.String.App_TestSpeaking);
                        break;
                    }
            }
            FindViewById<TextView>(Resource.Id.textView_tm_title).Text = test_type_title;
            if (test_folder.Trim() != "")
            {
                DirectoryInfo dif = new DirectoryInfo(test_folder);
                DirectoryInfo[] difs = dif.GetDirectories();
                foreach (DirectoryInfo df in difs)
                {
                    // go over all test folders and list basic info of each test
                    string tname = "";
                    string tauthor = "";
                    string tlogodir = "";
                    string turl = "#";
                    FileInfo[] fis = df.GetFiles();
                    foreach (FileInfo f in fis)
                    {
                        if (f.Name.ToLower().StartsWith("logo"))
                        {
                            tlogodir = f.FullName;
                        }
                        else if (f.Name.ToLower() == "info.txt")
                        {
                            string[] strs = File.ReadAllLines(f.FullName);
                            foreach (string str in strs)
                            {
                                try
                                {
                                    string[] sinfo = str.Split(']');
                                    if (str.Trim().StartsWith("[Author]"))
                                    {
                                        tauthor = sinfo[1].Trim().ToString();
                                    }
                                    else if (str.Trim().StartsWith("[Test_Name]"))
                                    {
                                        tname = sinfo[1].Trim().ToString();
                                    }
                                    else if (str.Trim().StartsWith("[Url]"))
                                    {
                                        turl = sinfo[1].Trim().ToString();
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    TestItemLayout tilayout = new TestItemLayout(this, tname, tauthor, tlogodir, df.FullName, this.root_type);
                    tilayout.Click += delegate
                    {
                        var activity2 = new Intent(this, typeof(TestActivity));
                        activity2.PutExtra("type", this.root_type);
                        activity2.PutExtra("tdir", df.FullName);
                        activity2.PutExtra("tname", tname);
                        activity2.PutExtra("turl", turl);
                        StartActivity(activity2);
                        OverridePendingTransition(Resource.Animation.Slide_in_from_right, Resource.Animation.Slide_out_to_left);
                    };
                    FindViewById<LinearLayout>(Resource.Id.linearLayout_tm_testlist).AddView(tilayout);
                }
                
            }
            else
            {
                Toast.MakeText(this, Resource.String.Warining_TestNotExist, ToastLength.Short).Show();
                return;
            }
        }
    }
}