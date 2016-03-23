﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Model.Logic;

namespace Lunggo.ApCommon.Flight.Service
{
    public partial class FlightService
    {
        public RevalidateFlightOutput RevalidateFlight(RevalidateFlightInput input)
        {
            var output = new RevalidateFlightOutput();
            if (input.Token == null)
            {
                output.IsSuccess = true;
                output.IsValid = false;
                output.NewFare = null;
                output.Sets = null;
                output.Token = null;
                return output;
            }
            else
            {
                var itins = IsItinBundleCacheId(input.Token)
                    ? GetItinerarySetFromCache(input.Token)
                    : new List<FlightItinerary> {GetItineraryFromCache(input.Token)};
                Parallel.ForEach(itins, itin =>
                {
                    var outputSet = new RevalidateFlightOutputSet();
                    var request = new RevalidateConditions
                    {
                        FareId = itin.FareId,
                        Trips = itin.Trips
                    };
                    var response = RevalidateFareInternal(request);
                    if (response.IsSuccess)
                    {
                        outputSet.IsSuccess = true;
                        outputSet.IsValid = response.IsValid;
                        outputSet.Itinerary = response.Itinerary;
                        if (outputSet.Itinerary != null)
                            outputSet.Itinerary.RegisterNumber = itin.RegisterNumber;
                    }
                    else
                    {
                        outputSet.IsSuccess = false;
                        response.Errors.ForEach(output.AddError);
                        if (response.ErrorMessages != null)
                            response.ErrorMessages.ForEach(output.AddError);
                    }
                    output.Sets.Add(outputSet);
                });

                if (output.Sets.TrueForAll(set => set.IsSuccess))
                {
                    var newItins = output.Sets.Select(set => set.Itinerary).ToList();
                    var searchId = output.Sets[0].Itinerary.SearchId;
                    var tripType = ParseTripType(searchId);
                    newItins.ForEach(itin => itin.RequestedTripType = tripType);
                    AddPriceMargin(newItins);
                    var searchedPrices = GetFlightRequestPrices(searchId);
                    var itinsPriceDifference = newItins.Select(itin => itin.LocalPrice - searchedPrices[itin.RegisterNumber]);
                    if (IsItinBundleCacheId(input.Token))
                        SaveItinerarySetAndBundleToCache(newItins, BundleItineraries(newItins), input.Token);
                    else
                        SaveItineraryToCache(newItins.Single(), input.Token);

                    output.IsSuccess = true;
                    output.IsValid = output.Sets.TrueForAll(set => set.IsValid) && itinsPriceDifference.All(diff => diff == 0);
                    if (output.Sets.Any(set => set.Itinerary == null))
                        output.NewFare = null;
                    else
                        output.NewFare = output.Sets.Sum(set => set.Itinerary.LocalPrice);
                    output.Token = input.Token;
                }
                else
                {
                    if (output.Sets.Any(set => set.IsSuccess))
                        output.PartiallySucceed();
                    output.IsSuccess = false;
                    output.DistinguishErrors();
                }
                return output;
            }
        }
    }
}
