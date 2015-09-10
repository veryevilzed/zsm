using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ZSM{

	[AttributeUsage(AttributeTargets.Method)]
	public class FSMEvent : System.Attribute {
		public string[] EventNames { get; protected set; }
		public FSMEvent(params string[] eventNames){
			this.EventNames = eventNames;
		}
	}

	public class ZFSMEventArgs : ZEventArgs {
		public FSM FSM { get {return (FSM)this.Sender; } }
		public ZFSMEventArgs(string eventName, FSM sender, string stateName):base(string.Format("fsm.{0}",eventName), sender, stateName) { }
	}


	public class FSM {
	
		public Dictionary<string, IState> States { get; protected set; }

		public EventManager Events {get; protected set;}

		public ZData Data {get; protected set;}

		public IState CurrentState { get; protected set; }

		public string StateName {get;set;}

		public void Add(IState state) {
			foreach (object a in state.GetType().GetCustomAttributes(true)) {
				if (a.GetType() == typeof(StateAttribute)) {
					this.Add(((StateAttribute)a).Name, state);
					return;
				}
			}

			this.Add(state.GetType().Name, state);
		}

		public void AddEvents(object fromTarget){
			
			foreach (System.Reflection.MethodInfo mi in fromTarget.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)) {
				foreach (object o in mi.GetCustomAttributes(true)) {
					if (o.GetType() == typeof(FSMEvent)) {
						foreach (string eventName in ((FSMEvent)o).EventNames) {
							this.AddEvent(eventName, (DZEvent)Delegate.CreateDelegate(typeof(DZEvent), fromTarget, mi, true));
						}
					}
				}
			}
		}

		public void AddEvent(string eventName, DZEvent listener){
			this.Events.AddEvent(string.Format("{0}", eventName), listener);
		}

		public void RemoveEvent(DZEvent listener){
			Events.RemoveEvent(listener);
		}

		public void Invoke(string eventName, params object[] args){
			this.Events.Invoke(new ZEventArgs(eventName, this, args));
		}

		public void SendEvent(string eventName, params object[] args){
			this.Do(eventName, new ZEventArgs(eventName, this, args));
		}

		public void Add(string name, IState state) {
			state.Parent = this;
			this.States.Add(name, state);
		}



		public virtual void Do(params object[] args){
			if (CurrentState == null)
				return;

			string newState = CurrentState.Do(args);
			if (newState != "")
				ExitState(newState, args);
		}

		public virtual void ChangeState(string newState, params object[] args){
			ExitState(newState, args);
		}

		public void Update(float deltaTime) {
			if (this.CurrentState != null)
				this.CurrentState.Update(deltaTime);
		}

		protected void ExitState(string newState, params object[] args) {
			if (CurrentState != null) {
				this.Events.Invoke(new ZFSMEventArgs("fsm.exit", this, StateName));
				string _newState = CurrentState.Exit(args);
				CurrentState = null;
				if (_newState != "")
					EnterState(_newState, args);
				else 
					EnterState(newState, args);
			}
		}

		protected void EnterState(string newState, params object[] args) {
			StateName = newState;
			CurrentState = States[newState];
			this.Events.Invoke(new ZFSMEventArgs("fsm.enter", this, StateName));
			string _newState = CurrentState.Enter(args);
			if (_newState != "")
				ExitState(_newState, args);
		}

		public void Start(string state){
			if (CurrentState == null)
				EnterState(state);
		}

		public FSM() : this(new System.Collections.Generic.Dictionary<string, object>()) {}

		public FSM(System.Collections.Generic.Dictionary<string, object> data) {
			States = new Dictionary<string, IState>();
			Events = new EventManager();
			Data = new ZData(data);
		}

	}




}

