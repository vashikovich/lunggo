﻿using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Lunggo.ApCommon.Constant;
using Lunggo.ApCommon.Product.Model;
using Lunggo.Framework.Database;
using Lunggo.Framework.Encoder;
using Lunggo.Framework.Redis;
using Lunggo.Repository.TableRecord;
using Lunggo.Repository.TableRepository;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.ApCommon.Identity.Users;
using Lunggo.ApCommon.Activity.Service;
using Lunggo.ApCommon.Activity.Database.Query;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Service;
using Lunggo.ApCommon.Hotel.Service;
using Lunggo.ApCommon.Identity.Auth;
using Lunggo.ApCommon.Payment.Constant;
using Lunggo.ApCommon.Product.Constant;

namespace Lunggo.ApCommon.Payment.Service
{
    public partial class PaymentService
    {
        public CampaignVoucher GetCampaignVoucher(string voucherCode)
        {
            return _db.GetCampaignVoucher(voucherCode);
        }
        public VoucherResponse ValidateVoucherRequest(string trxId, string voucherCode, out VoucherStatus status)
        {
            var response = new VoucherResponse();
            var isRsv = trxId.Length < 15;
            var cart = new Cart();
            if (!isRsv)
            {
                cart = GetCart(trxId);
                if (cart == null || cart.RsvNoList == null || !cart.RsvNoList.Any())
                {
                    status = VoucherStatus.InternalError;
                    return response;
                }
            }
            
            var voucher = _db.GetCampaignVoucher(voucherCode);

            if (voucher == null)
            {
                status = VoucherStatus.VoucherNotFound;
                return response;
            }

            if (isRsv && voucher.ProductType != null && !voucher.ProductType.Contains(trxId[0]))
            {
                status = VoucherStatus.TermsConditionsNotEligible;
                return response;
            }

            var contact = isRsv ? Contact.GetFromDb(trxId) : Contact.GetFromDb(cart.RsvNoList[0]);
            if (contact == null)
            {
                status = VoucherStatus.InternalError;
                return response;
            }

            //if (!_cache.IsPhoneAndEmailEligibleInCache(voucherCode, contact.CountryCallingCode + contact.Phone, contact.Email))
            //{
            //    status = VoucherStatus.EmailNotEligible;
            //    return response;
            //}

            var paymentDetails = GetPaymentDetails(trxId);
            if (paymentDetails == null)
            {
                paymentDetails = GetPaymentDetails(trxId);
                if (paymentDetails == null)
                {
                    status = VoucherStatus.InternalError;
                    return response;
                }
            }

            var price = paymentDetails.OriginalPriceIdr * paymentDetails.LocalCurrency.Rate;

            var validationStatus = ValidateVoucher(voucher, price, voucherCode);

            if (validationStatus == VoucherStatus.Success)
            {
                CalculateVoucherDiscount(voucher, price, response);
                response.Discount = new UsedDiscount
                {
                    Name = voucher.CampaignName,
                    Description = voucher.CampaignDescription,
                    DisplayName = voucher.DisplayName,
                    Percentage = voucher.ValuePercentage.GetValueOrDefault(),
                    Constant = voucher.ValueConstant.GetValueOrDefault(),
                    Currency = new Currency("IDR"),
                    IsFlat = false
                };
            }

            //ReservationBase rsv;
            //if (rsvNo.StartsWith("1"))
            //    rsv = FlightService.GetInstance().GetReservation(rsvNo);
            //else
            //    rsv = HotelService.GetInstance().GetReservation(rsvNo);

            if (isRsv)
            {
                ReservationBase rsv;
                if (trxId.StartsWith("1"))
                    rsv = FlightService.GetInstance().GetReservation(trxId);
                else if (trxId.StartsWith("2"))
                    rsv = HotelService.GetInstance().GetReservation(trxId);
                else
                    rsv = ActivityService.GetInstance().GetReservation(trxId);

                var cost = rsv.GetTotalSupplierPrice();
                if (voucher.MaxBudget.HasValue &&
                    (voucher.MaxBudget - voucher.UsedBudget < cost - paymentDetails.FinalPriceIdr * 0.97M))
                {
                    status = VoucherStatus.VoucherDepleted;
                    return response;
                }

                //////////////  HARDCODED VALIDATION /////////////////

                if (voucher.CampaignId == 66 || voucher.CampaignName == "Good Monday") // Good Monday
                {
                    var valid = true;
                    status = VoucherStatus.InternalError;

                    var identity = HttpContext.Current.User.Identity as ClaimsIdentity ?? new ClaimsIdentity();
                    var clientId = identity.Claims.Single(claim => claim.Type == "Client ID").Value;
                    var platform = Client.GetPlatformType(clientId);
                    if (platform == PlatformType.AndroidApp || platform == PlatformType.IosApp)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if ((rsv as FlightReservation).Itineraries.SelectMany(i => i.Trips)
                        .Any(t =>
                            FlightService.GetInstance().GetAirportCountryCode(t.OriginAirport) != "ID" ||
                            FlightService.GetInstance().GetAirportCountryCode(t.DestinationAirport) != "ID"))
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (!(new[] { "JKT", "CGK", "HLP", "SRG", "JOG", "TNJ", "SUB" }
                        .Contains((rsv as FlightReservation).Itineraries[0].Trips[0].DestinationAirport)))
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    var airlines =
                        (rsv as FlightReservation).Itineraries.SelectMany(i => i.Trips)
                            .SelectMany(t => t.Segments)
                            .Select(s => s.AirlineCode);
                    foreach (var airline in airlines)
                    {
                        if (!(new[] { "QG", "SJ", "IN", "ID" }.Contains(airline)))
                        {
                            status = VoucherStatus.TermsConditionsNotEligible;
                            valid = false;
                        }
                    }

                    if (DateTime.UtcNow.AddHours(7).DayOfWeek != DayOfWeek.Monday)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (!valid)
                    {
                        return response;
                    }
                }

                if (voucher.CampaignId == 71 || voucher.CampaignName == "Selasa Spesial") // Selasa Spesial
                {
                    var valid = true;
                    status = VoucherStatus.InternalError;

                    if (DateTime.UtcNow.AddHours(7).DayOfWeek != DayOfWeek.Tuesday)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (!valid)
                    {
                        return response;
                    }
                }

                if (voucher.CampaignId == 67 || voucher.CampaignName == "Promo Rabu") // Promo Rabu
                {
                    var valid = true;
                    status = VoucherStatus.InternalError;

                    var identity = HttpContext.Current.User.Identity as ClaimsIdentity ?? new ClaimsIdentity();
                    var clientId = identity.Claims.Single(claim => claim.Type == "Client ID").Value;
                    var platform = Client.GetPlatformType(clientId);
                    if (platform == PlatformType.AndroidApp || platform == PlatformType.IosApp)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if ((rsv as FlightReservation).Itineraries.SelectMany(i => i.Trips)
                        .Any(t =>
                            FlightService.GetInstance().GetAirportCountryCode(t.OriginAirport) != "ID" ||
                            FlightService.GetInstance().GetAirportCountryCode(t.DestinationAirport) != "ID"))
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    var airlines =
                        (rsv as FlightReservation).Itineraries.SelectMany(i => i.Trips)
                            .SelectMany(t => t.Segments)
                            .Select(s => s.AirlineCode);
                    foreach (var airline in airlines)
                    {
                        if (!(new[] { "SJ", "IN" }.Contains(airline)))
                        {
                            status = VoucherStatus.TermsConditionsNotEligible;
                            valid = false;
                        }
                    }

                    if (paymentDetails.Method != PaymentMethod.BankTransfer &&
                        paymentDetails.Method != PaymentMethod.VirtualAccount &&
                        paymentDetails.Method != PaymentMethod.Undefined)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }

                    if (DateTime.UtcNow.AddHours(7).DayOfWeek != DayOfWeek.Wednesday)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (!valid)
                    {
                        return response;
                    }
                }

                if (voucher.CampaignId == 68 || voucher.CampaignName == "Kamis Ceria") // Kamis Ceria
                {
                    var valid = true;
                    status = VoucherStatus.InternalError;

                    var identity = HttpContext.Current.User.Identity as ClaimsIdentity ?? new ClaimsIdentity();
                    var clientId = identity.Claims.Single(claim => claim.Type == "Client ID").Value;
                    var platform = Client.GetPlatformType(clientId);
                    if (platform == PlatformType.AndroidApp || platform == PlatformType.IosApp)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if ((rsv as FlightReservation).Itineraries.SelectMany(i => i.Trips)
                        .Any(t =>
                            FlightService.GetInstance().GetAirportCountryCode(t.OriginAirport) != "ID" ||
                            FlightService.GetInstance().GetAirportCountryCode(t.DestinationAirport) != "ID"))
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (!(new[] { "DPS", "LOP", "LBJ", "BTH", "BTJ" }
                        .Contains((rsv as FlightReservation).Itineraries[0].Trips[0].DestinationAirport)))
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    var airlines =
                        (rsv as FlightReservation).Itineraries.SelectMany(i => i.Trips)
                            .SelectMany(t => t.Segments)
                            .Select(s => s.AirlineCode);
                    foreach (var airline in airlines)
                    {
                        if (!(new[] { "QG", "SJ", "IN", "ID" }.Contains(airline)))
                        {
                            status = VoucherStatus.TermsConditionsNotEligible;
                            valid = false;
                        }
                    }


                    if (DateTime.UtcNow.AddHours(7).DayOfWeek != DayOfWeek.Thursday)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (!valid)
                    {
                        return response;
                    }
                }

                if (voucher.CampaignId == 72 || voucher.CampaignName == "Jumat Hemat") // Jumat Hemat
                {
                    var valid = true;
                    status = VoucherStatus.InternalError;

                    var identity = HttpContext.Current.User.Identity as ClaimsIdentity ?? new ClaimsIdentity();
                    var userId = identity.Name == "anonymous" ? null : identity.GetUser().Id;
                    var userEmail = identity.Name == "anonymous" ? null : identity.GetEmail();
                    var rsvs1 = FlightService.GetInstance()
                        .GetOverviewReservationsByUserIdOrEmail(userId, contact.Email, null, null, null, null)
                        .Where(r => r.Payment.Status == PaymentStatus.Settled);
                    var rsvs2 = FlightService.GetInstance()
                        .GetOverviewReservationsByUserIdOrEmail(userId, userEmail, null, null, null, null)
                        .Where(r => r.Payment.Status == PaymentStatus.Settled);
                    if (!rsvs1.Concat(rsvs2).Any())
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }

                    if (DateTime.UtcNow.AddHours(7).DayOfWeek != DayOfWeek.Friday)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }

                    if (!valid)
                    {
                        return response;
                    }
                }

                if (voucher.CampaignId == 69 || voucher.CampaignName == "Jalan-Jalan Sabtu") // Jalan-Jalan Sabtu
                {
                    var valid = true;
                    status = VoucherStatus.InternalError;

                    var identity = HttpContext.Current.User.Identity as ClaimsIdentity ?? new ClaimsIdentity();
                    var clientId = identity.Claims.Single(claim => claim.Type == "Client ID").Value;
                    var platform = Client.GetPlatformType(clientId);
                    if (platform == PlatformType.AndroidApp || platform == PlatformType.IosApp)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if ((rsv as FlightReservation).Itineraries.SelectMany(i => i.Trips)
                        .Any(t =>
                            FlightService.GetInstance().GetAirportCountryCode(t.OriginAirport) != "ID" ||
                            FlightService.GetInstance().GetAirportCountryCode(t.DestinationAirport) != "ID"))
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    var airlines =
                        (rsv as FlightReservation).Itineraries.SelectMany(i => i.Trips)
                            .SelectMany(t => t.Segments)
                            .Select(s => s.AirlineCode);
                    foreach (var airline in airlines)
                    {
                        if (!(new[] { "QG", "SJ", "IN", "ID" }.Contains(airline)))
                        {
                            status = VoucherStatus.TermsConditionsNotEligible;
                            valid = false;
                        }
                    }


                    if (paymentDetails.Method != PaymentMethod.BankTransfer &&
                        paymentDetails.Method != PaymentMethod.VirtualAccount &&
                        paymentDetails.Method != PaymentMethod.Undefined)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (DateTime.UtcNow.AddHours(7).DayOfWeek != DayOfWeek.Saturday)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (!valid)
                    {
                        return response;
                    }
                }

                if (voucher.CampaignId == 73 || voucher.CampaignName == "Sunday Funday") // Sunday Funday
                {
                    var valid = true;
                    status = VoucherStatus.InternalError;

                    if (DateTime.UtcNow.AddHours(7).DayOfWeek != DayOfWeek.Sunday)
                    {
                        status = VoucherStatus.TermsConditionsNotEligible;
                        valid = false;
                    }


                    if (!valid)
                    {
                        return response;
                    }
                }

                //////////////  HARDCODED VALIDATION /////////////////

            }

            status = validationStatus;
            return response;
        }

        public VoucherResponse UseVoucherRequest(string rsvNo, string voucherCode, out VoucherStatus status)
        {
            var response = ValidateVoucherRequest(rsvNo, voucherCode, out status);
            if (status == VoucherStatus.Success)
            {
                var isUseBudgetSuccess = !rsvNo.StartsWith("2") || UseHotelBudget(voucherCode, rsvNo);
                var isVoucherDecrementSuccess = VoucherDecrement(voucherCode);
                if (isUseBudgetSuccess && isVoucherDecrementSuccess)
                {
                    status = VoucherStatus.Success;
                    //var contact = Contact.GetFromDb(rsvNo);
                    //SavePhoneAndEmailInCache(voucherCode, contact.CountryCallingCode + contact.Phone, contact.Email);
                }
                else
                    status = VoucherStatus.InternalError;
            }
            return response;
        }

        private VoucherStatus ValidateVoucher(CampaignVoucher voucher, decimal price, string voucherCode)
        {
            var currentTime = DateTime.Now;
            if (voucher == null)
                return VoucherStatus.VoucherNotFound;
            if (voucher.StartDate >= currentTime)
                return VoucherStatus.OutsidePeriod;
            if (voucher.EndDate <= currentTime)
                return VoucherStatus.OutsidePeriod;
            if (voucher.RemainingCount < 1)
                return VoucherStatus.VoucherDepleted;
            if (voucher.MinSpendValue > price)
                return VoucherStatus.BelowMinimumSpend;
            //if (voucher.CampaignTypeCd == CampaignTypeCd.Mnemonic(CampaignType.Member))
            //{
            //    if (!_db.IsMember(email))
            //        return VoucherStatus.EmailNotEligible;
            //}
            //if (voucher.CampaignTypeCd == CampaignTypeCd.Mnemonic(CampaignType.Private))
            //{
            //    if (!_db.IsEligibleForVoucher(voucherCode, email))
            //        return VoucherStatus.EmailNotEligible;
            //}
            if (voucher.CampaignStatus == false)
                return VoucherStatus.OutsidePeriod;
            //if (voucher.IsSingleUsage != null && voucher.IsSingleUsage == true)
            //{
            //    if (_db.CheckVoucherUsage(voucherCode, email) > 0)
            //        return VoucherStatus.VoucherAlreadyUsed;
            //}
            return VoucherStatus.Success;
        }

        private void CalculateVoucherDiscount(CampaignVoucher voucher, decimal price, VoucherResponse response)
        {
            response.TotalDiscount = 0;

            if (voucher.ValuePercentage != null && voucher.ValuePercentage > 0)
                response.TotalDiscount += (price * (decimal)voucher.ValuePercentage / 100M);

            if (voucher.ValueConstant != null && voucher.ValueConstant > 0)
                response.TotalDiscount += (decimal)voucher.ValueConstant;

            if (voucher.MaxDiscountValue != null && voucher.MaxDiscountValue > 0
                && response.TotalDiscount > voucher.MaxDiscountValue)
                response.TotalDiscount = (decimal)voucher.MaxDiscountValue;

            response.TotalDiscount = Math.Floor(response.TotalDiscount);

            response.DiscountedPrice = price - response.TotalDiscount;
            if (response.DiscountedPrice < 50000)
            {
                response.DiscountedPrice = 50000M;
            }
        }

        private bool VoucherDecrement(string voucherCode)
        {
            try
            {
                return _db.VoucherDecrement(voucherCode);
            }
            catch (Exception)
            {
                return false;
            }
        }


        private bool VoucherIncrement(string voucherCode)
        {
            try
            {
                return _db.VoucherIncrement(voucherCode);
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool UseHotelBudget(string voucherCode, string rsvNo)
        {
            try
            {
                return _db.UseHotelBudget(voucherCode, rsvNo);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}

