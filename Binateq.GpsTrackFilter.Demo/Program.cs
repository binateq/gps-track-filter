using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Binateq.GpsTrackFilter.Demo
{
    internal static class Program
    {
        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        static void Main(string[] args)
        {
            switch (args.FirstOrDefault())
            {
                case "rawlocs":
                    PrintRawLocations();
                    break;

                case "rawdirs":
                    PrintRawDirections();
                    break;

                case "fillocs":
                    PrintFilteredLocations();
                    break;

                case "fildirs":
                    PrintFilteredDirections();
                    break;

                default:
                    Console.WriteLine("Filters demo GPS track with bad data.");
                    Console.WriteLine("  demo rawlocs | rawdirs | fillocs | fildirs");
                    Console.WriteLine("  rawlocs -- locations (latitude and longitude)");
                    Console.WriteLine("             before filtration");
                    Console.WriteLine("  rawdirs -- directions (lat, long, and direction)");
                    Console.WriteLine("             before filtration");
                    Console.WriteLine("  fillocs -- locations (latitude and longitude)");
                    Console.WriteLine("             after filtration");
                    Console.WriteLine("  fildirs -- locations (lat, long, and direction)");
                    Console.WriteLine("             after filtration");
                    break;
            }

        }

        private static void PrintRawLocations()
        {
            PrintPoints(Examples.Locations);
        }

        private static void PrintRawDirections()
        {
            var locations = Examples.Directions
                                    .Select(x => new Location(x.Latitude, x.Longitude, x.Timestamp));
            
            PrintPoints(locations);
        }

        private static void PrintFilteredLocations()
        {
            var filter = new GpsTrackFilter();
            var track = filter.Filter(Examples.Locations);
            PrintPoints(track);
        }

        private static void PrintFilteredDirections()
        {
            var filter = new GpsTrackFilter();
            var track = filter.Filter(Examples.Directions)
                              .Select(x => new Location(x.Latitude, x.Longitude, x.Timestamp));;
            PrintPoints(track);
        }

        static void PrintPoints(IEnumerable<Location> points)
        {
            var multiLineString = new
            {
                type = "FeatureCollection",
                features = new[]
                {
                    new
                    {
                        type = "Feature",
                        properties = new { },
                        geometry = new
                        {
                            type = "MultiLineString",
                            coordinates = new []
                            {
                                points.Select(x => new []{x.Longitude, x.Latitude}),
                            },
                        }
                    }
                }
            };

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            var geoJson = JsonConvert.SerializeObject(multiLineString, settings);

            Console.Write(geoJson);
        }
    }
}
