package md5ecea7f87d442059f5dba1fb5e65e3ec2;


public class MyClickableSpan
	extends android.text.style.ClickableSpan
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onClick:(Landroid/view/View;)V:GetOnClick_Landroid_view_View_Handler\n" +
			"";
		mono.android.Runtime.register ("PTE_APP.MyClickableSpan, PTE_APP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MyClickableSpan.class, __md_methods);
	}


	public MyClickableSpan () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MyClickableSpan.class)
			mono.android.TypeManager.Activate ("PTE_APP.MyClickableSpan, PTE_APP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onClick (android.view.View p0)
	{
		n_onClick (p0);
	}

	private native void n_onClick (android.view.View p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
