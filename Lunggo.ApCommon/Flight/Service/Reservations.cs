﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Lunggo.ApCommon.Flight.Constant;

using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.ApCommon.Product.Constant;
using Lunggo.Framework.Config;

namespace Lunggo.ApCommon.Flight.Service
{
    public partial class FlightService
    {
        public FlightItineraryForDisplay GetItineraryForDisplay(string token)
        {
            var itins = GetItinerariesFromCache(token);
            return ConvertToItineraryForDisplay(itins);
        }

        public FlightReservationForDisplay GetReservationForDisplay(string rsvNo)
        {
            try
            {
                var rsv = GetReservation(rsvNo);
                return ConvertToReservationForDisplay(rsv);
            }
            catch
            {
                return null;
            }
        }

        internal FlightReservation GetReservation(string rsvNo)
        {
            try
            {
                return GetReservationFromDb(rsvNo);
            }
            catch
            {
                return null;
            }
        }

        public FlightReservationForDisplay GetOverviewReservation(string rsvNo)
        {
            try
            {
                var rsv = GetOverviewReservationFromDb(rsvNo);
                return ConvertToReservationForDisplay(rsv);
            }
            catch
            {
                return null;
            }
        }

        public List<FlightReservationForDisplay> GetOverviewReservationsByContactEmail(string contactEmail)
        {
            var rsvs = GetOverviewReservationsFromDb(contactEmail) ?? new List<FlightReservation>();
            return rsvs.Select(ConvertToReservationForDisplay).ToList();
        }

        public List<FlightReservationForDisplay> SearchReservations(FlightReservationSearch search)
        {
            var rsvs = GetSearchReservationsFromDb(search);
            return rsvs.Select(ConvertToReservationForDisplay).ToList();
        }

        public void ExpireReservations()
        {
            UpdateExpireReservationsToDb();
        }

        internal void CancelReservation(string rsvNo, CancellationType cancellationType)
        {
            UpdateCancelReservationToDb(rsvNo, cancellationType);
        }

        //internal void ConfirmReservationRefund(string rsvNo, Refund refund)
        //{
        //    UpdateConfirmRefundToDb(rsvNo, refund);
        //}

        public byte[] GetEticket(string rsvNo)
        {
            var azureConnString = ConfigManager.GetInstance().GetConfigValue("azureStorage", "connectionString");
            var storageName = azureConnString.Split(';')[1].Split('=')[1];
            var url = @"https://" + storageName + @".blob.core.windows.net/eticket/" + rsvNo + ".pdf";
            var client = new WebClient();
            try
            {
                return client.DownloadData(url);
            }
            catch
            {
                return null;
            }
        }

        //public void UpdateIssueProgress(string rsvNo, string progressMessage)
        //{
        //    UpdateIssueProgressToDb(rsvNo, progressMessage);
        //}

        public List<Tuple<FlightTripForDisplay, BookingStatus>> GetAllBookingStatus(string rsvNo)
        {
            var rsv = GetReservation(rsvNo);
            if (rsv == null)
                return null;
            return
                rsv.Itineraries.SelectMany(
                    itin =>
                        itin.Trips.Select(
                            trip =>
                                new Tuple<FlightTripForDisplay, BookingStatus>(ConvertToTripForDisplay(trip),
                                    itin.BookingStatus))).ToList();
        }
    }
}
