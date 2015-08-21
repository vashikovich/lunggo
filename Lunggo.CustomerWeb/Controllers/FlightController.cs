﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.ModelBinding;
using System.Web.Mvc;
using Lunggo.ApCommon.Constant;
using Lunggo.ApCommon.Dictionary;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Model.Logic;
using Lunggo.ApCommon.Flight.Service;
using Lunggo.ApCommon.Flight.Database;
using Lunggo.ApCommon.Payment;
using Lunggo.ApCommon.Payment.Constant;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.CustomerWeb.Models;
using Lunggo.Framework.Filter;
using Lunggo.Framework.Payment.Data;
using Lunggo.Framework.Database;
using Lunggo.Framework.Redis;
using RestSharp.Serializers;

namespace Lunggo.CustomerWeb.Controllers
{
    public class FlightController : Controller
    {
        [DeviceDetectionFilter]
        public ActionResult SearchResultList(FlightSearchData search)
        {
            return View(search);
        }

        public ActionResult Checkout(FlightSelectData select)
        {
            var service = FlightService.GetInstance();
            var itinerary = service.GetItineraryFromCache(select.token);
            var itineraryApi = service.ConvertToItineraryApi(itinerary);
            return View(new FlightCheckoutData
            {
                HashKey = select.token,
                ItineraryApi = itineraryApi,
            });
        }

        [HttpPost]
        public ActionResult Checkout(FlightCheckoutData data)
        {
            data.Itinerary = FlightService.GetInstance().GetItineraryFromCache(data.HashKey);
            var passengerInfo = data.Passengers.Select(passenger => new PassengerInfoFare
            {
                Type = passenger.Type,
                Title = passenger.Title,
                FirstName = passenger.FirstName,
                LastName = passenger.LastName,
                Gender = passenger.Title == Title.Mister ? Gender.Male : Gender.Female,
                DateOfBirth = passenger.BirthDate,
                PassportNumber = passenger.PassportNumber,
                PassportExpiryDate = passenger.PassportExpiryDate,
                PassportCountry = passenger.Country
            });
            var bookInfo = new BookFlightInput
            {
                ContactData = new ContactData
                {
                    Name = data.Contact.Name,
                    CountryCode = data.Contact.CountryCode,
                    Phone = data.Contact.Phone,
                    Email = data.Contact.Email
                },
                PassengerInfoFares = passengerInfo.ToList(),
                Itinerary = data.Itinerary,
                Trips = new List<FlightTrip>
                {
                    new FlightTrip
                    {
                        OriginAirport = data.Itinerary.FlightTrips[0].OriginAirport,
                        DestinationAirport = data.Itinerary.FlightTrips[0].DestinationAirport,
                        DepartureDate = data.Itinerary.FlightTrips[0].DepartureDate
                    }
                },
                OverallTripType = data.Itinerary.TripType,
                DiscountCode = data.DiscountCode
            };
            var bookResult = FlightService.GetInstance().BookFlight(bookInfo);
            if (bookResult.IsSuccess && bookResult.BookResult.BookingStatus == BookingStatus.Booked)
            {
                var transactionDetails = new TransactionDetails
                {
                    OrderId = bookResult.RsvNo,
                    Amount = bookResult.FinalPrice
                };
                var itemDetails = new List<ItemDetails>
                    {
                        new ItemDetails
                        {
                            Id = "1",
                            Name = 
                                data.Itinerary.TripType + " " +
                                data.Itinerary.FlightTrips[0].OriginAirport + "-" +
                                data.Itinerary.FlightTrips[0].DestinationAirport + " " +
                                data.Itinerary.FlightTrips[0].DepartureDate.ToString("d MMM yy") +
                                (data.Itinerary.TripType == TripType.Return
                                ? "-" + data.Itinerary.FlightTrips[1].DepartureDate.ToString("d MMM yy")
                                : ""),
                            Quantity = 1,
                            Price = bookResult.FinalPrice
                        }
                    };

                var url = PaymentService.GetInstance().GetPaymentUrl(transactionDetails, itemDetails, data.Payment.Method);
                if (url == null && data.Payment.Method == PaymentMethod.BankTransfer)
                    return RedirectToAction("Confirmation", "Flight", new {RsvNo = bookResult.RsvNo});
                else
                    return Redirect(url);
            }
            else
            {
                return RedirectToAction("Checkout", new FlightSelectData
                {
                    token = data.HashKey,
                    error = bookResult.Errors[0]
                });
            }
        }

        public ActionResult Thankyou(string rsvNo)
        {
            var service = FlightService.GetInstance();
            var summary = service.GetReservation(rsvNo);
            return View(summary);
        }

        public ActionResult Confirmation()
        {
            return View();
        }
    }
}