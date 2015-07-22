using System;
using System.Collections.Generic;

namespace Mokap
{
    abstract class Disposable : IDisposable
    {
        private bool disposed;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public bool Disposed
        {
            get { return disposed; }
        }

        void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    DisposeManaged();

                DisposeUnmanaged();

                GC.SuppressFinalize(this);

                disposed = true;
            }
        }

        protected virtual void DisposeManaged()
        { }

        protected virtual void DisposeUnmanaged()
        { }

        protected void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        public static void SafeDispose<T>(ref T obj)
            where T : IDisposable
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = default(T);
            }
        }

        public static bool SafeDisposeReturn<T>(ref T obj)
            where T : IDisposable
        {
            if (obj != null)
            {
                obj.Dispose();
                obj = default(T);

                return true;
            }
            else
                return false;
        }

        public static void SafeDispose<T>(IEnumerable<T> objects)
            where T : IDisposable
        {
            if (objects != null)
            {
                foreach (var obj in objects)
                {
                    if (obj != null)
                        obj.Dispose();
                }
            }
        }
    }
}
