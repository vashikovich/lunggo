﻿using System.Collections.Generic;

namespace Lunggo.ApCommon.Flight.Model.Logic
{
    public class RevalidateFlightInput
    {
        public string FareId { get; set; }
        public List<FlightTripInfo> TripInfos { get; set; }
    }
}
