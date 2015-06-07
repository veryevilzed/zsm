using System;
using System.Collections;
using System.Collections.Generic;

namespace ZSM{

	public interface IState {
		FSM Parent { get; set; }
		Dictionary<string, object> Data { get; }
		string Enter(params object[] args);
		string Do(params object[] args);
		string Exit(params object[] args);
	}

	public class State : IState{

		public FSM Parent { get; set; }

		public Dictionary<string, object> Data { get; protected set; }

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
			Data = new Dictionary<string, object>();
		}

		public State(Dictionary<string, object> data) {
			Data = data;
		}
	}

	public class SmartState : IState {

		private Route route;
		public FSM Parent {get; set; }
		public Dictionary<string, object> Data { get; protected set; }

		public int GetInt(string name){
			return GetInt(name, 0);
		}

		public int GetInt(string name, int def){
			if (!Data.ContainsKey(name))
				return def;
			else
				return (int)Data[name];
		}

		public float GetFloat(string name){
			return GetFloat(name, 0);
		}

		public float GetFloat(string name, float def){
			if (!Data.ContainsKey(name))
				return def;
			else
				return (float)Data[name];
		}

		public string GetString(string name){
			return GetString(name, "");
		}

		public string GetString(string name, string def){
			if (!Data.ContainsKey(name))
				return def;
			else
				return (string)Data[name];
		}

		public bool GetBool(string name){
			return GetBool(name, false);
		}

		public bool GetBool(string name, bool def){
			if (!Data.ContainsKey(name))
				return def;
			else
				return (bool)Data[name];
		}

		public double GetDouble(string name){
			return GetDouble(name, 0);
		}

		public double GetDouble(string name, double def){
			if (!Data.ContainsKey(name))
				return def;
			else
				return (double)Data[name];
		}

		public void Inc(string name){
			int i = GetInt(name, 0);
			this.Data[name] = i + 1;
		}

		public void Inc(string name, int val){
			int i = GetInt(name, 0);
			this.Data[name] = i + val;
		}

		public void Inc(string name, float val){
			float i = GetFloat(name, 0);
			this.Data[name] = i + val;
		}


		public void Dec(string name){
			int i = GetInt(name, 0);
			this.Data[name] = i - 1;
		}

		public void Dec(string name, int val){
			int i = GetInt(name, 0);
			this.Data[name] = i - val;
		}

		public void Dec(string name, float val){
			float i = GetFloat(name, 0);
			this.Data[name] = i - val;
		}


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
			Data = data;
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

	public class FSM {

		public Dictionary<string, IState> states;
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
		}

	}



}

