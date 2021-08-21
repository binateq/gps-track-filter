namespace Binateq.GpsTrackFilter.Viewer
{
    using System;
    using System.Windows;
    using Helpers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using ViewModels;

    public partial class App : Application
    {
        public App()
        {
            var serviceProvider = BuildServiceProvider();

            ViewModelLocator.Initialize(serviceProvider);
        }

        public static IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
                       
            services.AddSingleton<Func<DateTimeOffset>>(() => DateTimeOffset.Now);
            services.AddLogging(options => options.AddDebug());
            
            services.AddSingleton<TrackService>();            
            services.AddSingleton<MapViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
