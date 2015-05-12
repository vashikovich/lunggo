﻿using System;
using System.Collections.Generic;
using Lunggo.ApCommon.Constant;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Utility;
using Lunggo.ApCommon.Mystifly;
using Lunggo.ApCommon.Mystifly.OnePointService.Flight;
using Lunggo.Framework.Config;
using Lunggo.Framework.Redis;

namespace Lunggo.ApCommon.Flight.Service
{
    public partial class FlightService
    {
        private static readonly FlightService Instance = new FlightService();
        private static readonly MystiflyWrapper MystiflyWrapper = MystiflyWrapper.GetInstance();
        private bool _isInitialized;

        private FlightService()
        {
            
        }

        public static FlightService GetInstance()
        {
            return Instance;
        }

        public void Init()
        {
            if (!_isInitialized)
            {
                MystiflyWrapper.Init();
                _isInitialized = true;
            }
            else
            {
                throw new InvalidOperationException("FlightService is already initialized");
            }
        }

        private SearchFlightResult SearchFlightInternal(SearchFlightConditions conditions)
        {
            return MystiflyWrapper.SearchFlight(conditions);
        }

        private SearchFlightResult SpecificSearchFlightInternal(SpecificSearchConditions conditions)
        {
            return MystiflyWrapper.SpecificSearchFlight(conditions);
        }

        private RevalidateFareResult RevalidateFareInternal(RevalidateConditions conditions)
        {
            return MystiflyWrapper.RevalidateFare(conditions);
        }

        private BookFlightResult BookFlightInternal(FlightBookingInfo bookInfo)
        {
            var supplier = FlightIdUtil.GetSupplier(bookInfo.FareId);
            switch (supplier)
            {
                case FlightSupplier.Mystifly:
                    return MystiflyWrapper.BookFlight(bookInfo);
                default:
                    return null;
            }
            
        }

        private OrderTicketResult OrderTicketInternal(string bookingId)
        {
            var supplier = FlightIdUtil.GetSupplier(bookingId);
            switch (supplier)
            {
                case FlightSupplier.Mystifly:
                    return MystiflyWrapper.OrderTicket(bookingId);
                default:
                    return null;
            }
        }

        private GetTripDetailsResult GetTripDetailsInternal(TripDetailsConditions conditions)
        {
            return MystiflyWrapper.GetTripDetails(conditions);
        }

        private List<BookingStatusInfo> GetBookingStatusInternal()
        {
            return MystiflyWrapper.GetBookingStatus();
        }

        private CancelBookingResult CancelBookingInternal(string bookingId)
        {
            return MystiflyWrapper.CancelBooking(bookingId);
        }

        private GetRulesResult GetRulesInternal(string fareId)
        {
            return MystiflyWrapper.GetRules(fareId);
        }
    }
}
