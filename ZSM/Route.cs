using System;
using System.Reflection;

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
			return Execute((string)args[0], Utils.Trim(args));
		}

		public Route(object target) {
			this.Target = target;
		}
	}
}

