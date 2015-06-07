using System;
using System.Collections.Generic;
using ZSM;

namespace Test {
	class MainClass {

		[ZSM.StateAttribute("init")]
		class InitState : State {
			public override string Enter(params object[] args) {
				Console.WriteLine("Init");
				this.Parent.ChangeState("ready");
				return "";
			}

			public override string Exit(params object[] args) {
				Console.WriteLine("Exit Init");
				return "";
			}
		}

		[ZSM.StateAttribute("ready")]
		class ReadyState : SmartState {

			public override string Enter(params object[] args) {
				Console.WriteLine("Ready");
				return base.Enter(args);
			}

			public string Hello(string name) {
				Data.Inc("id");
				Console.WriteLine(string.Format("{0} {1}",Data["id"], name));
				if (Data.GetInt("id", 0) >= 3)
					return "done";
				return "";
			}

			public ReadyState() : base(new Dictionary<string, object>(){ { "id" , 1 } }){}
		}

		[ZSM.StateAttribute("done")]
		class DoneState : SmartState {

			public override string Enter(params object[] args) {
				Console.WriteLine("Done");
				return base.Enter(args);
			}

			public DoneState() : base(){}
		}



		public static void Main(string[] args) {

			ZSM.FSM fsm = new ZSM.FSM();
			fsm.Add(new InitState());
			fsm.Add(new ReadyState());
			fsm.Add(new DoneState());
			fsm.StrtState("init");

			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
		}
	}
}
