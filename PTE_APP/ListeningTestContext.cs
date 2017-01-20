using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PTE_APP
{
    class ListeningTestContext
    {
        public string tip { get; set; }
        public string question { get; set; }
        public string script { get; set; }
        public string script_visibility { get; set; }
        public List<string[]> lst_option;
        public string type { get; set; }
        public ListeningTestContext()
        {
            lst_option = new List<string[]>();
        }
    }
}