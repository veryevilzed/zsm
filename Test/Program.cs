using System;
using System.Collections.Generic;
using ZSM;

namespace Test {
	class MainClass {


		public static void Main(string[] _args) {

			ZData data = new ZData();
			data.EventManager.AddEvent("*", ev);
			data.AddChangeFieldEvent("test", ch);
			data.AddChangeFieldEvent("tmp", ch2);
			//ZList<int> iii=  data.GetZList<int>("test");
			//Console.WriteLine("{0}", iii);
			data.Set("tmp", 5555);
			data.Set("tmp", 6666);
		}

		public static void ch(ZEventArgs args){
			Console.WriteLine("CHANGED");
		}

		public static void ch2(ZEventArgs args){
			Console.WriteLine("CHANGED TMP");
		}

		public static void ev(ZEventArgs args){
			Console.WriteLine("e:{0}",args.EventName);
		}


	}
}
