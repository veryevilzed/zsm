using System;
using System.Collections.Generic;

namespace ZSM {

	public class ZDataEventArgs : ZEventArgs {
		public ZData Data {get { return (ZData)Sender; } }
		public string Key { get { return (string)Args[0]; } }
		public object Value { get { return Args[1]; } }
		public object Before { get { return Args[2]; } }

		public ZDataEventArgs(ZData sender, string key, object value, object before):base(string.Format("change.{0}", key), sender, key, value, before) {
		}
	}

	public class ZData {

		public EventManager EventManager {get; protected set;}

		private Dictionary<string, object> data;

		public object this[string key] {
			get { return this.data[key]; }
			set { this.Set(key, value); }
		}

		public void Set(string key, object value, bool silent){
			object before = this.Get(key);
			if (value != null) {
				this.data[key] = value;
				if (!silent){
					EventManager.Invoke(new ZDataEventArgs(this, key, value, before));
				}
			} else {
				this.data.Remove(key);
				if (!silent)
					EventManager.Invoke(new ZDataEventArgs(this, key, null, before));
			}
		}

		public void Set(string key, object value){ Set(key, value, false); }

		public object Get(string key){
			return Get(key, null);
		}

		public object Get(string key, object def){
			if (this.data.ContainsKey(key))
				return data[key];
			return def;
		}

		public void AddChangeFieldEvent(string field, DZEvent listener){
			this.EventManager.AddEvent(string.Format("change.{0}",field), listener);
		}

		#region helper

		public int GetInt(string name){
			return GetInt(name, 0);
		}

		public int GetInt(string name, int def){
			return (int)Get(name, def);
		}

		public float GetFloat(string name){
			return GetFloat(name, 0);
		}

		public float GetFloat(string name, float def){
			return (float)GetFloat(name, def);
		}

		public string GetString(string name){
			return GetString(name, "");
		}

		public string GetString(string name, string def){
			return (string)Get(name, def);
		}

		public bool GetBool(string name){
			return GetBool(name, false);
		}

		public bool GetBool(string name, bool def){
			return (bool)Get(name, def);
		}

		public double GetDouble(string name){
			return GetDouble(name, 0);
		}

		public double GetDouble(string name, double def){
			return (double)Get(name, def);
		}

		public void Inc(string name){
			int i = GetInt(name, 0);
			this.Set(name, i + 1);
		}

		public void Inc(string name, int val){
			int i = GetInt(name, 0);
			this.Set(name, i + val);
		}

		public void Inc(string name, float val){
			float i = GetFloat(name, 0);
			this.Set(name, i + val);
		}


		public void Dec(string name){
			int i = GetInt(name, 0);
			this.Set(name, i - 1);
		}

		public void Dec(string name, int val){
			int i = GetInt(name, 0);
			this.Set(name, i - val);
		}

		public void Dec(string name, float val){
			float i = GetFloat(name, 0);
			this.Set(name, i - val);
		}

		#endregion

		public ZData():this(new Dictionary<string, object>()) {
		}

		public ZData(Dictionary<string, object> data) {
			EventManager = new EventManager();
			this.data = data;
		}
	}
}

