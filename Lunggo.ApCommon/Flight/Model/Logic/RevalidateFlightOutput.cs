﻿namespace Lunggo.ApCommon.Flight.Model.Logic
{
    public class RevalidateFlightOutput : OutputBase
    {
        public bool IsValid { get; set; }
        public FlightItinerary Itinerary { get; set; }
    }
}
