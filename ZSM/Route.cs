using System;
using System.Reflection;
using System.Collections.Generic;

namespace ZSM {
	public class Route {

		public object Target { get; set; }

		public string Execute(string method, params object[] args){
			foreach (MethodInfo mi in Target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance)) {
				if (mi.Name == method && mi.GetParameters().Length == args.Length)
					return (string)mi.Invoke(Target, args);
			}
			return "";
		}

		public string Do(params object[] args){
			return Execute((string)args[0], Utils.RemoveFirst(args));
		}

		public Route(object target) {
			this.Target = target;
		}
	}


	public class EventRouter : Route {

		public object Target {get; protected set;}

		public void MethodFirst(ZEventArgs args) {
			string[] a = args.EventName.Split('.',1);
			string name = a[0];
			this.Execute(name, args.Args);
		}

		public void MethodFull(ZEventArgs args) {
			string name = args.EventName.Replace(".","_");
			this.Execute(name, args.Args);
		}

	}
}

