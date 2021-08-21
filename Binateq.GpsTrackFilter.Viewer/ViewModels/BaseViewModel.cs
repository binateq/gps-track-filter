namespace Binateq.GpsTrackFilter.Viewer.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Helpers;

    public class BaseViewModel : CancellationSource, INotifyPropertyChanged
    {
        public BaseViewModel()
        {
            CommandFactory = new SafeCommandFactory();
        }

        protected BaseViewModel(SafeCommandFactory commandFactory)
        {
            CommandFactory = commandFactory;
        }
        
        protected SafeCommandFactory CommandFactory { get; }

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set { isBusy = value; PropertyChanged?.Invoke(this, default); }
        }

        public bool IsNotBusy => !IsBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        protected Func<Task> WrapCommandFunc(Func<Task> func)
        {
            return async () =>
            {
                if (IsBusy)
                    return;

                try
                {
                    IsBusy = true;
                    await func();
                }
                finally
                {
                    IsBusy = false;
                }
            };
        }
    }
}
