using System;
using System.Collections.Generic;
using System.Collections;

namespace ZSM {

	public interface IZList {
		ZData Parent { get; set; }
		string ParentKey   { get; set; }
	}

	[Serializable]
	public class ZList<T> : IEnumerable<T>, IZList {

		private List<T> data;
		public ZData Parent { get; set; }
		public string ParentKey   { get; set; }

		private void OnParentEvent(){


			if (Parent != null){
				Parent.EventManager.Invoke(string.Format("change.{0}", ParentKey), Parent, ParentKey, this, this);
			}
		}

		#region IEnumerable implementation

		public IEnumerator<T> GetEnumerator(){
			return data.GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		IEnumerator IEnumerable.GetEnumerator(){
			return data.GetEnumerator();
		}

		#endregion



		public void Add(T value, bool silent = false){
			data.Add(value);
			if (!silent) OnParentEvent();
		}

		public void Remove(T value, bool silent = false){
			data.Remove(value);
			if (!silent) OnParentEvent();
		}

		public void RemoveAt(int index, bool silent = false){
			data.RemoveAt(index);
			if (!silent) OnParentEvent();
		}

		public void Clear(bool silent = false){
			data.Clear();
			if (!silent) OnParentEvent();
		}

		public T[] ToArray() {
			return this.ToArray();
		}

		public override string ToString(){
			string res =string.Format("[ZList: Count={0}] ", Count);
			for(int i=0;i<this.Count;i++)
				res += this[i].ToString() + " ";
			return res;
		}

		public int Count {
			get { return this.data.Count; }
		}

//		public List<T>.Enumerator GetEnumerator() {
//			return data.GetEnumerator();
//		}
//
		public void AddRange(IEnumerable<T> values, bool silent = false){
			data.AddRange(values);
			if (!silent) OnParentEvent();

		}

		public void RemoveAll(Predicate<T> match, bool silent = false) {
			this.data.RemoveAll(match);
			if (!silent) OnParentEvent();
		}

		public bool Exists(Predicate<T> match) {
			return this.data.Exists(match);
		}

		public T this[int index] {
			get {return this.data[index]; }
			set { 
				this.data[index] = value; 
				OnParentEvent();
			}
		}

		public bool Contains(T value){
			return this.data.Contains(value);
		}


		public ZList() {
			data = new List<T>();
		}


		public ZList(IEnumerable<T> collectoin) {
			data = new List<T>(collectoin);
		}
	}
}

