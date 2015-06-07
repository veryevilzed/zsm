using System;
using System.Collections.Generic;

namespace ZSM {
	public class FSMData : Dictionary<string, object>  {

		public int GetInt(string name){
			return GetInt(name, 0);
		}

		public int GetInt(string name, int def){
			if (!this.ContainsKey(name))
				return def;
			else
				return (int)this[name];
		}

		public float GetFloat(string name){
			return GetFloat(name, 0);
		}

		public float GetFloat(string name, float def){
			if (!this.ContainsKey(name))
				return def;
			else
				return (float)this[name];
		}

		public string GetString(string name){
			return GetString(name, "");
		}

		public string GetString(string name, string def){
			if (!this.ContainsKey(name))
				return def;
			else
				return (string)this[name];
		}

		public bool GetBool(string name){
			return GetBool(name, false);
		}

		public bool GetBool(string name, bool def){
			if (!this.ContainsKey(name))
				return def;
			else
				return (bool)this[name];
		}

		public double GetDouble(string name){
			return GetDouble(name, 0);
		}

		public double GetDouble(string name, double def){
			if (!this.ContainsKey(name))
				return def;
			else
				return (double)this[name];
		}

		public void Inc(string name){
			int i = GetInt(name, 0);
			this[name] = i + 1;
		}

		public void Inc(string name, int val){
			int i = GetInt(name, 0);
			this[name] = i + val;
		}

		public void Inc(string name, float val){
			float i = GetFloat(name, 0);
			this[name] = i + val;
		}


		public void Dec(string name){
			int i = GetInt(name, 0);
			this[name] = i - 1;
		}

		public void Dec(string name, int val){
			int i = GetInt(name, 0);
			this[name] = i - val;
		}

		public void Dec(string name, float val){
			float i = GetFloat(name, 0);
			this[name] = i - val;
		}


		public FSMData() : base() {
		}

		public FSMData(Dictionary<string, object> data) : base(data) {
		}

	}
}

