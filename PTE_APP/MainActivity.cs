using System.IO;
using System.Threading.Tasks;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Net;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Android.Views.Animations;
using Android.Content.PM;

namespace PTE_APP
{
    [Activity(Label = "PTE_APP", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            InitializeMainViews();
            InitializeApp();
        }

        private void InitializeMainViews()
        {
            // get the function layout
            LinearLayout ll_buttons = FindViewById<LinearLayout>(Resource.Id.linearLayout_main_funcion);
            for (int i = 0; i < ll_buttons.ChildCount; i++)
            {
                View v = ll_buttons.GetChildAt(i);
                if (v.GetType() == typeof(ImageButton))
                {
                    v.Touch += (s, e) => {
                        OnTouch(s, e, v);
                    };
                }
            }
            var dropAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.scale_bounce_from_top);
            FindViewById<ImageView>(Resource.Id.imageView_main_upside).StartAnimation(dropAnimation);
            FindViewById<LinearLayout>(Resource.Id.linearLayout_main_funcion).StartAnimation(dropAnimation);
            var upAnimation = AnimationUtils.LoadAnimation(this, Resource.Animation.scale_bounce_from_center);
            FindViewById<ImageView>(Resource.Id.imageView_main_middle).StartAnimation(upAnimation);
        }

        private void InitializeApp()
        {
            if (CheckExternalStorageState())
            {
                CheckAppFolder();
                if (!CheckSampleTest("listening"))
                {
                    UnzipSampleTest("listening");
                }
                // add reading/ writting/ speaking later
            }
            else
            {
                Toast.MakeText(this, Resource.String.Warining_SDFail, ToastLength.Short).Show();
            }
        }

        private void CheckAppFolder()
        {
            string app_folder = Environment.ExternalStorageDirectory.AbsolutePath + "/" + GetString(Resource.String.App_RootDir);
            string test_folder = app_folder + "/" + GetString(Resource.String.App_TestDir);
            string reading_folder = test_folder + "/" + GetString(Resource.String.App_TestReading);
            string writting_folder = test_folder + "/" + GetString(Resource.String.App_TestWritting);
            string speaking_folder = test_folder + "/" + GetString(Resource.String.App_TestSpeaking);
            string listening_folder = test_folder + "/" + GetString(Resource.String.App_TestListening);
            CheckFolder(app_folder);
            CheckFolder(test_folder);
            CheckFolder(reading_folder);
            CheckFolder(writting_folder);
            CheckFolder(speaking_folder);
            CheckFolder(listening_folder);
        }

        private void CheckFolder(string folder)
        {
            DirectoryInfo dif_app = new DirectoryInfo(folder);
            if (!dif_app.Exists)
            {
                dif_app.Create();
            }
        }

        private bool CheckSampleTest(string type)
        {
            string app_folder = Environment.ExternalStorageDirectory.AbsolutePath + "/" + GetString(Resource.String.App_RootDir);
            string test_folder = app_folder + "/" + GetString(Resource.String.App_TestDir) + "/" + type +"/sample_test";
            DirectoryInfo dif_app = new DirectoryInfo(test_folder);
            return dif_app.Exists;
        }

        private void UnzipSampleTest(string type)
        {
            // Application.Context.Assets.
            string app_folder = Environment.ExternalStorageDirectory.AbsolutePath + "/" + GetString(Resource.String.App_RootDir);
            string test_folder = app_folder + "/" + GetString(Resource.String.App_TestDir) + "/" + type;

            Stream zipstream = this.Assets.Open("sample_"+type+"_test.zip");
            UnzipFromStream(zipstream, test_folder);
        }

        public async void UnzipFromStream(Stream zipStream, string outFolder)
        {
            ZipInputStream zipInputStream = new ZipInputStream(zipStream);
            ZipEntry zipEntry = zipInputStream.GetNextEntry();
            await Task.Run(delegate
            {
                while (zipEntry != null)
                {
                    string entryFileName = zipEntry.Name;
                    // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                    // Optionally match entrynames against a selection list here to skip as desired.
                    // The unpacked length is available in the zipEntry.Size property.

                    byte[] buffer = new byte[4096];     // 4K is optimum

                    // Manipulate the output filename here as desired.
                    string fullZipToPath = Path.Combine(outFolder, entryFileName);
                    string directoryName = Path.GetDirectoryName(fullZipToPath);
                    if (directoryName.Length > 0)
                        Directory.CreateDirectory(directoryName);

                    // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                    // of the file, but does not waste memory.
                    // The "using" will close the stream even if an exception occurs.
                    if (zipEntry.IsFile)
                    {
                        using (FileStream streamWriter = File.Create(fullZipToPath))
                        {
                            StreamUtils.Copy(zipInputStream, streamWriter, buffer);
                        }
                    }
                    zipEntry = zipInputStream.GetNextEntry();
                }
            });
        }

        private void OnTouch( object s , Android.Views.View.TouchEventArgs e, View v)
        {
            int touch_up_image_id = Resource.Drawable.listening;
            int touch_down_image_id = Resource.Drawable.listening_on;
            switch (v.Id)
            {
                case Resource.Id.imageButton_main_listening:
                    {
                        touch_up_image_id = Resource.Drawable.listening;
                        touch_down_image_id = Resource.Drawable.listening_on;
                        break;
                    }
                case Resource.Id.imageButton_main_reading:
                    {
                        touch_up_image_id = Resource.Drawable.reading;
                        touch_down_image_id = Resource.Drawable.reading_on;
                        break;
                    }
                case Resource.Id.imageButton_main_speaking:
                    {
                        touch_up_image_id = Resource.Drawable.speaking;
                        touch_down_image_id = Resource.Drawable.speaking_on;
                        break;
                    }
                case Resource.Id.imageButton_main_writting:
                    {
                        touch_up_image_id = Resource.Drawable.writting;
                        touch_down_image_id = Resource.Drawable.writting_on;
                        break;
                    }
            }
            if (e.Event.Action == MotionEventActions.Down)
            {
                (v as ImageButton).SetImageResource(touch_down_image_id);    
            }
            else if (e.Event.Action == MotionEventActions.Up)
            {
                (v as ImageButton).SetImageResource(touch_up_image_id);
                var activity2 = new Intent(this, typeof(TestMainActivity));
                activity2.PutExtra("vid", v.Id);
                StartActivity(activity2);
                OverridePendingTransition(Resource.Animation.Slide_in_from_right, Resource.Animation.Slide_out_to_left);
            }
        }

        private bool CheckExternalStorageState()
        {
            bool mExternalStorageAvailable = false;
            bool mExternalStorageWriteable = true;
            string state = Environment.ExternalStorageState;
            if (Environment.MediaMounted.Equals(state))
            {
                mExternalStorageAvailable = true;
            }
            else if (Environment.MediaMountedReadOnly.Equals(state))
            {
                mExternalStorageAvailable = true;
                mExternalStorageWriteable = false;
            }
            else
            {
                mExternalStorageAvailable = mExternalStorageWriteable = false;
            }
            return mExternalStorageAvailable && mExternalStorageWriteable;
        }
    }
}

