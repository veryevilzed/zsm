using System;
using System.Collections.Generic;

namespace ZSM {
	public class LoadingCache {

		public delegate object DLoadingCacheObject(string key);
		public delegate bool DLoadingCacheDestroy(string key);

		class CachedObject {
			public int RequestCount { get; set; }
			public DateTime LoadAccess {get;set;}
			public object Cached {get;set;}
		}

		Dictionary<string, CachedObject> cache = new Dictionary<string, CachedObject>();

		public int LifeTime = -1;
		public int MaxRequestCount = -1;

		public DLoadingCacheObject New {get;set;}
		public DLoadingCacheObject Refresh {get;set;}
		public DLoadingCacheDestroy Destroy {get;set;}

		public object this[string key] {
			get {
				CachedObject c = null;
				if (!this.cache.ContainsKey(key)) {
					if (this.New != null) {
						c = new CachedObject {
							RequestCount = MaxRequestCount,
							LoadAccess = DateTime.Now,
							Cached = this.New.Invoke(key)
						};
						this.cache.Add(key, c);
					} else
						return null;
				} else {
					c = cache[key];
					if (c.RequestCount != -1)
						c.RequestCount--;

					if (c.RequestCount == 0 || (LifeTime!=-1 && (DateTime.Now - c.LoadAccess).TotalSeconds >= LifeTime)) {
						if (Refresh != null) {
							c.RequestCount = MaxRequestCount;
							c.LoadAccess = DateTime.Now;
							c.Cached = this.Refresh.Invoke(key);
						} else {
							if (Destroy == null || !Destroy.Invoke(key)) {
								cache.Remove(key);
								return null;
							} 
						}
					}
				}
				return c.Cached;
			}
			set{ 
				if (this.cache.ContainsKey(key)) {
					this.cache[key].Cached = value;
					this.cache[key].RequestCount = MaxRequestCount;
					this.cache[key].LoadAccess = DateTime.Now;
				} else {
					cache.Add(key, new CachedObject {
						RequestCount = MaxRequestCount,
						LoadAccess = DateTime.Now,
						Cached = value
					});
				}
			}
		}

		public LoadingCache() {
		}
	}
}

