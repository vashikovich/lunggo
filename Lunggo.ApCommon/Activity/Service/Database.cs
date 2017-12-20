﻿using System;
using System.Collections.Generic;
using Lunggo.ApCommon.Activity.Model;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.Framework.Database;
using System.Linq;
using Lunggo.ApCommon.Activity.Database.Query;
using Lunggo.ApCommon.Activity.Model.Logic;
using Lunggo.ApCommon.Product.Constant;
using Lunggo.ApCommon.Sequence;
using Lunggo.Repository.TableRecord;
using Lunggo.Repository.TableRepository;
using Lunggo.ApCommon.Product.Model;
using Lunggo.ApCommon.Payment.Model;
using System.Web;
using Lunggo.ApCommon.Identity.Query;
using Lunggo.ApCommon.Identity.Users;
using BookingStatus = Lunggo.ApCommon.Activity.Constant.BookingStatus;
using BookingStatusCd = Lunggo.ApCommon.Activity.Constant.BookingStatusCd;

namespace Lunggo.ApCommon.Activity.Service
{
    public partial class ActivityService
    {
        #region Get
        public SearchActivityOutput GetActivitiesFromDb(SearchActivityInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                string endDate = input.ActivityFilter.EndDate.ToString("yyyy/MM/dd");
                string startDate = input.ActivityFilter.StartDate.ToString("yyyy/MM/dd");

                var savedActivities = GetSearchResultQuery.GetInstance()
                    .ExecuteMultiMap(conn, new { Name = input.ActivityFilter.Name, StartDate = startDate, EndDate = endDate, Page = input.Page, PerPage = input.PerPage },
                    null, (activities, duration) =>
                        {
                            activities.Duration = duration;
                            return activities;
                        }, "Amount").ToList();
                
                var output = new SearchActivityOutput
                {
                    ActivityList = savedActivities,
                    Page = input.Page,
                    PerPage = input.PerPage
                };
                return output;
            }
        }

        public GetDetailActivityOutput GetActivityDetailFromDb(GetDetailActivityInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {

                var details = GetActivityDetailQuery.GetInstance().ExecuteMultiMap(conn, new { ActivityId = input.ActivityId }, null,
                        (detail, duration) =>
                        {
                            detail.Duration = duration;
                            return detail;
                        }, "Amount");

                var mediaSrc = GetMediaActivityDetailQuery.GetInstance()
                    .Execute(conn, new { ActivityId = input.ActivityId }).ToList();

                var additionalContentsDetail = GetAdditionalContentActivityDetailQuery.GetInstance()
                    .Execute(conn, new { ActivityId = input.ActivityId });

                var activityDetail = details.First();

                activityDetail.BookingStatus = BookingStatus.Booked;
                activityDetail.MediaSrc = mediaSrc;
                activityDetail.AdditionalContents = new AdditionalContent
                {
                    Title = "Keterangan Tambahan",
                    Contents = additionalContentsDetail.ToList()
                };

                var output = new GetDetailActivityOutput
                {
                    ActivityDetail = activityDetail
                };
                return output;
            }
        }

        public GetAvailableDatesOutput GetAvailableDatesFromDb(GetAvailableDatesInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var savedDates = GetAvailableDatesQuery.GetInstance()
                    .Execute(conn, new { ActivityId = input.ActivityId });

                var result = new List<DateAndAvailableHour>();

                foreach(var i in savedDates.ToList())
                {                   
                    var savedHours = GetAvailableSessionQuery.GetInstance()
                        .Execute(conn, new { ActivityId = input.ActivityId, Date = i.Date }).ToList();
                    if (savedHours.TrueForAll(e => e == null))
                        savedHours = null;
                    var savedDatesAndHours = new DateAndAvailableHour()
                    {
                        Date = i.Date,
                        AvailableHours = savedHours
                    };

                    result.Add(savedDatesAndHours);
                }
                
                var output = new GetAvailableDatesOutput
                {
                    AvailableDateTimes = result
                };
                return output;
            }
        }

        private ActivityReservation GetReservationFromDb(string rsvNo)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var reservationRecord = ReservationTableRepo.GetInstance()
                    .Find1(conn, new ReservationTableRecord { RsvNo = rsvNo });

                if (reservationRecord == null)
                    return null;

                var activityReservation = new ActivityReservation
                {
                    RsvNo = rsvNo,
                    Contact = Contact.GetFromDb(rsvNo),
                    Pax = new List<Pax>(),
                    Payment = PaymentDetails.GetFromDb(rsvNo),
                    State = ReservationState.GetFromDb(rsvNo),
                    ActivityDetails = new ActivityDetail(),
                    RsvTime = reservationRecord.RsvTime.GetValueOrDefault(),
                    RsvStatus = RsvStatusCd.Mnemonic(reservationRecord.RsvStatusCd)
                };

                if (activityReservation.Contact == null || activityReservation.Payment == null)
                    return null;

                var activityDetailRecord = ActivityReservationTableRepo.GetInstance()
                    .Find1(conn, new ActivityReservationTableRecord { RsvNo = rsvNo });

                var actDetail = GetActivityDetailFromDb(new GetDetailActivityInput() {ActivityId = activityDetailRecord.ActivityId });

                activityReservation.ActivityDetails = actDetail.ActivityDetail;
                activityReservation.DateTime = new DateAndSession()
                {
                    Date = activityDetailRecord.Date,
                    Session = activityDetailRecord.SelectedSession
                };
                    
                var paxRecords = PaxTableRepo.GetInstance()
                        .Find(conn, new PaxTableRecord { RsvNo = rsvNo }).ToList();

                if (paxRecords.Count != 0)
                    foreach (var passengerRecord in paxRecords)
                    {
                        var passenger = new Pax
                        {
                            Title = TitleCd.Mnemonic(passengerRecord.TitleCd),
                            FirstName = passengerRecord.FirstName,
                            LastName = passengerRecord.LastName,
                            Type = PaxTypeCd.Mnemonic(passengerRecord.TypeCd)
                        };
                        activityReservation.Pax.Add(passenger);
                    }

                return activityReservation;
            }
        }

        public GetMyBookingsOutput GetMyBookingsFromDb(GetMyBookingsInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var userName = HttpContext.Current.User.Identity.GetUser();
                
                var savedBookings = GetMyBookingsQuery.GetInstance()
                    .Execute(conn, new { UserId = userName.Id, Page = input.Page, PerPage = input.PerPage }).ToList();
                
                var output = new GetMyBookingsOutput
                {
                    MyBookings = savedBookings,
                    Page = input.Page,
                    PerPage = input.PerPage
                };
                return output;
            }
        }

        public GetMyBookingDetailOutput GetMyBookingDetailFromDb(GetMyBookingDetailInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                
                var savedBooking = GetMyBookingDetailQuery.GetInstance()
                    .Execute(conn, new { input.RsvNo }).First();

                var savedPassengers = GetPassengersQuery.GetInstance().ExecuteMultiMap(conn, new { input.RsvNo }, null,
                        (passengers, typeCd, titleCd, genderCd) =>
                        {
                            passengers.Type = PaxTypeCd.Mnemonic(typeCd);
                            passengers.Title = TitleCd.Mnemonic(titleCd);
                            passengers.Gender = GenderCd.Mnemonic(genderCd);
                            return passengers;
                        }, "TypeCd, TitleCd, GenderCd").ToList();
                savedBooking.Passengers = savedPassengers;

                var output = new GetMyBookingDetailOutput
                {
                    BookingDetail = savedBooking
                };
                return output;
            }
        }

        public GetAppointmentRequestOutput GetAppointmentRequestFromDb(GetAppointmentRequestInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var userName = HttpContext.Current.User.Identity.GetUser();

                var savedBookings = GetAppointmentRequestQuery.GetInstance()
                    .Execute(conn, new {UserId = userName.Id, Page = input.Page, PerPage = input.PerPage });

                var output = new GetAppointmentRequestOutput
                {
                    Appointments = savedBookings.Select(a => new AppointmentDetail()
                    {
                        ActivityId = a.ActivityId,
                        RsvNo = a.RsvNo,
                        Name = a.Name,
                        Date = a.Date,
                        Session = a.Session,
                        RequestTime = a.RequestTime,
                        PaxCount = a.PaxCount,
                        MediaSrc = a.MediaSrc
                    }).ToList(),
                    Page = input.Page,
                    PerPage = input.PerPage
                };
                return output;
            }
        }

        public GetAppointmentListOutput GetAppointmentListFromDb(GetAppointmentListInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var userName = HttpContext.Current.User.Identity.GetUser();

                var savedBookings = GetAppointmentListQuery.GetInstance()
                    .Execute(conn, new { UserId = userName.Id, Page = input.Page, PerPage = input.PerPage });

                var output = new GetAppointmentListOutput
                {
                    Appointments = savedBookings.Select(a => new AppointmentDetail()
                    {
                        ActivityId = a.ActivityId,
                        Name = a.Name,
                        Date = a.Date,
                        Session = a.Session,
                        RequestTime = a.RequestTime,
                        PaxCount = a.PaxCount,
                        MediaSrc = a.MediaSrc
                    }).ToList(),
                    Page = input.Page,
                    PerPage = input.PerPage
                };
                return output;
            }
        }

        public GetListActivityOutput GetListActivityFromDb(GetListActivityInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var userName = HttpContext.Current.User.Identity.GetUser();

                var savedBookings = GetListActivityQuery.GetInstance()
                    .Execute(conn, new { UserId = userName.Id, Page = input.Page, PerPage = input.PerPage });

                var output = new GetListActivityOutput
                {
                    ActivityList = savedBookings.Select(a => new SearchResult()
                    {
                        Id = a.Id,
                        Name = a.Name,
                        MediaSrc = a.MediaSrc
                    }).ToList(),
                    Page = input.Page,
                    PerPage = input.PerPage
                };
                return output;
            }
        }

        public GetAppointmentDetailOutput GetAppointmentDetailFromDb(GetAppointmentDetailInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var userName = HttpContext.Current.User.Identity.GetUser();

                string date = input.Date.ToString("yyyy/MM/dd");

                var savedAppointments = GetAppointmentDetailQuery.GetInstance()
                    .Execute(conn, new { ActivityId = input.ActivityId, Date = date, Session = input.Session }, new { Session = input.Session });
                var savedAppointment = savedAppointments.First();

                var appointmentDetail = new AppointmentDetail()
                {
                    ActivityId = savedAppointment.ActivityId,
                    Name = savedAppointment.Name,
                    Date = savedAppointment.Date,
                    Session = savedAppointment.Session,
                    MediaSrc = savedAppointment.MediaSrc,
                    PaxGroup = new PaxGroup()
                };
                
                foreach(var appointment in savedAppointments.ToList())
                {
                    var savedPassengers = GetPassengersQuery.GetInstance().ExecuteMultiMap(conn, new { RsvNo = appointment.RsvNo }, null,
                        (passengers, typeCd, titleCd, genderCd) =>
                        {
                            passengers.Type = PaxTypeCd.Mnemonic(typeCd);
                            passengers.Title = TitleCd.Mnemonic(titleCd);
                            passengers.Gender = GenderCd.Mnemonic(genderCd);
                            return passengers;
                        }, "TypeCd, TitleCd, GenderCd").ToList();
                    var contact = Contact.GetFromDb(appointment.RsvNo);
                    var paxgroup = new PaxGroup()
                    {
                        Contact = contact,
                        Passengers = ConvertToPaxForDisplay(savedPassengers)
                    };

                    appointmentDetail.PaxGroup = paxgroup;
                }
                
                var output = new GetAppointmentDetailOutput
                {
                    AppointmentDetail = appointmentDetail
                };
                return output;
            }
        }

        internal List<ActivityReservation> GetBookedActivitiesFromDb()
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var rsvNos = GetBookedActivitiesRsvNoQuery.GetInstance().Execute(conn, null);
                var reservations = rsvNos.Select(GetReservationFromDb).ToList();
                return reservations;
            }
        }

        #endregion

        #region Insert

        private void InsertActivityRsvToDb(ActivityReservation reservation)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var reservationRecord = new ReservationTableRecord
                {
                    RsvNo = reservation.RsvNo,
                    RsvTime = reservation.RsvTime.ToUniversalTime(),
                    RsvStatusCd = RsvStatusCd.Mnemonic(reservation.RsvStatus),
                    CancellationTypeCd = null,
                    UserId = reservation.User != null ? reservation.User.Id : null,
                    InsertBy = "LunggoSystem",
                    InsertDate = DateTime.UtcNow,
                    InsertPgId = "0"
                };

                var activityRecord = new ActivityReservationTableRecord
                {
                    Id = ActivityReservationIdSequence.GetInstance().GetNext(),
                    RsvNo = reservation.RsvNo,
                    ActivityId = reservation.ActivityDetails.ActivityId,
                    BookingStatusCd = BookingStatusCd.Mnemonic(reservation.ActivityDetails.BookingStatus),
                    Date = reservation.DateTime.Date,
                    SelectedSession = reservation.DateTime.Session,
                    TicketCount = reservation.TicketCount,
                    UserId = reservation.User.Id
                };

                ActivityReservationTableRepo.GetInstance().Insert(conn, activityRecord);
                ReservationTableRepo.GetInstance().Insert(conn, reservationRecord);
                reservation.Contact.InsertToDb(reservation.RsvNo);
                reservation.State.InsertToDb(reservation.RsvNo);
                reservation.Payment.InsertToDb(reservation.RsvNo);
                if(reservation.Pax != null)
                {
                    foreach (var passenger in reservation.Pax)
                    {

                        var passengerRecord = new PaxTableRecord
                        {
                            Id = PaxIdSequence.GetInstance().GetNext(),
                            RsvNo = reservation.RsvNo,
                            TypeCd = PaxTypeCd.Mnemonic(passenger.Type),
                            GenderCd = GenderCd.Mnemonic(passenger.Gender),
                            TitleCd = TitleCd.Mnemonic(passenger.Title),
                            FirstName = passenger.FirstName,
                            LastName = passenger.LastName,
                            BirthDate = passenger.DateOfBirth.HasValue ? passenger.DateOfBirth.Value.ToUniversalTime() : (DateTime?)null,
                            NationalityCd = passenger.Nationality,
                            PassportNumber = passenger.PassportNumber,
                            PassportExpiryDate = passenger.PassportExpiryDate.HasValue ? passenger.PassportExpiryDate.Value.ToUniversalTime() : (DateTime?)null,
                            PassportCountryCd = passenger.PassportCountry,
                            InsertBy = "LunggoSystem",
                            InsertDate = DateTime.UtcNow,
                            InsertPgId = "0"
                        };
                        PaxTableRepo.GetInstance().Insert(conn, passengerRecord);
                    }
                }
                
            }
        }
        #endregion

        #region Update

        private void UpdateRsvStatusDb(string rsvNo, RsvStatus status)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                ReservationTableRepo.GetInstance().Update(conn, new ReservationTableRecord
                {
                    RsvNo = rsvNo,
                    RsvStatusCd = RsvStatusCd.Mnemonic(status)
                });
            }

        }

        private void UpdateActivityDb(ActivityUpdateInput input)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                ActivityTableRepo.GetInstance().Update(conn, new ActivityTableRecord
                {
                    Id = input.ActivityId,
                    Name = input.Name,
                    Category = input.Category,
                    Description = input.ShortDesc,
                    City = input.City,
                    Country = input.Country,
                    Address = input.Address,
                    Latitude = input.Latitude,
                    Longitude = input.Longitude,
                    PriceDetail = input.PriceDetail,
                    AmountDuration = int.Parse(input.Duration.Amount),
                    UnitDuration = input.Duration.Unit,
                    OperationTime = input.OperationTime,
                    //TODO: ImportantNotices, Warning, AdditionalNotes missing
                    Cancellation = input.Cancellation,
                    IsPaxDoBNeeded = input.IsPaxDoBNeeded,
                    IsPassportIssuedDateNeeded = input.IsPassportIssuedDateNeeded,
                    IsPassportNumberNeeded = input.IsPassportNumberNeeded 
                });
                UpdatePriceQuery.GetInstance().Execute(conn, new {Price = input.Price, ActivityId = input.ActivityId });
            }

        }
        #endregion

        private void UpdateActivityBookingStatusInDb(string rsvNo, BookingStatus bookingStatus)
        {
            var bookingStatusCd = BookingStatusCd.Mnemonic(bookingStatus);
            using (var conn = DbService.GetInstance().GetOpenConnection())
                UpdateActivityBookingStatusQuery.GetInstance().Execute(conn, new {rsvNo, bookingStatusCd});
        }
    }
}