using System;
using System.Reflection;

namespace ZSM {
	public class Timing : IComparable<Timing> {

		public float resetTime = 1.0f;
		public float currentTime = 0.0f;
		public bool loop = false;
		private bool enable = false;
		public bool defaultEnabled = true;
		public int Priority = 0;
		//public Action listener;
		MethodInfo mi;
		object obj;

		public bool Enable {
			get {
				return enable;
			}
		}


		public int CompareTo(Timing other) {
			return this.Priority - other.Priority;
		}



		public static Timing One(float delay, MethodInfo action, object obj) {
			Timing t = new Timing();
			t.resetTime = delay;
			t.currentTime = 0;
			t.mi = action;
			t.obj = obj;
			return t;
		}

		public static Timing Loop(float delay, MethodInfo action, object obj, float start) {
			Timing t = new Timing();
			t.resetTime = delay;
			t.currentTime = start;
			t.loop = true;
			t.mi = action;
			t.obj = obj;

			return t;
		}

		public static Timing Loop(float delay, MethodInfo action, object obj) {
			Timing t = new Timing();
			t.resetTime = delay;
			t.currentTime = 0;
			t.loop = true;
			t.mi = action;
			t.obj = obj;

			return t;
		}

		public void Stop() {
			this.enable = false;
			this.currentTime = 0;
		}

		public void Start() {
			if (this.enable)
				this.Stop();

			this.enable = true;
		}

		public void Reset() {
			this.currentTime = 0;
			this.enable = defaultEnabled;
		}

		public virtual void Update(float deltaTime) {
			if (enable && obj != null){
				currentTime += deltaTime;
				if(currentTime>=resetTime){
					//currentTime -= resetTime;
					mi.Invoke(obj, new object[0]);
					currentTime = 0;
					if (!loop)
						enable = false;
				}
			}
		}
	}
}

