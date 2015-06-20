using System;
using System.Collections.Generic;
using ZSM;

namespace Test {
	class MainClass {


		public static void Main(string[] _args) {

			ZData data = new ZData(new Dictionary<string, object>() { {"tmp" , null} });
			data.AddChangeFieldEvent("test", ch);
			data.Set("test", "15");
			data.Set("test", "");
		}

		public static void ch(ZEventArgs args){
			Console.WriteLine("CHANGED");
		}
	}
}
