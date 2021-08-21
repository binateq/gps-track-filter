namespace Binateq.GpsTrackFilter.Viewer.Maps.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DeviceLocationExtensions
    {
        /// <summary>
        /// Исправляет ошибку с датой и временем в некоторых GPS-приёмниках.
        /// </summary>
        /// https://www.iguides.ru/main/security/po_vsemu_miru_slomalis_gps_navigatory/
        public static IEnumerable<DeviceLocation> FixGpsTimestamp(this IEnumerable<DeviceLocation> locations)
        {
            foreach (var location in locations)
            {
                if (location.Timestamp.Year == 1999)
                {
                    const int weeksIn10Bits = 7 * 1024;
                    var fixedTimestamp = location.Timestamp.AddDays(weeksIn10Bits);

                    yield return location.New(fixedTimestamp);
                }
                else
                    yield return location;
            }
        }

        public static IReadOnlyList<DeviceLocation> SmoothTimestamp(this IEnumerable<DeviceLocation> locations)
        {
            var rLocations = locations.ToArray();

            var state = SmoothingState.Normal;
            var threshold = TimeSpan.FromSeconds(30).Ticks;
            var cacheIndex = 0;
                       
            for (var i = 1; i < rLocations.Length; i++)
            {
                var delta = rLocations[i].Timestamp.Ticks - rLocations[i - 1].Timestamp.Ticks;

                switch (state)
                {
                    case SmoothingState.Normal:
                        if (delta > threshold)
                        {
                            state = SmoothingState.WarningUp;
                            cacheIndex = i - 1;
                        }

                        if (delta < -threshold)
                        {
                            state = SmoothingState.WarningDown;
                            cacheIndex = i - 1;
                        }
                        break;

                    case SmoothingState.WarningUp:
                        if (delta < -threshold)
                        {
                            rLocations = Smoothing(cacheIndex, i, rLocations);
                            state = SmoothingState.Normal;
                        }
                        break;

                    case SmoothingState.WarningDown:
                        if (delta > threshold)
                        {
                            rLocations = Smoothing(cacheIndex, i, rLocations);
                            state = SmoothingState.Normal;
                        }
                        break;
                }
            }

            return rLocations;
        }

        private static DeviceLocation[] Smoothing(int cacheIndex, int currentIndex, DeviceLocation[] locations)
        {
            var outliersLength = currentIndex - 1 - cacheIndex;
            var averageTime = (locations[currentIndex].Timestamp.Ticks - locations[cacheIndex].Timestamp.Ticks) / (outliersLength + 1);

            var sum = locations[cacheIndex].Timestamp.Ticks + averageTime;
            for (var i = cacheIndex + 1; i < currentIndex; i++)
            {
                var buffer = locations[i];
                locations[i] = buffer.New(new DateTimeOffset().AddTicks(sum));
                sum += averageTime;
            }

            return locations;
        }

        private static DeviceLocation New(this DeviceLocation location, DateTimeOffset timestamp) =>
            new(location.Latitude, location.Longitude, location.Heading, location.Speed, timestamp);

        private enum SmoothingState
        {
            Normal = 0,

            WarningUp = 1,

            WarningDown = 2
        }
    }
}
