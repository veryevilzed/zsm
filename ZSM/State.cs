using System;
using System.Collections.Generic;

namespace ZSM {
	public interface IState {
		FSM Parent { get; set; }
		FSMData Data { get; }
		string Enter(params object[] args);
		string Do(params object[] args);
		string Exit(params object[] args);
	}

	public class State : IState{

		public FSM Parent { get; set; }

		public FSMData Data { get; protected set; }

		public virtual string Enter(params object[] args) {
			return "";
		}

		public virtual string Do(params object[] args){
			return "";
		}

		public virtual string Exit(params object[] args) {
			return "";
		}

		public State() {
			Data = new FSMData();
		}

		public State(Dictionary<string, object> data) {
			Data = new FSMData(data);
		}
	}

	public class SmartState : IState {

		private Route route;
		public FSM Parent {get; set; }
		public FSMData Data { get; protected set; }



		public virtual string Enter(params object[] args) {
			return "";
		}

		public virtual string Do(params object[] args) {
			if (args.Length > 0)
				return route.Do(args);
			return "";
		}

		public virtual string Exit(params object[] args) {
			return "";
		}

		public SmartState():this(new Dictionary<string, object>()) {}

		public SmartState(Dictionary<string, object> data) {
			Data = new FSMData(data);
			route = new Route(this);
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class StateAttribute : Attribute {
		public string Name { get; set; }
		public StateAttribute(string name){
			this.Name = name;
		}
	}
}

