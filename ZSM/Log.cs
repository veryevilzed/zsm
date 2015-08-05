using System;

namespace ZSM {

	public interface ILog {
		void Debug(string format, params object[] args);
		void Info(string format, params object[] args);
		void Warn(string format, params object[] args);
		void Error(string format, params object[] args);

	}

	public class ZSMLog : ILog {

		private static ILog log = new ZSMLog();

		public static ILog Log {
			get { return log; } 
			set { log = value; } 
		}


		#region ILog implementation
		public void Debug(string format, params object[] args){
			Console.WriteLine(string.Format("[DEBUG]"+ format, args));
		}
		public void Info(string format, params object[] args){
			Console.WriteLine(string.Format("[INFO ]"+ format, args));
		}
		public void Warn(string format, params object[] args){
			Console.WriteLine(string.Format("[WARN ]"+ format, args));
		}
		public void Error(string format, params object[] args){
			Console.WriteLine(string.Format("[ERROR]"+ format, args));
		}
		#endregion
	}
}

