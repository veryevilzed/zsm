using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ZSM{

	public delegate void DFSMEvent(FSM fms, string eventName, object[] args);

	[AttributeUsage(AttributeTargets.Method)]
	public class FSMEvent : System.Attribute {
		public string[] EventNames { get; protected set; }
		public FSMEvent(params string[] eventNames){
			this.EventNames = eventNames;
		}
	}

	public class FSM {

		public Dictionary<string, IState> states;
		public Dictionary<string, List<DFSMEvent>> events;

		public IState CurrentState { get; protected set; }

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
							this.AddEvent(eventName, (DFSMEvent)Delegate.CreateDelegate(typeof(DFSMEvent), fromTarget, mi, true));
						}
					}
				}
			}
		}

		public void AddEvent(string eventName, DFSMEvent target){
			if (!this.events.ContainsKey(eventName))
				this.events.Add(eventName, new List<DFSMEvent>());	
				
			this.events[eventName].Add(target);
			
		}

		public void RemoveEvent(string eventName, DFSMEvent target){
			if (!this.events.ContainsKey(eventName))
				return;
			this.events[eventName].Remove(target);
			if (this.events[eventName].Count == 0)
				this.events.Remove(eventName);
		}

		public void Event(string eventName, params object[] args){
			if (this.events.ContainsKey(eventName))
				foreach (DFSMEvent target in this.events[eventName])
					target.Invoke(this, eventName, args);
			if (this.events.ContainsKey("*"))
				foreach (DFSMEvent target in this.events["*"])
					target.Invoke(this, eventName, args);
		}

		public void Add(string name, IState state) {
			state.Parent = this;
			this.states.Add(name, state);
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

		protected void ExitState(string newState, params object[] args) {
			if (CurrentState != null) {
				string _newState = CurrentState.Exit(args);
				CurrentState = null;
				if (_newState != "")
					EnterState(_newState, args);
				else 
					EnterState(newState, args);
			}
				
		}

		protected void EnterState(string newState, params object[] args) {
			CurrentState = states[newState];
			string _newState = CurrentState.Enter(args);
			if (_newState != "")
				ExitState(_newState, args);
		}

		public void StrtState(string state){
			if (this.CurrentState == null)
				EnterState(state);
		}

		public FSM() {
			states = new Dictionary<string, IState>();
			events = new Dictionary<string, List<DFSMEvent>>();
		}

	}




}

