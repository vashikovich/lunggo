﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunggo.Flight.Query;
using Lunggo.Framework.Database;
using Lunggo.Repository.TableRecord;
using Lunggo.Repository.TableRepository;

namespace Lunggo.Flight.Model
{
    public class FlightReservationIntegrated
    {
        public FlightReservationsTableRecord Reservation { get; set; }
        public List<FlightTripTableRecord> Trip { get; set; }
        public Dictionary<long,List<FlightTripDetailTableRecord>> TripDetail { get; set; }
        public Dictionary<long,List<FlightPassengerTableRecord>> Passenger { get; set; }

        public FlightReservationIntegrated()
        {
            Trip = new List<FlightTripTableRecord>();
            TripDetail = new Dictionary<long, List<FlightTripDetailTableRecord>>();
            Passenger = new Dictionary<long, List<FlightPassengerTableRecord>>();
        }
        /*
        public static IEnumerable<FlightReservationIntegrated> GetFromDb(FlightReservationSearch search, QueryType queryType)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var rsvNoList = new List<string>();
                if (search.RsvNo != null)
                    rsvNoList.Add(search.RsvNo);
                else
                    rsvNoList.AddRange(GetFlightRsvNoQuery.GetInstance().Execute(conn, search, search));
                foreach (var rsvNo in rsvNoList)
                {
                    var integrated = new FlightReservationIntegrated();
                    integrated.Reservation =
                        GetFlightReservationQuery.GetInstance()
                            .Execute(conn, new {RsvNo = rsvNo}, queryType)
                            .First();
                    integrated.Trip =
                        GetFlightTripQuery.GetInstance().Execute(conn, new {RsvNo = rsvNo}, queryType).ToList();
                    foreach (var id in integrated.Trip.Select(trip => trip.TripId.GetValueOrDefault()))
                    {
                        integrated.TripDetail.Add(id,
                            GetFlightTripDetailQuery.GetInstance()
                                .Execute(conn, new {TripId = id}, queryType)
                                .ToList());
                        integrated.Passenger.Add(id,
                            GetFlightPassengerQuery.GetInstance()
                                .Execute(conn, new {TripId = id}, queryType)
                                .ToList());
                    }
                    yield return integrated;
                }
            }
        }
        */
        public enum QueryType
        {
            Overview = 0,
            Complete,
            PrimKeys
        }

        public static IEnumerable<FlightReservationIntegrated> GetFromDb(FlightReservationSearch search, QueryType queryType)
        {
            List<FlightReservationIntegrated> integratedList;
            var integratedLookup = new List<FlightReservationIntegrated>();
            var condition = new {Param = search, QueryType = queryType};
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                integratedList = GetFlightReservationExcPassengerQuery.GetInstance()
                    .ExecuteMultiMap(conn, search, condition,
                        (reservation, trip, tripDetail) =>
                        {
                            var integrated =
                                integratedLookup.SingleOrDefault(x => x.Reservation.RsvNo == reservation.RsvNo);
                            var tripId = trip.TripId.GetValueOrDefault();
                            if (integrated == null)
                            {
                                integrated = new FlightReservationIntegrated();
                                integrated.Reservation = reservation;
                                integrated.Trip.Add(trip);
                                integrated.TripDetail.Add(tripId, new List<FlightTripDetailTableRecord>());
                                integrated.TripDetail[tripId].Add(tripDetail);
                                integratedLookup.Add(integrated);
                            }
                            else
                            {
                                var integratedTrip = integrated.Trip.SingleOrDefault(x => x.TripId == trip.TripId);
                                if (integratedTrip == null)
                                {
                                    integrated.Trip.Add(trip);
                                    integrated.TripDetail.Add(tripId, new List<FlightTripDetailTableRecord>());
                                    integrated.TripDetail[tripId].Add(tripDetail);
                                }
                                else
                                {
                                    integrated.TripDetail[tripId].Add(tripDetail);
                                }
                            }
                            foreach (var id in integrated.Trip.Select(tripItem => tripItem.TripId.GetValueOrDefault()))
                            {
                                integrated.TripDetail[id].Sort(
                                    (x, y) => x.SequenceNo.GetValueOrDefault() - y.SequenceNo.GetValueOrDefault());
                            }
                            return integrated;
                        }, "TripId,TripDetailId").Distinct().ToList();
                foreach (var integrated in integratedList)
                {
                    foreach (var trip in integrated.Trip)
                    {
                        integrated.Passenger.Add(trip.TripId.GetValueOrDefault(),
                            GetFlightPassengerQuery.GetInstance().Execute(conn, new { tripId = trip.TripId.GetValueOrDefault(), search.PassengerName }, new { QueryType = queryType }).ToList());
                    }
                }
            }
            return integratedList;
        }

        public void DeleteFromDb()
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                foreach (var trip in Trip)
                {
                    var tripId = trip.TripId.GetValueOrDefault();
                    foreach (var passenger in Passenger[tripId])
                    {
                        FlightPassengerTableRepo.GetInstance().Delete(conn, passenger);
                    }
                    foreach (var tripDetail in TripDetail[tripId])
                    {
                        FlightTripDetailTableRepo.GetInstance().Delete(conn, tripDetail);
                    }
                    FlightTripTableRepo.GetInstance().Delete(conn, trip);
                }
                FlightReservationsTableRepo.GetInstance().Delete(conn, Reservation);
            }
        }

    }
}
