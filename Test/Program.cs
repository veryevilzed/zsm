using System;
using System.Collections.Generic;
using ZSM;

namespace Test {

	class StateA :SmartState {
		public override string Enter(params object[] args){
			Console.WriteLine("ENTER A");
			return "";
		}

		public override string Exit(params object[] args){
			Console.WriteLine("EXIT A");
			return "";
		}
	}

	class StateB :SmartState {
		public override string Enter(params object[] args){
			Console.WriteLine("ENTER B");
			return "";
		}

		public override string Exit(params object[] args){
			Console.WriteLine("EXIT B");
			return "";
		}

	}

	class MainClass {

		static EventManager em;

		static void test(ZEventArgs arg){
			Console.WriteLine(" --- *** TEST(ARG) *** ---");
			em.RemoveEvent(test);
		}

		static void test2(){
			Console.WriteLine(" --- *** TEST() *** ---");
			em.RemoveAction(test2);
		}

		public static void Main(string[] _args) {
			em = new EventManager();
			em.AddEvent("test", test );
			em.AddAction("test", test2 );
			em.Invoke("test");
			em.Invoke("test");

//			LoadingCache c = new LoadingCache();
//			c.LifeTime = 1;
//
//			c.New = delegate(string key) {
//				return 5;
//			};
//
//			c.Destroy = delegate(string key) {
//				return true;
//			};

//			c.Refresh = delegate(string key) {
//				return 6;
//			};

//			Console.WriteLine("{0}", c["test"]);
//			System.Threading.Thread.Sleep(1001);
//			Console.WriteLine("{0}", c["test"]);

		}
	}
}
