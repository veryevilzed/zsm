using System;
using System.Collections.Generic;

namespace ZSM {

	public delegate void DZEvent(string eventName, object sender, object[] args);

	public class EventManager {

		private class EventTree {
			public string Leaf {get; protected set; }
			public EventManager Manager {get; protected set;}
			public HashSet<DZEvent> Listeners { get; protected set; }
			public Dictionary<string, EventTree> Childs { get; protected set; }
			public EventTree Parent {get; protected set;}

			public void AddEvent(List<string> path, DZEvent listener) {
				if (path.Count == 0) {
					Listeners.Add(listener);
					if (!Manager.support.ContainsKey(listener))
						Manager.support.Add(listener, new List<EventTree>());
					Manager.support[listener].Add(this);
				} else {
					string e = path[0];
					path.RemoveAt(0);
					if (!Childs.ContainsKey(e))
						Childs.Add(e, new EventTree(this, e, Manager));

					Childs[e].AddEvent(path, listener);
				}
			}

			public void RemoveEvent(DZEvent ev){
				this.Listeners.Remove(ev);
				if (this.Parent != null && this.Listeners.Count == 0 && this.Childs.Count == 0)
					Parent.RemoveEvent(this.Leaf);
			}

			public void RemoveEvent(string path){
				this.Childs.Remove(path);
				if (this.Parent != null && this.Listeners.Count == 0 && this.Childs.Count == 0)
					Parent.RemoveEvent(this.Leaf);
			}

			public string ToString() {
				EventTree l = this;
				string rr = "";
				while(l.Parent != null){
					rr = l.Leaf + " - " + rr;
					l = l.Parent;
				}
				return "Leaf = " + rr;
			}

			public HashSet<DZEvent> GetListeners(List<string> path, HashSet<DZEvent> res) {
			
				Console.WriteLine(this.ToString());

				if (Leaf == "*" || path.Count == 0)
					foreach(DZEvent listener in Listeners)
						res.Add(listener);

				if (path.Count == 0)
					return res;
					
				string e = path[0];
				path.RemoveAt(0);

				if (Childs.ContainsKey(e))
					res = Childs[e].GetListeners(new List<string>(path.ToArray()), res);

				if (Childs.ContainsKey("*"))
					res = Childs["*"].GetListeners(new List<string>(path.ToArray()), res);
					
				return res;
			}

			public EventTree(EventTree parent, string leaf, EventManager manager) {
				this.Listeners = new HashSet<DZEvent>();
				Childs = new Dictionary<string, EventTree>();
				this.Parent = parent;
				this.Leaf = leaf;
				this.Manager = manager;
			}
		}

		private Dictionary<DZEvent, List<EventTree>> support;

		private EventTree root;

		public void Invoke(string eventName, object sender, params object[] args){
			HashSet<DZEvent> listeners = root.GetListeners(new List<string>(eventName.Trim().Split('.')), new HashSet<DZEvent>());
			foreach(DZEvent listener in listeners){
				if (listener != null)
					listener.Invoke(eventName, sender, args);

			}
		}

		public void AddEvent(string eventName, DZEvent listener){
			root.AddEvent(new List<string>(eventName.Trim().Split('.')), listener);
		}

		public void RemoveEvent(DZEvent listener){
			if (!this.support.ContainsKey(listener))
				return;

			foreach(EventTree et in this.support[listener])
				et.RemoveEvent(listener);
			this.support.Remove(listener);
		}

		public EventManager() {
			root = new EventTree(null, "", this);
			support = new Dictionary<DZEvent, List<EventTree>>();

		}
	}
}

