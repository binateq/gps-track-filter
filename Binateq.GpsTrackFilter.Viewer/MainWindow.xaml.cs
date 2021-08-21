namespace Binateq.GpsTrackFilter.Viewer
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Helpers;
    using MapControl;
    using ViewModels;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ImageLoader.HttpClient.DefaultRequestHeaders.Add("User-Agent", "XAML Map Control Test Application");

            InitializeComponent();
            DataContext = ViewModelLocator.Resolve<MapViewModel>();
        }

        private MapViewModel ViewModel => (MapViewModel)DataContext;

        private void InstScroll_Loaded(object sender, RoutedEventArgs e)
        {
            listview.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
        }

        private void MyMouseWheelH(object sender, RoutedEventArgs e)
        {
            var mouseWheelEventArgs = (MouseWheelEventArgs)e;
            var x = (double)mouseWheelEventArgs.Delta;
            var y = instScroll.VerticalOffset;
            instScroll.ScrollToVerticalOffset(y - x);
        }

        private void MapMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                map.TargetCenter = map.ViewToLocation(e.GetPosition(map));
        }
        
        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            var location = map.ViewToLocation(e.GetPosition(map));
            var latitude = Math.Round(location.Latitude, 6, MidpointRounding.ToEven);
            var longitude = Math.Round(location.Longitude, 6, MidpointRounding.ToEven);

            MouseCoordinates.Text =
                $"{latitude.ToString("F6", CultureInfo.InvariantCulture)}, " +
                $"{longitude.ToString("F6", CultureInfo.InvariantCulture)}";
        }

        private void MapMouseLeave(object sender, MouseEventArgs e)
        {
            MouseCoordinates.Text = string.Empty;
        }

        private void MapManipulationInertiaStarting(object sender, ManipulationInertiaStartingEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 0.001;
        }

        private void MapItemTouchDown(object sender, TouchEventArgs e)
        {
            if (sender is not MapItem mapItem)
                return;
            
            mapItem.IsSelected = !mapItem.IsSelected;
            e.Handled = true;
        }
        
        public void SelectorOnSelected(object sender, RoutedEventArgs e)
        {
            if (sender is not ListView { SelectedItem: Track track }) 
                return;

            ViewModel.LoadCellIdCommand.Execute(track);
        }
    }
}
