
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZSM {

	public class ZDataEventArgs : ZEventArgs {
		public ZData Data {get { return (ZData)Sender; } }
		public string Key { get { return (string)Args[0]; } }
		public object Value { get { return Args[1]; } }
		public object Before { get { return Args[2]; } }

		public T GetValue<T>() {
			return (T)Value;
		}

		public T GetBefore<T>() {
			return (T)Before;
		}


		public ZDataEventArgs(ZData sender, string key, object value, object before, string eventName):base(eventName, sender, key, value, before) {
		}

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

				if (data.ContainsKey(key) && value.IsSimpleType() && data[key] == value)
					return;
				this.data[key] = value;
				if (typeof(IZList).IsAssignableFrom(value.GetType())){
					((IZList)value).Parent = this;
					((IZList)value).ParentKey = key;
				}
				if (!silent){
					this.EventManager.Invoke(new ZDataEventArgs(this, key, value, before));
				}
			} else {
				if (this.data.ContainsKey(key)){
					if (typeof(IZList).IsAssignableFrom(value.GetType())){
						((IZList)this.data[key]).Parent = null;
						((IZList)this.data[key]).ParentKey = "";
					}
					this.data.Remove(key);
					if (!silent) {
						EventManager.Invoke(new ZDataEventArgs(this, key, null, before));
					}
				}
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

		public T Get<T>(string key){
			return Get<T>(key, default(T));
		}

		public T Get<T>(string key, T def){
			if (this.data.ContainsKey(key))
				return (T)this.data[key];
			else
				return def;
		}



		public void AddChangeFieldEvent(string field, DZEvent listener){
			this.EventManager.AddEvent(string.Format("change.{0}",field), listener);
		}

		#region helper

		public T GetOrCreate<T>(string name, params object[] createArgs) {
			if (this.data.ContainsKey(name))
				return (T)data[name];

			List<Type> types = new List<Type>();
			foreach (object o in createArgs)
				types.Add(o.GetType());

			ConstructorInfo ci = typeof(T).GetConstructor(types.ToArray());
			if (ci != null){
					T res = (T)ci.Invoke(createArgs);
					this.Set(name, res);
					return res;
			}
			return default(T);
		}

		public ZData GetData(string name){
			return (ZData)this.Get(name, null);
		}

		public ZList<T> GetZList<T>(string name){
			return (ZList<T>)this.GetOrCreate<ZList<T>>(name);
		}


		public T[] GetArray<T>(string name){
			return (T[])Get(name, new T[0]);
		}

		public T[] GetArray<T>(string name, T[] def){
			return (T[])Get(name, def);
		}

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
			return (float)Get(name, def);
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

		public Type GetKeyType(string name){
			if (!this.data.ContainsKey(name))
				return null;
			return this.data[name].GetType();

		}

		public Type GetKeyType(string name, Type def){
			if (!this.data.ContainsKey(name))
				return def;
			return this.data[name].GetType();
		}


		public long GetLong(string name, long def){
			if (GetKeyType(name, typeof(int)) == typeof(int))
				return (long)((int)Get(name, def));
			return (long)Get(name, def);
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

		public void Inc(string name, long val){
			long i = GetLong(name, 0);
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

		public void Dec(string name, long val){
			long i = GetLong(name, 0);
			this.Set(name, i - val);
		}

		#endregion

		public ZData():this(new Dictionary<string, object>()) {
		}

		public ZData(Dictionary<string, object> data) {
			EventManager = new EventManager();
			this.data = data;
			foreach(KeyValuePair<string, object> kv in this.data){
				if (kv.Value != null && typeof(IZList).IsAssignableFrom(kv.Value.GetType())){
					((IZList)kv.Value).Parent = this;
					((IZList)kv.Value).ParentKey = kv.Key;
				}
			}
		}
	}
}

