using System;
using System.Threading;

namespace ZSM.Async
{


    
    public class ZTask<T>
    {
        public delegate void DComplite(ZTask<T> result);
        public delegate T DTask(object[] args);
        
        private Thread t;

        private DTask request;

        private int timeout = -1;
        
        public DComplite OnComplite;
        
        private T result;

        private object[] args;
        
        public Exception TaskException { get; protected set; }
        
        public bool IsDone { get; set; }

        public void Timeout(int timeout)
        {
            new Thread(() =>
            {
                Thread.Sleep(timeout);
                if (t!=null && t.IsAlive)
                    Abort();
            }).Start();       
        }

        
        public void Start()
        {
            if (t != null)
                return;
            t = new Thread(Run);
            t.Start();
            if (timeout > 0) Timeout(timeout);
        }

        public void Abort()
        {
            if (t != null) {
                t.Abort();
                t = null;
                TaskException = new ZTaskAbortException();
                IsDone = true;
                if (OnComplite != null)
                    OnComplite(this);
            }
        }

        private void Run()
        {
            try
            {
                result = request.Invoke(args);
            }
            catch (Exception e)
            {
                TaskException = e;
            }
            finally
            {
                IsDone = true;
                OnComplite(this);
            }
            
        }
        
        public ZTask()
        {
            IsDone = false;
        }

        public static class TaskFactory
        {
            public static ZTask<T> CreateAsyncTask(DTask method)
            {
                ZTask<T> res = new ZTask<T>();
                res.Start();
                return res;
            }
            
            public static ZTask<T> CreateTimeoutedTask(DTask method, int timeout)
            {
                ZTask<T> res = new ZTask<T>();
                res.Start();
                return res;
            } 

        }   
    }

    public class ZTaskAbortException : Exception
    {
    }

}