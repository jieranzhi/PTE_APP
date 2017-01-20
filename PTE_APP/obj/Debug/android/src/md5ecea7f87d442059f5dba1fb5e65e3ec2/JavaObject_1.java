package md5ecea7f87d442059f5dba1fb5e65e3ec2;


public class JavaObject_1
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("PTE_APP.JavaObject`1, PTE_APP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", JavaObject_1.class, __md_methods);
	}


	public JavaObject_1 () throws java.lang.Throwable
	{
		super ();
		if (getClass () == JavaObject_1.class)
			mono.android.TypeManager.Activate ("PTE_APP.JavaObject`1, PTE_APP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

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
