namespace Binateq.GpsTrackFilter.Viewer.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Input;

    public class SafeCommand : ICommand
    {
        private readonly Func<Exception, Task> handler;
        private readonly Func<object, bool> canExecute;
        private readonly Func<object, Task> execute;

        public SafeCommand(Func<object, Task> execute, Func<Exception, Task> handler = null)
        {
            this.execute = execute;
            this.handler = handler;
        }

        public SafeCommand(Func<Task> execute, Func<bool> canExecute, Func<Exception, Task> handler = null)
        {
            this.execute = o => execute();
            this.canExecute = o => canExecute();
            this.handler = handler;
        }

        public SafeCommand(Func<object, Task> execute, Func<object, bool> canExecute, Func<Exception, Task> handler = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.handler = handler;
        }

        public bool CanExecute(object parameter) => parameter == null || canExecute == null || canExecute(parameter);

        public async void Execute(object parameter)
        {
            try
            {
                await execute(parameter).ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
                await handler(exception).ConfigureAwait(true);
            }
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }

    public class SafeCommand<T> : SafeCommand
    {
        public SafeCommand(Func<T, Task> execute, Func<Exception, Task> handler = null) : base(o => IsValidParameter(o) ? execute((T)o) : Task.CompletedTask, handler)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
        }

        public SafeCommand(Func<T, Task> execute, Func<T, bool> canExecute, Func<Exception, Task> handler = null) : base(
            o => IsValidParameter(o) ? execute((T)o) : Task.CompletedTask,
            o => IsValidParameter(o) && canExecute((T)o),
            handler)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
        }

        private static bool IsValidParameter(object o)
        {
            if (o != null)
                return o is T;

            var t = typeof(T);

            if (Nullable.GetUnderlyingType(t) != null)
                return true;

            return !t.GetTypeInfo().IsValueType;
        }
    }
}
