using System;

namespace ZSM {
	public static class Utils {

		public static object[] Trim(object[] arg){
			object[] a = new object[arg.Length - 1];
			if (a.Length > 0)
				Array.Copy(arg, 1, a, 0, a.Length);
				//System.Buffer.BlockCopy(arg, 1, a, 0, a.Length);
			return a;
		}
	}
}

