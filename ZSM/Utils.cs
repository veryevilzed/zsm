using System;

namespace ZSM {
	public static class Utils {

		public static object[] RemoveFirst(object[] arg){
			object[] a = new object[arg.Length - 1];
			if (a.Length > 0)
				Array.Copy(arg, 1, a, 0, a.Length);
				//System.Buffer.BlockCopy(arg, 1, a, 0, a.Length);
			return a;
		}

		public static object[] AddFirst(object obj, object[] arg){
			if (arg.Length == 0)
				return new object[] { obj };

			object[] a = new object[arg.Length + 1];
			a[0] = obj;
			Array.Copy(arg, 0, a, 1, arg.Length);
			return a;
		}

		public static T GetEventArgs<T>(this ZEventArgs eventArgs) {
			if (eventArgs.GetType() == typeof(T))
				return (T)(object)eventArgs;
			throw new InvalidCastException();
		}

		public static ZDataEventArgs GetDataEventArgs(this ZEventArgs eventArgs) {
			return eventArgs.GetEventArgs<ZDataEventArgs>();
		}

		public static bool IsSimpleType(this object t) {
			Type type = t.GetType();
			return type.IsPrimitive || type.Equals(typeof(string));
		}
	}
}

