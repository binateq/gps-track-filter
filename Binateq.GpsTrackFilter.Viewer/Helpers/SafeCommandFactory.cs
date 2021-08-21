namespace Binateq.GpsTrackFilter.Viewer.Helpers
{
    using System;
    using System.Threading.Tasks;

    public class SafeCommandFactory
    {
        protected IExceptionHandler DefaultExceptionHandler { get; }

        public SafeCommandFactory()
        {
            DefaultExceptionHandler = new DefaultExceptionHandler();
        }

        public SafeCommand Create(Func<Task> execute, Func<bool> canExecute, Func<Exception, Task> exceptionHandler = null)
        {
            return new SafeCommand(execute, canExecute, exceptionHandler ?? DefaultExceptionHandler.HandleAsync);
        }

        public SafeCommand<TResult> Create<TResult>(Func<TResult, Task> execute, Func<Exception, Task> exceptionHandler = null)
        {
            return new SafeCommand<TResult>(execute, exceptionHandler ?? DefaultExceptionHandler.HandleAsync);
        }
    }
}
