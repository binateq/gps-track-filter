namespace Binateq.GpsTrackFilter.Viewer.Helpers
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    
    public class DefaultExceptionHandler : IExceptionHandler
    {
        public Task HandleAsync(Exception exception)
        {
            switch (exception)
            {
                case TaskCanceledException _:
                    // Ignore
                    break;
                default:
                    var exceptionTypeName = exception.GetType().ToString();
                    var text = exception.Message;
                    MessageBox.Show(Application.Current.MainWindow!, text, caption: exceptionTypeName);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
