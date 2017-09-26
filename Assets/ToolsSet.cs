using System;


public class ToolsSet
{
	public ToolsSet ()
	{
	}
	public static string GetRandomFileName (int num)
	{
		System. Random r = new System.Random ();
		string s = string.Empty;
		string str = string.Empty;
		for (int i = 0; i < num; i++) {
			s = ((char)r.Next (97, 123)).ToString ();
			str += s;
		}
		return str;
	}

}
