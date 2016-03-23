﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Lunggo.ApCommon.Flight.Model
{
    public class FlightTripForDisplay : FlightTripBase
    {
        [JsonProperty("trnscnt")]
        public int TotalTransit { get; set; }
        [JsonProperty("trns")]
        public List<Transit> Transits { get; set; }
        [JsonProperty("air")]
        public List<Airline> Airlines { get; set; }
        [JsonProperty("dur")]
        public TimeSpan TotalDuration { get; set; }
    }

    public class FlightTrip : FlightTripBase
    {

    }

    public class FlightTripBase
    {
        [JsonProperty("ori")]
        public string OriginAirport { get; set; }
        [JsonProperty("des")]
        public string DestinationAirport { get; set; }
        [JsonProperty("desct")]
        public string DestinationCity { get; set; }
        [JsonProperty("desnm")]
        public string DestinationAirportName { get; set; }
        [JsonProperty("orict")]
        public string OriginCity { get; set; }
        [JsonProperty("orinm")]
        public string OriginAirportName { get; set; }
        [JsonProperty("dt")]
        public DateTime DepartureDate { get; set; }
        [JsonProperty("seg")]
        public List<FlightSegment> Segments { get; set; }

        public bool Identical(FlightTrip otherTrip)
        {
            if (OriginAirport != otherTrip.OriginAirport ||
                DestinationAirport != otherTrip.DestinationAirport ||
                DepartureDate != otherTrip.DepartureDate ||
                Segments == null ||
                otherTrip.Segments == null ||
                Segments.Count != otherTrip.Segments.Count)
                return false;
            for (var i = 0; i < Segments.Count; i++)
            {
                var segment = Segments[i];
                var otherSegment = otherTrip.Segments[i];
                if (segment.DepartureAirport != otherSegment.DepartureAirport ||
                    segment.ArrivalAirport != otherSegment.ArrivalAirport ||
                    segment.DepartureTime != otherSegment.DepartureTime ||
                    segment.ArrivalTime != otherSegment.ArrivalTime ||
                    segment.Duration != otherSegment.Duration ||
                    segment.AirlineCode != otherSegment.AirlineCode ||
                    segment.FlightNumber != otherSegment.FlightNumber ||
                    segment.CabinClass != otherSegment.CabinClass)
                    return false;
            }
            return true;
        }
    }

    public class Airline
    {
        [JsonProperty("cd")]
        public string Code { get; set; }
        [JsonProperty("nm")]
        public string Name { get; set; }
        [JsonProperty("lg")]
        public string LogoUrl { get; set; }
    }

    public class Transit
    {
        [JsonProperty("stp")]
        public bool IsStop { get; set; }
        [JsonProperty("air")]
        public string Airport { get; set; }
        [JsonProperty("arr")]
        public DateTime ArrivalTime { get; set; }
        [JsonProperty("dep")]
        public DateTime DepartureTime { get; set; }
        [JsonProperty("dur")]
        public TimeSpan Duration { get; set; }
    }


}
