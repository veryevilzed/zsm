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
			Console.WriteLine(" --- *** TEST *** ---");
			em.RemoveEvent(test);
		}

		public static void Main(string[] _args) {
			em = new EventManager();
			em.AddEvent("test", test);
			em.Invoke("test");
			em.Invoke("test");
		}
	}
}
