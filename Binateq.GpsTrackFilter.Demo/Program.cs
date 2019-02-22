using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Binateq.GpsTrackFilter.Demo
{
    class Program
    {
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
                    break;
            }

        }

        private static void PrintRawLocations()
        {
            PrintPoints(Examples.Locations
                                .OfType<ILocation>());
        }

        private static void PrintRawDirections()
        {
            PrintPoints(Examples.Directions
                                .OfType<ILocation>());
        }

        private static void PrintFilteredLocations()
        {
            var filter = new GpsTrackFilter();
            var track = filter.Filter(Examples.Locations)
                              .OfType<ILocation>();
            PrintPoints(track);
        }

        private static void PrintFilteredDirections()
        {
            var filter = new GpsTrackFilter();
            var track = filter.Filter(Examples.Directions)
                              .OfType<ILocation>();
            PrintPoints(track);
        }

        static void PrintPoints(IEnumerable<ILocation> points)
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
