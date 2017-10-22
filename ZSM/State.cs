using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZSM {
	public interface IState {
		FSM Parent { get; set; }
		ZData Data { get; }
		string Enter(params object[] args);
		string Do(params object[] args);
		string Exit(params object[] args);
		void Update(float deltaTime);

		string doEnter(params object[] args);
		string doExit(params object[] args);
		void doUpdate(float deltaTime);
		string NextState(string key);
	}

	public class State : IState{

		public string doEnter(params object[] args)
		{
			BeforeEnter(args);
			var res =  Enter(args);
			AfterEnter(args);
			return res;
		}

		public string doExit(params object[] args)
		{
			BeforeExit(args);
			var res =  Exit(args);
			AfterExit(args);
			return res;
		}
		public void doUpdate(float deltaTime) { this.Update(deltaTime); }

		public FSM Parent { get; set; }

		public ZData Data { get; protected set; }
		
		public virtual Dictionary<string, string> NextStateCollection { get { return new Dictionary<string, string>(); } }

		public virtual string NextState(string key)
		{
			return NextStateCollection.ContainsKey(key) ? NextStateCollection[key] : key;
		}

		public virtual void BeforeEnter(params object[] args)
		{
		}

		public virtual void AfterEnter(params object[] args)
		{
		}

		
		public virtual string Enter(params object[] args) {
			return "";
		}

		public virtual string Do(params object[] args){
			return "";
		}

		
		public virtual void BeforeExit(params object[] args)
		{
		}

		public virtual void AfterExit(params object[] args)
		{
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

		Dictionary<string, Timing> timings = new Dictionary<string, Timing>();
		List<Timing> tms = new List<Timing>();

		public Dictionary<string, Timing> Timers { get { return timings; } }

		private Route route;
		
		public FSM Parent {get; set; }
		public ZData Data { get; protected set; }

		public virtual void BeforeEnter(params object[] args)
		{
		}

		public virtual void AfterEnter(params object[] args)
		{
		}
		
		public virtual void BeforeExit(params object[] args)
		{
		}

		public virtual void AfterExit(params object[] args)
		{
		}
		
		public virtual Dictionary<string, string> NextStateCollection { get { return new Dictionary<string, string>(); } }

		public virtual string NextState(string key)
		{
			return NextStateCollection.ContainsKey(key) ? NextStateCollection[key] : key;
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

		public virtual void Update(float deltaTime) {
		}

		public string doEnter(params object[] args)
		{
			ResetAllTimers();
			BeforeEnter(args);
			var res =   Enter(args);
			AfterEnter(args);
			return res;
		}

		public string doExit(params object[] args)
		{
			BeforeExit(args);
			var res =  Exit(args);
			AfterExit(args);
			return res;
		}
		
		public void doUpdate(float deltaTime) { 
			UpdateAllTimers(deltaTime);
			this.Update(deltaTime); 
		}

		private void ResetAllTimers() {
			foreach (Timing t in tms)
				t.Reset();
		}

		private void UpdateAllTimers(float deltaTime) {
			foreach (Timing t in tms)
				t.Update(deltaTime);
		}


		private void CreateTimers() {
			foreach (MethodInfo mi in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
				bool _disabled = false;
				foreach (object o2 in mi.GetCustomAttributes(false)) 
					if (o2 is DisableTimer) 
						_disabled = true;
				

				foreach (object o in mi.GetCustomAttributes(false)) {
					if (o is Loop){
						if (mi.GetGenericArguments().Length > 0)
							throw new System.ArgumentException("Need method without arguments");
						Loop r = (Loop)o;
						Timing t = Timing.Loop(r.Interval, mi, this, r.StartInterval);
						t.Priority = r.Priority;
						if (_disabled) {
							t.defaultEnabled = false;
							t.Stop();
						}
						
						if (this.timings.ContainsKey(r.Name)) {
							this.tms.Remove(this.timings[r.Name]);
							this.timings[r.Name] = t;
							this.tms.Add(t);
						} else {
							this.timings.Add(r.Name, t);
							this.tms.Add(t);
						}
						continue;
					}
					if (o is One) {
						if (mi.GetGenericArguments().Length > 0)
							throw new System.ArgumentException("Need method without arguments");
						One r = (One)o;
						Timing t = Timing.One(r.Interval, mi, this);
						if (_disabled) {
							t.defaultEnabled = false;
							t.Stop();
						}
						t.Priority = r.Priority;
						if (this.timings.ContainsKey(r.Name)) {
							this.tms.Remove(this.timings[r.Name]);
							this.timings[r.Name] = t;
							this.tms.Add(t);
						} else {
							this.timings.Add(r.Name, t);
							this.tms.Add(t);
						}
						continue;
					}

				}
			}

			tms.Sort();
		}

		public SmartState():this(new Dictionary<string, object>()) {}

		public SmartState(Dictionary<string, object> data) {
			Data = new ZData(data);
			route = new Route(this);
			CreateTimers();
		} 
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class StateAttribute : Attribute {
		public string Name { get; set; }
		public StateAttribute(string name){
			this.Name = name;
		}
	}


	public class ZSMExecutionAttribute : System.Attribute, IComparable<ZSMExecutionAttribute> {

		public int Priority {get; protected set;}
		public MethodInfo Method {get; set;}
		public object Target {get; set;}
		public string Name { get; protected set; }

		public virtual int CompareTo (ZSMExecutionAttribute other)
		{
			return this.Priority - other.Priority;
		}

		public virtual void Invoke(){
			this.Method.Invoke(this.Target, new object[0]);
		}

		public virtual void Invoke(object[] args){
			this.Method.Invoke(this.Target, args);
		}

		public ZSMExecutionAttribute(){ }
	}


	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class Loop : ZSMExecutionAttribute, IComparable<Loop>{

		private float currentInterval = 0;
		private float startInterval = 0;

		public float Interval {get; protected set;}
		public float StartInterval { get {return startInterval;} set {startInterval = value;}}


		public void Update(){
			//base.Invoke();
		}

		public int CompareTo (Loop other) {
			return this.Priority - other.Priority;
		}

		public Loop():this("loop", 1.0f, 0.0f, 0) {}
		public Loop(string name, float interval):this(name, interval, 0.0f, 0) {}
		public Loop(string name, float interval, float startIntervalValue):this(name, interval, startIntervalValue, 0) {}
		public Loop(string name, float interval, float startIntervalValue, int priority){
			this.Name = name;
			this.Interval = interval;
			this.currentInterval = startIntervalValue;
			this.Priority = priority;
		}



		public override string ToString ()
		{
			return string.Format ("[Loop({3}): Interval={0}/{1} P={2}]", Interval, currentInterval, Priority, Name);
		}
	}

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class One : ZSMExecutionAttribute, IComparable<One>{
		private float currentInterval = 0;
		public float Interval {get; protected set;}

		public int CompareTo (One other)
		{
			return this.Priority - other.Priority;
		}

		public One(string name, float interval):this(name, interval, 0) {}
		public One(string name, float interval, int priority){
			this.Name = name;
			this.Interval = interval;
			this.currentInterval = 0.0f;
			this.Priority = priority;
		}



		public One():this("one", 1.0f, 0) {}

		public override string ToString ()
		{
			return string.Format ("[One({3}): Interval={0}/{1} P={2}]", Interval, currentInterval, Priority, Name);
		}
	}

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	public class DisableTimer : System.Attribute {
		
	}


}

