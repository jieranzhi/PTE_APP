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
using Android.Text.Style;

namespace PTE_APP
{
    class MyClickableSpan : ClickableSpan
    {
        public Action<View> Click;

        public override void OnClick(View widget)
        {
            if (Click != null)
                Click(widget);
        }
    }
}