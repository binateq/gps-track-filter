namespace Binateq.GpsTrackFilter.Viewer.Helpers
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using ViewModels;

    public static class ViewModelLocator
    {
        private static readonly object Sync = new();
        private static IServiceProvider _serviceProvider;

        public static void Initialize(IServiceProvider serviceProvider)
        {
            lock (Sync)
            {
                if (_serviceProvider != null)
                    return;

                _serviceProvider = serviceProvider;
            }
        }

        public static TViewModel Resolve<TViewModel>() where TViewModel : BaseViewModel
        {
            lock (Sync)
            {
                return _serviceProvider.GetRequiredService<TViewModel>();
            }
        }
    }
}
