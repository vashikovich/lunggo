﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunggo.ApCommon.Flight.Model
{
    public class RevalidateFareResult : ResultBase
    {
        internal bool IsValid { get; set; }
        internal FlightItinerary Itinerary { get; set; }
    }
}
