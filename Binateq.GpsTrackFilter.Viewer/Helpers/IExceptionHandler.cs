namespace Binateq.GpsTrackFilter.Viewer.Helpers
{
    using System;
    using System.Threading.Tasks;

    public interface IExceptionHandler
    {
        Task HandleAsync(Exception exception);
    }
}
