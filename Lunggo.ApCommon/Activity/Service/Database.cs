﻿using System;
using System.Collections.Generic;
using Lunggo.ApCommon.Activity.Model;
using Lunggo.Framework.Database;
using System.Linq;
using Lunggo.ApCommon.Activity.Database.Query;
using Lunggo.ApCommon.Activity.Model.Logic;
using Lunggo.ApCommon.Flight.Constant;
using Lunggo.ApCommon.Product.Constant;
using Lunggo.ApCommon.Sequence;
using Lunggo.Repository.TableRecord;
using Lunggo.Repository.TableRepository;

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
                
                //for (int i = 0; i < savedActivities.Count; i++)
                //{
                //    var id = savedActivities[i].Id;
                //    var mediaSrc = GetMediaActivityDetailQuery.GetInstance()
                //        .Execute(conn, new { ActivityId = id }).ToList();
                //    savedActivities[i].MediaSrc = mediaSrc[0];
                //}

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
                //var details = GetActivityDetailQuery.GetInstance()
                //    .ExecuteMultiMap(conn, new { ActivityId = input.ActivityId}, null, (detail, mediaSrc) =>
                //    {
                //        detail.MediaSrc = new List<string> {mediaSrc};
                //        return detail;
                //    }, "MediaSrc").ToList();

                var details = GetActivityDetailQuery.GetInstance().ExecuteMultiMap(conn, new { ActivityId = input.ActivityId }, null,
                        (detail, duration, content) =>
                        {
                            detail.Duration = duration;
                            detail.Contents = content;
                            return detail;
                        }, "Amount, Content1");

                var mediaSrc = GetMediaActivityDetailQuery.GetInstance()
                    .Execute(conn, new { ActivityId = input.ActivityId }).ToList();

                var additionalContentsDetail = GetAdditionalContentActivityDetailQuery.GetInstance()
                    .Execute(conn, new { ActivityId = input.ActivityId });

                var activityDetail = details.First();

                activityDetail.MediaSrc = mediaSrc;
                activityDetail.AdditionalContent = additionalContentsDetail.Select(a => new AdditionalContent()
                {
                    Title = a.Title,
                    Description = a.Description
                }).ToList();

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
                var savedActivities = GetAvailableDatesQuery.GetInstance()
                    .Execute(conn, new { ActivityId = input.ActivityId });

                var output = new GetAvailableDatesOutput
                {
                    AvailableDates = savedActivities.Select(a => new ActivityDetail()
                    {
                        Date = a.Date
                    }).ToList()
                };
                return output;
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
                    Date = reservation.ActivityDetails.Date

                };

                ActivityReservationTableRepo.GetInstance().Insert(conn, activityRecord);
                ReservationTableRepo.GetInstance().Insert(conn, reservationRecord);
                reservation.Contact.InsertToDb(reservation.RsvNo);
                reservation.State.InsertToDb(reservation.RsvNo);
                reservation.Payment.InsertToDb(reservation.RsvNo);
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

        #endregion
    }
}