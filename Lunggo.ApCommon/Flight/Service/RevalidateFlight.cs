﻿using Lunggo.ApCommon.Flight.Model;

namespace Lunggo.ApCommon.Flight.Service
{
    public partial class FlightService
    {
        public RevalidateFlightOutput RevalidateFlight(RevalidateFlightInput input)
        {
            var output = new RevalidateFlightOutput();
            var request = new RevalidateConditions
            {
                FareId = input.FareId,
                TripInfos = input.TripInfos
            };
            var response = RevalidateFareInternal(request);
            if (response.IsSuccess)
            {
                output.IsSuccess = true;
                output.IsValid = response.IsValid;
                output.Itinerary = response.Itinerary;
            }
            else
            {
                output.IsSuccess = false;
                output.Errors = response.Errors;
                output.ErrorMessages = response.ErrorMessages;
            }
            return output;
        }
    }
}