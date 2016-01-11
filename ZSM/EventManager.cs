using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ZSM {

	public delegate void DZEvent(ZEventArgs args);

	public class ZEventArgs {
		public string EventName { get; set; }
		public object Sender { get; set; }
		public object[] Args { get; set; }
		public bool Immediately { get;set;}
		public EventManager Manager {get; set;}
		public override string ToString(){
			return string.Format("[ZEventArgs: EventName={0}, Sender={1}, Args={2}]", EventName, Sender, Args);
		}

		public ZEventArgs():this("") {}
		public ZEventArgs(string eventName):this(eventName,null) {}

		public ZEventArgs(string eventName, object sender, params object[] args ) {
			this.EventName = eventName;
			this.Sender = sender;
			this.Args = args;
			this.Immediately = false;
		}
	}

	public class EventManager {

//		private class EventTree {
//			public string Leaf {get; protected set; }
//			public EventManager Manager {get; protected set;}
//			public HashSet<DZEvent> Listeners { get; protected set; }
//			public Dictionary<string, EventTree> Childs { get; protected set; }
//			public EventTree Parent {get; protected set;}
//
//			public void AddEvent(List<string> path, DZEvent listener) {
//				if (path.Count == 0) {
//					Listeners.Add(listener);
//					if (!Manager.support.ContainsKey(listener))
//						Manager.support.Add(listener, new List<EventTree>());
//					Manager.support[listener].Add(this);
//				} else {
//					string e = path[0];
//					path.RemoveAt(0);
//					if (!Childs.ContainsKey(e))
//						Childs.Add(e, new EventTree(this, e, Manager));
//
//					Childs[e].AddEvent(path, listener);
//				}
//			}
//
//			public void RemoveEvent(DZEvent ev){
//				this.Listeners.Remove(ev);
//				if (this.Parent != null && this.Listeners.Count == 0 && this.Childs.Count == 0)
//					Parent.RemoveEvent(this.Leaf);
//			}
//
//			public void RemoveEvent(string path){
//				this.Childs.Remove(path);
//				if (this.Parent != null && this.Listeners.Count == 0 && this.Childs.Count == 0)
//					Parent.RemoveEvent(this.Leaf);
//			}
//
//			public override string ToString() {
//				EventTree l = this;
//				string rr = "";
//				while(l.Parent != null){
//					rr = l.Leaf + " - " + rr;
//					l = l.Parent.ToString();
//				}
//				return "Leaf = " + rr;
//			}
//
//			public HashSet<DZEvent> GetListeners(List<string> path, HashSet<DZEvent> res) {
//
//				if (Leaf == "*" || path.Count == 0)
//					foreach(DZEvent listener in Listeners)
//						res.Add(listener);
//
//				if (path.Count == 0)
//					return res;
//					
//				string e = path[0];
//				path.RemoveAt(0);
//
//				if (Childs.ContainsKey(e))
//					res = Childs[e].GetListeners(new List<string>(path.ToArray()), res);
//
//				if (Childs.ContainsKey("*"))
//					res = Childs["*"].GetListeners(new List<string>(path.ToArray()), res);
//					
//				return res;
//			}
//
//			public EventTree(EventTree parent, string leaf, EventManager manager) {
//				this.Listeners = new HashSet<DZEvent>();
//				Childs = new Dictionary<string, EventTree>();
//				this.Parent = parent;
//				this.Leaf = leaf;
//				this.Manager = manager;
//			}
//		}

		public class EventTree {
			public EventManager Manager {get; protected set;}
			public HashSet<DZEvent> Listeners { get; protected set; }
			public HashSet<DZEvent> GetListeners() {
				return this.Listeners;
			}

			public void RemoveEvent(DZEvent e){
				this.Listeners.Remove(e);
			}

			public EventTree(EventManager parent) {
				this.Listeners = new HashSet<DZEvent>();
				this.Manager = parent;
			}
		}

		private Dictionary<DZEvent, List<EventTree>> support;
		private Dictionary<string, EventTree> _events;


		public static Queue eventPool = null;
		public static int poolStep = 3;

		public static void ApplayPool(int step){
			if (eventPool.Count == 0)
				return;
			for(int i=0;i<step;i++){
				if (eventPool.Count == 0)
					break;
				ZEventArgs o = (ZEventArgs)eventPool.Dequeue();
				o.Manager.InvokeImmediately(o);
			}
		}


		public static char EventSeparator = '.';

		public void Invoke(string eventName){
			this.Invoke(new ZEventArgs(eventName));
		}

		public void Invoke(string eventName, object sender, params object[] args){
			this.Invoke(new ZEventArgs(eventName, sender, args));
		}

		public void Invoke(ZEventArgs args){
			if (!_events.ContainsKey(args.EventName))
				return;
			if (eventPool == null || args.Immediately)
				InvokeImmediately(args);
			else{
				args.Manager = this;
				eventPool.Enqueue(args);
			}
		}


		public void InvokeImmediately(ZEventArgs args){
			if (!_events.ContainsKey(args.EventName))
				return;
			HashSet<DZEvent> _listeners = _events[args.EventName].GetListeners();
			DZEvent[] listeners = new DZEvent[_listeners.Count];
			_listeners.CopyTo(listeners);
			if (listeners.Length > 15)
				ZSMLog.Log.Warn("Invoke event:{0} ({1})", args.EventName, listeners.Length);
			if (listeners.Length == 0)
				return;
			System.DateTime dt = DateTime.Now;
			foreach(DZEvent listener in listeners){
				if (listener != null) {
					if (listener.Method.IsStatic || listener.Target != null){
						try{
							listener.Invoke(args);
						}catch{
							this.RemoveEvent(listener);
						}
					}else
						this.RemoveEvent(listener);
				}
			}
			if ((DateTime.Now - dt).TotalSeconds > 1.0f)
				ZSMLog.Log.Warn("Invoke {1} time {0}", (DateTime.Now - dt).TotalSeconds, args.EventName);
		}

		public void AddEvent(string eventName, DZEvent listener){
			ZSMLog.Log.Debug("Add event:{0}", eventName);
			if (this._events.ContainsKey(eventName)){
				this._events[eventName].Listeners.Add(listener);
				if (this.support.ContainsKey(listener))
					this.support[listener].Add(this._events[eventName]);
				else
					this.support.Add(listener, new List<EventTree>(new EventTree[]{ this._events[eventName] }));
		    }else{
				EventTree et =  new EventTree(this);
				et.Listeners.Add(listener);
				this._events.Add(eventName, et);
				if (this.support.ContainsKey(listener))
					this.support[listener].Add(et);
				else
					this.support.Add(listener, new List<EventTree>(new EventTree[]{ et }));
			}
		}

		public void RemoveEvent(DZEvent listener){

			if (!this.support.ContainsKey(listener)){
				return;
			}
				
			foreach(EventTree et in this.support[listener])
				et.RemoveEvent(listener);
			this.support.Remove(listener);

			ZSMLog.Log.Debug("Remove event:{0}", listener.Method.Name);
		}

		public EventManager() {
//			root = new EventTree(null, "", this);
			support = new Dictionary<DZEvent, List<EventTree>>();
			_events = new Dictionary<string, EventTree>();

		}

		public override string ToString(){
			return string.Format("[EventManager] Keys={0}", _events.Keys.Count);
		}
	}



}

