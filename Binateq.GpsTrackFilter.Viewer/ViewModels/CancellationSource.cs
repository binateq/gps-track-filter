namespace Binateq.GpsTrackFilter.Viewer.ViewModels
{
    using System;
    using System.Threading;

    public abstract class CancellationSource : IDisposable
    {
        // https://stackoverflow.com/questions/6168483/how-to-reset-the-cancellationtokensource-and-debug-the-multithread-with-vs2010
        private CancellationTokenSource cancellation;

        protected CancellationTokenSource Cancellation
        {
            get
            {
                if (disposed)
                    return null;

                if (cancellation != null)
                {
                    if (cancellation.IsCancellationRequested)
                        cancellation.Dispose();
                    else
                        return cancellation;
                }

                return cancellation = new CancellationTokenSource();
            }
        }

        protected CancellationToken CancellationToken => Cancellation?.Token ?? new CancellationToken(true);

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                cancellation?.Dispose();
                cancellation = null;
            }

            disposed = true;
        }

        private bool disposed;
    }
}
