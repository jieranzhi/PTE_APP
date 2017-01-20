using System.IO;
using System.Threading;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Android.Content;
using Android.Media;
using Android.Net;
using Android.Text;
using Android.Text.Style;
using static Android.Media.MediaPlayer;
using Android.Content.PM;

namespace PTE_APP
{
    [Activity(Label = "ListeningTestActivity", Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ListeningTestActivity : Activity
    {
        private int tid; // test id
        private string test_dir, test_type, test_name; // test name
        private DirectoryInfo[] quiz_names;
        private FileInfo file_mp3, file_info;
        private int mp3_duration, time_remain;
        private MediaPlayer _player;
        private Timer timer;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.ListeningTest);
            test_dir = Intent.GetStringExtra("tdir");
            test_name = Intent.GetStringExtra("tname");
            test_type = Intent.GetStringExtra("type");

            InitializeMainViews();
            InitializeTimer();
            LoadTest();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_player != null)
            {
                _player.Stop();
                _player.Dispose();
            }
        }

        public override void OnBackPressed()
        {
            this.Finish();
            OverridePendingTransition(Resource.Animation.Slide_in_from_left, Resource.Animation.Slide_out_to_right);
        }

        private void InitializeMainViews()
        {
            // set up click event for each image button
            LinearLayout ll_top = FindViewById<LinearLayout>(Resource.Id.linearLayout_lt2);
            for (int i = 0; i < ll_top.ChildCount; i++)
            {
                View v = ll_top.GetChildAt(i);
                if (v.GetType() == typeof(ImageButton))
                {
                    (v as ImageButton).Click += delegate {
                        BtnOnClick(v);
                    };
                }
            }
            LinearLayout ll_buttons = FindViewById<LinearLayout>(Resource.Id.linearLayout_lt_player);
            for (int i = 0; i < ll_buttons.ChildCount; i++)
            {
                View v = ll_buttons.GetChildAt(i);
                if (v is ImageButton)
                {
                    (v as ImageButton).Click += delegate {
                        BtnOnClick(v);
                    };
                }
            }
            LinearLayout ll_bottom = FindViewById<LinearLayout>(Resource.Id.linearLayout_lt_menu);
            for (int i = 0; i < ll_bottom.ChildCount; i++)
            {
                View v = ll_bottom.GetChildAt(i);
                if (v.GetType() == typeof(ImageButton))
                {
                    (v as ImageButton).Click += delegate {
                        BtnOnClick(v);
                    };
                }
            }
            // setup play imagebutton tag
            FindViewById<ImageButton>(Resource.Id.imageButton_lt_play).SetTag(Resource.Id.imageButton_lt_play, "play");
            FindViewById<TextView>(Resource.Id.textView_lt_title).Text = this.test_name;
        }

        private void LoadTest()
        {
            LoadQuiz();
            LoadQuizByCurrentTid();
            DisplayQuiz();
        }

        private void LoadQuiz()
        {
            DirectoryInfo dif_test = new DirectoryInfo(this.test_dir);
            if (dif_test.Exists)
            {
                quiz_names = dif_test.GetDirectories();
            }
            tid = 0;
        }

        private void LoadQuizByCurrentTid()
        {
            try
            {
                DirectoryInfo dif_quiz = quiz_names[tid];
                FileInfo[] files = dif_quiz.GetFiles();
                FindViewById<TextView>(Resource.Id.textView_lt_title).Text = test_type + " (" + (tid + 1).ToString() + " / " + quiz_names.Length.ToString() + ")";
                foreach (FileInfo f in files)
                {
                    if (f.Name.EndsWith("mp3"))
                    {
                        this.file_mp3 = f;
                        long dur = GetMp3Length(this.file_mp3);
                        this.mp3_duration = (int)dur;
                        SetUpProgressBar((int)dur);
                    }
                    else if (f.Name.EndsWith("xml"))
                    {
                        this.file_info = f;
                    }
                }
                InitializeMediaPlayer();
            }
            catch
            {
                Toast.MakeText(this, Resource.String.Warining_TestNotExist, ToastLength.Short).Show();
            }
        }

        #region Display Test
        private void DisplayQuiz()
        {
            FindViewById<LinearLayout>(Resource.Id.linearLayout_lt_conntent).RemoveAllViews();
            XmlReader xml_reader = XmlReader.Create(this.file_info.FullName);
            string info_type = "", info_tip = "", info_script = "", info_script_visibility = "", info_question = "";
            List<string[]> lst_options = new List<string[]>();

            xml_reader.MoveToContent();
            while (xml_reader.Read())
            {
                if (xml_reader.NodeType == XmlNodeType.Element)
                {
                    if (xml_reader.Name == "Type")
                    {
                        info_type = xml_reader.GetAttribute("Value");
                    }
                    else if (xml_reader.Name == "Tip")
                    {
                        info_tip = xml_reader.GetAttribute("Value");
                    }
                    else if (xml_reader.Name == "Question")
                    {
                        info_question = xml_reader.GetAttribute("Value");
                    }
                    else if (xml_reader.Name == "Option")
                    {
                        string answer = xml_reader.GetAttribute("Answer").Trim();
                        string value = xml_reader.GetAttribute("Value");
                        lst_options.Add(new string[] { value, answer });
                    }
                    else if (xml_reader.Name == "Script")
                    {
                        info_script_visibility = xml_reader.GetAttribute("Visibility");
                        info_script = xml_reader.GetAttribute("Value");
                    }
                }
            }
            ListeningTestContext mc = new ListeningTestContext();
            mc.type = info_type;
            mc.tip = info_tip;
            mc.question = info_question;
            mc.lst_option = lst_options;
            mc.script = info_script;
            mc.script_visibility = info_script_visibility;
            if (info_type.Equals(GetString(Resource.String.TestOption_Multiple_Choice1)) || info_type.Equals(GetString(Resource.String.TestOption_Multiple_Choice2)))
            {
                DisplayMC(mc);
            }
            else if (info_type.Equals(GetString(Resource.String.TestOption_Gap_Fill)))
            {
            }
        }

        private void DisplayCommonView(ListeningTestContext gf)
        {
            FindViewById<TextView>(Resource.Id.textView_lt_tip).Text = gf.tip;
            FindViewById<TextView>(Resource.Id.textView_lt_script).Text = gf.script;
            FindViewById<TextView>(Resource.Id.textView_lt_question).Text = gf.question;
            if (gf.script_visibility.ToLower().Trim().Equals("true"))
            {
                FindViewById<TextView>(Resource.Id.textView_lt_script).Visibility = ViewStates.Visible;
            }
            else
            {
                FindViewById<TextView>(Resource.Id.textView_lt_script).Visibility = ViewStates.Gone;
            }
        }

        private void DisplayMC(ListeningTestContext mc)
        {
            if (mc.type.Equals(GetString(Resource.String.TestOption_Multiple_Choice1)))
            {
                RadioGroup rg = new RadioGroup(this);
                RadioGroup.LayoutParams rg_lp = new RadioGroup.LayoutParams(this, null);
                rg_lp.SetMargins(5, 15, 5, 5);
                rg.LayoutParameters = rg_lp;
                rg.VerticalScrollBarEnabled = true;
                rg.ScrollBarSize = 5;
                foreach (string[] ht in mc.lst_option)
                {
                    RadioButton rb = new RadioButton(this);
                    rb.Tag = ht[1];
                    rb.Text = ht[0];
                    rb.SetTextColor(Android.Graphics.Color.Black);
                    rb.LayoutParameters = rg_lp;
                    rb.Click += delegate
                    {
                        if (rb.Tag.ToString().Equals("True"))
                        {
                            rb.SetTextColor(Android.Graphics.Color.DodgerBlue);
                        }
                        else
                        {
                            rb.SetTextColor(Android.Graphics.Color.Red);
                        }
                    };
                    rg.AddView(rb);
                }
                FindViewById<LinearLayout>(Resource.Id.linearLayout_lt_conntent).AddView(rg);
            }
            else if (mc.type.Equals(GetString(Resource.String.TestOption_Multiple_Choice2)))
            {
                foreach (string[] ht in mc.lst_option)
                {
                    CheckBox rb = new CheckBox(this);
                    rb.Tag = ht[1];
                    rb.Text = ht[0];
                    rb.SetTextColor(Android.Graphics.Color.Black);
                    rb.Click += delegate
                    {
                        if (rb.Tag.ToString().Equals("True"))
                        {
                            rb.SetTextColor(Android.Graphics.Color.DodgerBlue);
                        }
                        else
                        {
                            rb.SetTextColor(Android.Graphics.Color.Red);
                        }
                    };
                    FindViewById<LinearLayout>(Resource.Id.linearLayout_lt_conntent).AddView(rb);
                }
            }
            DisplayCommonView(mc);
        }

        private void DisplayGp(ListeningTestContext gf)
        {
            foreach (string[] ht in gf.lst_option)
            {
                SpannableStringBuilder builder = CreateSpannableStringBuilder(ht, true, "_____", false);
                TextView tv = new TextView(this);
                tv.SetText(builder, TextView.BufferType.Spannable);
                FindViewById<LinearLayout>(Resource.Id.linearLayout_lt_conntent).AddView(tv);
            }
            DisplayCommonView(gf);
        }

        private void DisplayIc(ListeningTestContext gf)
        {
            foreach (string[] ht in gf.lst_option)
            {
                SpannableStringBuilder builder = CreateSpannableStringBuilder(ht, false, "_____", true);
                TextView tv = new TextView(this);
                tv.SetText(builder, TextView.BufferType.Spannable);
                FindViewById<LinearLayout>(Resource.Id.linearLayout_lt_conntent).AddView(tv);
            }
            DisplayCommonView(gf);
        }

        private void DisplayDic(ListeningTestContext dic)
        {
            DisplayCommonView(dic);
        }

        private void DisplaySm(ListeningTestContext sm)
        {
            DisplayCommonView(sm);
        }
        #endregion

        private SpannableStringBuilder CreateSpannableStringBuilder(string[] ht, bool hide, string symbol, bool every_word_clickable)
        {
            string[] answer = ht[1].Split(new char[] { ',' });
            string value = ht[0];
            int idx = value.IndexOf(symbol);
            List<string> lst_script = new List<string>();
            if (every_word_clickable)
            {
                string[] array_script = value.Split(new char[] { ',', ' ', '.', '?', '!', '\r' });
                lst_script.AddRange(array_script);
            }
            else
            {
                while (idx != -1)
                {
                    idx = value.IndexOf(symbol);
                    if (idx != 0)
                    {
                        string temp = value.Substring(0, idx + 1);
                        lst_script.Add(temp);
                    }
                    lst_script.Add(symbol);
                    idx += symbol.Length;
                    value = value.Substring(idx);
                }
            }
            SpannableStringBuilder builder = new SpannableStringBuilder();
            int i = 0;
            foreach (string str in lst_script)
            {
                if (str.Equals(symbol))
                {
                    SpannableString ss = new SpannableString(answer[i]);
                    MyClickableSpan cs = new MyClickableSpan();
                    if (hide)
                    {
                        ss.SetSpan(new BackgroundColorSpan(Android.Graphics.Color.Gray), 0, answer[i].Length, 0);
                        ss.SetSpan(new ForegroundColorSpan(Android.Graphics.Color.Gray), 0, answer[i].Length, 0);
                        cs.Click += delegate
                        {
                            ss.SetSpan(new BackgroundColorSpan(Android.Graphics.Color.DodgerBlue), 0, answer[i].Length, 0);
                            ss.SetSpan(new ForegroundColorSpan(Android.Graphics.Color.White), 0, answer[i].Length, 0);
                        };
                    }
                    else
                    {
                        ss.SetSpan(new ForegroundColorSpan(Android.Graphics.Color.Black), 0, answer[i].Length, 0);
                        cs.Click += delegate
                        {
                            ss.SetSpan(new ForegroundColorSpan(Android.Graphics.Color.DodgerBlue), 0, answer[i].Length, 0);
                        };
                    }
                    ss.SetSpan(cs, 0, answer[i].Length, 0);
                    builder.Append(ss);
                    i++;
                }
                else
                {
                    SpannableString ss = new SpannableString(str);
                    ss.SetSpan(new ForegroundColorSpan(Android.Graphics.Color.Black), 0, str.Length, 0);
                    if (every_word_clickable)
                    {
                        MyClickableSpan cs = new MyClickableSpan();
                        cs.Click += delegate
                        {
                            ss.SetSpan(new ForegroundColorSpan(Android.Graphics.Color.Red), 0, str.Length, 0);
                        };
                        ss.SetSpan(cs, 0, answer[i].Length, 0);
                    }
                    builder.Append(ss);
                }
            }
            
            return builder;
        }

        private long GetMp3Length(FileInfo f)
        {
            MediaMetadataRetriever metaRetriever = new MediaMetadataRetriever();
            metaRetriever.SetDataSource(f.FullName);
            string duration = metaRetriever.ExtractMetadata(MetadataKey.Duration);
            long dur = long.Parse(duration);
            return dur;
        }

        private void InitializeMediaPlayer()
        {
            // initialize media player
            Java.IO.File mf = new Java.IO.File(this.file_mp3.FullName);
            Uri mp3_uri = Uri.FromFile(mf);
            _player = MediaPlayer.Create(this, mp3_uri);
            _player.Completion += delegate {
                timer.Change(Timeout.Infinite, Timeout.Infinite);
                PlayerStop();
            };
        }

        private void InitializeTimer()
        {
            // Create the delegate that invokes methods for the timer.
            TimerCallback timerDelegate = new TimerCallback(UpdateProgress);
            // Create a timer that waits one second, then invokes every second.
            timer = new Timer(timerDelegate, "ready", Timeout.Infinite, Timeout.Infinite);
        }

        private void UpdateProgress(object state)
        {
            this.time_remain -= 1000;
            if (this.time_remain<=0)
            {
                this.time_remain = 0;
            }
            RunOnUiThread(() =>
            {
                FindViewById<ProgressBar>(Resource.Id.progressBar_lt_progress).Progress = this.mp3_duration - this.time_remain;
                string min = (this.time_remain / 60000).ToString();
                string sec = ((this.time_remain % 60000) / 1000).ToString();
                FindViewById<TextView>(Resource.Id.textView_lt_progress).Text = min + ":" + sec;
            });
        }

        private void PlayerStop()
        {
            this.time_remain = this.mp3_duration;
            ImageButton ibtn = this.FindViewById<ImageButton>(Resource.Id.imageButton_lt_play);
            ibtn.SetTag(ibtn.Id, "play");
            ibtn.SetImageResource(Resource.Drawable.play);
            FindViewById<ProgressBar>(Resource.Id.progressBar_lt_progress).Progress = 0;
            string min = (this.mp3_duration / 60000).ToString();
            string sec = ((this.mp3_duration % 60000) / 1000).ToString();
            FindViewById<TextView>(Resource.Id.textView_lt_progress).Text = min + ":" + sec;
        }

        private void SetUpProgressBar(int max)
        {
            ProgressBar pb = FindViewById<ProgressBar>(Resource.Id.progressBar_lt_progress);
            pb.Max = max;
            pb.Progress = 0;
            this.time_remain = max;
            string min = (this.mp3_duration / 60000).ToString();
            string sec = ((this.mp3_duration % 60000) / 1000).ToString();
            FindViewById<TextView>(Resource.Id.textView_lt_progress).Text = min + ":" + sec;
        }

        private void BtnOnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.imageButton_lt_logo:
                    {
                        this.Finish();
                        OverridePendingTransition(Resource.Animation.Slide_in_from_left, Resource.Animation.Slide_out_to_right);
                        break;
                    }
                case Resource.Id.imageButton_lt_play:
                    {
                        if (v.GetTag(v.Id).ToString().Equals("play"))
                        {
                            v.SetTag(v.Id, "pause");
                            (v as ImageButton).SetImageResource(Resource.Drawable.pause);
                            _player.Start();
                            timer.Change(0, 1000);
                        }
                        else
                        {
                            v.SetTag(v.Id, "play");
                            (v as ImageButton).SetImageResource(Resource.Drawable.play);
                            if (_player.IsPlaying)
                            {
                                _player.Pause();
                            }
                            timer.Change(Timeout.Infinite, Timeout.Infinite);
                        }
                        break;
                    }
                case Resource.Id.imageButton_lt_stop:
                    {
                        if (_player.IsPlaying)
                        {
                            PlayerStop();
                            _player.Stop();
                            _player.SeekTo(0);
                            timer.Change(Timeout.Infinite, Timeout.Infinite);
                            InitializeMediaPlayer();
                        }
                        break;
                    }
                case Resource.Id.imageButton_lt_viewscript:
                    {
                        TextView txtv = this.FindViewById<TextView>(Resource.Id.textView_lt_script);
                        txtv.Visibility = (txtv.Visibility == ViewStates.Visible) ? ViewStates.Gone : ViewStates.Visible;
                        break;
                    }
                case Resource.Id.imageButton_lt_previousquiz:
                    {
                        if (tid > 0)
                        {
                            if (_player != null)
                            {
                                if (_player.IsPlaying)
                                {
                                    _player.Stop();
                                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                                    PlayerStop();
                                }
                            }

                            tid--;
                            LoadQuizByCurrentTid();
                            DisplayQuiz();
                        }
                        else
                        {
                            Toast.MakeText(this, Resource.String.Warining_FistofTest, ToastLength.Short).Show();
                        }
                        break;
                    }
                case Resource.Id.imageButton_lt_nextquiz:
                    {
                        if (tid < quiz_names.Length-1)
                        {
                            if (_player != null)
                            {
                                if (_player.IsPlaying)
                                {
                                    _player.Stop();
                                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                                    PlayerStop();
                                }
                            }
                            tid++;
                            LoadQuizByCurrentTid();
                            DisplayQuiz();
                        }
                        else
                        {
                            Toast.MakeText(this, Resource.String.Warining_EndofTest, ToastLength.Short).Show();
                        }
                        break;
                    }
            }
        }
    }
}