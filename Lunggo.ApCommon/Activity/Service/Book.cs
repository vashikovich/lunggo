﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Lunggo.ApCommon.Activity.Constant;
using Lunggo.ApCommon.Activity.Model;
using Lunggo.ApCommon.Activity.Model.Logic;
using Lunggo.ApCommon.Identity.Auth;
using Lunggo.ApCommon.Identity.Users;
using Lunggo.ApCommon.Payment.Constant;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.ApCommon.Payment.Service;
using Lunggo.ApCommon.Product.Constant;
using Lunggo.ApCommon.Product.Model;
using Lunggo.ApCommon.Sequence;
using Lunggo.Framework.Config;
using Lunggo.Framework.Context;
using System.Collections.Generic;

namespace Lunggo.ApCommon.Activity.Service
{
    public partial class ActivityService
    {
        public BookActivityOutput BookActivity(BookActivityInput input)
        {
            var getDetail = GetActivityDetailFromDb(new GetDetailActivityInput()
            {
                ActivityId = Convert.ToInt32(input.ActivityId)
            });

            var packageIds = ActivityService.GetInstance().GetPackageIdFromDb(Convert.ToInt64(input.ActivityId)).ToList();
            if (!packageIds.Contains(input.PackageId))
            {
                return new BookActivityOutput
                {
                    IsValid = false
                };
            }

            var dateAndSession = ActivityService.GetInstance().GetAvailableDatesFromDb(new GetAvailableDatesInput { ActivityId = Convert.ToInt32(input.ActivityId) }).AvailableDateTimes;
            var dates = new List<DateTime?>();
            foreach(var date in dateAndSession)
            {
                dates.Add(date.Date);
            }

            if (!dates.Contains(input.DateTime.Date))
            {
                return new BookActivityOutput
                {
                    IsValid = false
                };
            }

            var sessions = dateAndSession.Where(session => session.Date == input.DateTime.Date).First().AvailableHours;
            if (!sessions.Contains(input.DateTime.Session))
            {
                return new BookActivityOutput
                {
                    IsValid = false
                };
            }
            
            getDetail.ActivityDetail.BookingStatus = BookingStatus.Booked;
            
            var rsvDetail = CreateActivityReservation(input, getDetail.ActivityDetail);
            if (rsvDetail == null)
            {
                return new BookActivityOutput
                {
                    IsValid = false
                };
            }
            InsertActivityRsvToDb(rsvDetail);
            ExpireReservationWhenTimeout(rsvDetail.RsvNo, rsvDetail.Payment.TimeLimit);
            return new BookActivityOutput
            {
                IsPriceChanged = false,
                IsValid = true,
                RsvNo = rsvDetail.RsvNo,
                TimeLimit = rsvDetail.Payment.TimeLimit
            };
        }

        public ActivityReservation CreateActivityReservation(BookActivityInput input, ActivityDetail activityInfo)
        {
            var rsvNo = RsvNoSequence.GetInstance().GetNext(ProductType.Activity);

            var env = ConfigManager.GetInstance().GetConfigValue("general", "environment");
            PlatformType platform;
            string deviceId;
            if (env == "production")
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity ?? new ClaimsIdentity();
                var clientId = identity.Claims.Single(claim => claim.Type == "Client ID").Value;
                platform = Client.GetPlatformType(clientId);
                deviceId = identity.Claims.Single(claim => claim.Type == "Device ID").Value;
            }
            else
            {
                platform = PlatformType.Undefined;
                deviceId = null;
            }

            decimal originalPriceIdr = 0;
            int allTicketCount = 0;
            var pricePackages = ActivityService.GetInstance().GetPackagePriceFromDb(input.PackageId);
            var typePricePackages = new List<string>();
            var packageAttribute = ActivityService.GetInstance().GetPackageAttributeFromDb(input.PackageId);

            foreach (var pricePackage in pricePackages)
            {
                typePricePackages.Add(pricePackage.Type);
            }
            foreach (var ticketCount in input.TicketCount)
            {
                allTicketCount += ticketCount.Count;
                if (!typePricePackages.Contains(ticketCount.Type))
                {
                    return null;
                }
                var price = pricePackages.Where(package => package.Type == ticketCount.Type).First();
                if (ticketCount.Count < price.MinCount)
                {
                    return null;
                }
                originalPriceIdr += (price.Amount * ticketCount.Count);
            }
            
            if (allTicketCount > packageAttribute[0].MaxCount || allTicketCount < packageAttribute[0].MinCount)
            {
                return null;
            }

            var rsvDetail = new ActivityReservation
            {
                RsvNo = rsvNo,
                Contact = input.Contact,
                ActivityDetails = activityInfo,
                PackageId = input.PackageId,
                TicketCount = input.TicketCount,
                DateTime = input.DateTime,
                Pax = input.Passengers,
                Payment = new PaymentDetails
                {
                    Status = PaymentStatus.Pending,
                    LocalCurrency = new Currency(OnlineContext.GetActiveCurrencyCode()),
                    OriginalPriceIdr = originalPriceIdr,
                    TimeLimit = DateTime.UtcNow.AddHours(1)
                    //TimeLimit = bookInfo.Rooms.SelectMany(r => r.Rates).Min(order => order.TimeLimit).AddMinutes(-10),
                },
                RsvStatus = RsvStatus.InProcess,
                RsvTime = DateTime.UtcNow,
                State = new ReservationState
                {
                    Platform = platform,
                    DeviceId = deviceId,
                    Language = "id", //OnlineContext.GetActiveLanguageCode();
                    Currency = new Currency("IDR"), //OnlineContext.GetActiveCurrencyCode());
                },
                User = HttpContext.Current.User.Identity.IsUserAuthorized()
                    ? HttpContext.Current.User.Identity.GetUser()
                    : null
            };

            PaymentService.GetInstance().GetUniqueCode(rsvDetail.RsvNo, null, null);

            return rsvDetail;
        }

    }
}