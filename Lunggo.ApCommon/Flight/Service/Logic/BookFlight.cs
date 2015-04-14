﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Mystifly.OnePointService.Flight;

namespace Lunggo.ApCommon.Flight.Service
{
    public partial class FlightService
    {
        public BookFlightOutput BookFlight(BookFlightInput input)
        {
            var output = new BookFlightOutput();
            var bookInfo = new FlightBookingInfo
            {
                FareId = input.BookingInfo.FareId,
                ContactData = new ContactData
                {
                    Email = input.BookingInfo.ContactData.Email,
                    Phone = input.BookingInfo.ContactData.Phone
                },
                PassengerFareInfos = input.BookingInfo.PassengerFareInfos
            };
            var response = BookFlightInternal(bookInfo);
            output.BookResult = new BookResult();
            if (response.IsSuccess)
            {
                output.IsSuccess = true;
                output.BookResult.BookingId = response.Status.BookingId;
                output.BookResult.BookingStatus = response.Status.BookingStatus;
                if (response.Status.BookingStatus == BookingStatus.Booked)
                    output.BookResult.TimeLimit = response.Status.TimeLimit;
                // TODO InsertBookingDbQuery
            }
            else
            {
                output.IsSuccess = false;
                output.BookResult = null;
                output.Errors = response.Errors;
                output.ErrorMessages = response.ErrorMessages;
            }
            return output;
        }
    }
}
