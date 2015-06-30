using System;
using System.Collections.Generic;

namespace ZSM {
	public interface IState {
		FSM Parent { get; set; }
		ZData Data { get; }
		string Enter(params object[] args);
		string Do(params object[] args);
		string Exit(params object[] args);
		void Update(float deltaTime);
	}

	public class State : IState{

		public FSM Parent { get; set; }

		public ZData Data { get; protected set; }

		public virtual string Enter(params object[] args) {
			return "";
		}

		public virtual string Do(params object[] args){
			return "";
		}

		public virtual string Exit(params object[] args) {
			return "";
		}

		public virtual void Update(float deltaTime) {
		}

		public State() {
			Data = new ZData();
		}

		public State(Dictionary<string, object> data) {
			Data = new ZData(data);
		}
	}

	public class SmartState : IState {

		private Route route;
		public FSM Parent {get; set; }
		public ZData Data { get; protected set; }

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

		public virtual void Update(float deltaTime) {
		}

		public SmartState():this(new Dictionary<string, object>()) {}

		public SmartState(Dictionary<string, object> data) {
			Data = new ZData(data);
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

