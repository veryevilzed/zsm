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
				this.Parent.Event("state", "ready");
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
				this.Parent.Event("log", "ok");
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

		static EventManager em = new EventManager();

		public static void DDD(string eventName, object sender, object[] args) {
			Console.WriteLine("Call event *.*.c from event {0}", eventName);
		}


		public static void Main(string[] _args) {
		
//			em.AddEvent("a.b.c", (string eventName, object sender, object[] args) => {
//				Console.WriteLine("Call event a.b.c from event {0}", eventName);
//			});
//
//			em.AddEvent("a.b.*", (string eventName, object sender, object[] args) => {
//				Console.WriteLine("Call event a.b.* from event {0}", eventName);
//			});
//
//			em.AddEvent("a.*.*", (string eventName, object sender, object[] args) => {
//				Console.WriteLine("Call event a.*.* from event {0}", eventName);
//			});
//
//
//			em.AddEvent("a.*.c", (string eventName, object sender, object[] args) => {
//				Console.WriteLine("Call event a.*.c from event {0}", eventName);
//			});
//
//			em.AddEvent("a.*", (string eventName, object sender, object[] args) => {
//				Console.WriteLine("Call event a.* from event {0}", eventName);
//			});

//			em.AddEvent("a.*.b", (string eventName, object sender, object[] args) => {
//				Console.WriteLine("Call event a.*.b from event {0}", eventName);
//			});

			//em.AddEvent("*.*.c", DDD);
			em.AddEvent("a.b.c", DDD);

			em.AddEvent("a.*", (string eventName, object sender, object[] args) => {
				Console.WriteLine("Call event a.*.b from event {0}", eventName);
			});


			em.Invoke("a.b.c", null);

			em.RemoveEvent(DDD);
			Console.WriteLine("-----");
			em.Invoke("a.b.c", null);




		}
	}
}
