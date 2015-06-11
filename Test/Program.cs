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
				this.Parent.Invoke("state", "ready");
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
				this.Parent.Invoke("log", "ok");
				return base.Enter(args);
			}

			public DoneState() : base(){}
		}

		public class EventFromFSM {
			[FSMEvent("log", "state")]
			public void Log(FSM fms, string eventName, object[] args){
				Console.WriteLine("Event from FSM!! {0} {1}", eventName, args[0]);
			}
		}

		public static void Main2(string[] args) {

			ZSM.FSM fsm = new ZSM.FSM();
			fsm.Add(new InitState());
			fsm.Add(new ReadyState());
			fsm.Add(new DoneState());

			fsm.AddEvents(new EventFromFSM());

			fsm.Strt("init");

			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
			fsm.Do("Hello", "World");
		}

		//static EventManager em = new EventManager();

		public static void DDD(ZEventArgs _args) {
			ZDataEventArgs args = _args.GetDataEventArgs();
			Console.WriteLine("DataChanged Field:{0} From:{1} To:{2}", args.GetDataEventArgs().Key, args.Before, args.Value);
		}

		public struct KeyData {
			public string name;
			public int id;
		}

		public static void Main(string[] _args) {

//			ZData data = new ZData();
//			data.EventManager.AddEvent("change.A", DDD);
//
//			data.Set("A", 5);
//			data.Set("B", 15);
//			data.Inc("A");
//			data.Set("A", null);

			Dictionary<KeyData, string> s = new Dictionary<KeyData, string>();
			s.Add(new KeyData{ name="a", id = 0 }, "a-0");
			s.Add(new KeyData{ name="a", id = 1 }, "a-1");

			Console.WriteLine(s[new KeyData { name="a", id=1}]);

			KeyData a1 = new KeyData{name="1", id=2};
			KeyData a2 = new KeyData{name="1", id=2};

			Console.WriteLine("{0}", a1.Equals(a2));

		}
	}
}
