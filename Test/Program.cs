﻿using System;
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

		static FSM f;

		public static void Main(string[] _args) {
			f = new FSM();
			f.Add(new StateA());
			f.Add(new StateB());
			f.Start("StateA");
			Console.ReadKey();
			f.ChangeState("StateB");
		}



	}
}
