﻿using System;
using System.Collections.Generic;
using System.Linq;
using Lunggo.ApCommon.Campaign.Constant;
using Lunggo.ApCommon.Campaign.Service;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Flight.Service;
using Lunggo.ApCommon.Hotel.Service;
using Lunggo.ApCommon.Payment.Constant;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.ApCommon.Payment.Query;
using Lunggo.ApCommon.Product.Constant;
using Lunggo.ApCommon.Product.Model;
using Lunggo.Framework.Database;
using Lunggo.Repository.TableRecord;
using Lunggo.Repository.TableRepository;

namespace Lunggo.ApCommon.Payment.Service
{
    public partial class PaymentService
    {
        public PaymentDetails SubmitPayment(string rsvNo, PaymentMethod method, PaymentSubMethod submethod,
            PaymentData paymentData, string discountCode, out bool isUpdated)
        {
            isUpdated = false;
            ReservationBase reservation;
            if (rsvNo.StartsWith("1"))
                reservation = FlightService.GetInstance().GetReservation(rsvNo);
            else
                reservation = HotelService.GetInstance().GetReservation(rsvNo);
            var paymentDetails = reservation.Payment;

            if (paymentDetails == null)
                return null;

            if (paymentDetails.Method != PaymentMethod.Undefined && paymentDetails.Method != PaymentMethod.BankTransfer)
            {
                paymentDetails.Status = PaymentStatus.Failed;
                paymentDetails.FailureReason = FailureReason.MethodNotAvailable;
                return paymentDetails;
            }

            paymentDetails.Data = paymentData;
            paymentDetails.Method = method;
            paymentDetails.Medium = GetPaymentMedium(method);
            paymentDetails.FinalPriceIdr = paymentDetails.OriginalPriceIdr;
            paymentDetails.SubMethod = submethod;

            if (!string.IsNullOrEmpty(discountCode))
            {
                var campaign = CampaignService.GetInstance().UseVoucherRequest(rsvNo, discountCode);
                if (campaign.VoucherStatus != VoucherStatus.Success || campaign.Discount == null)
                {
                    paymentDetails.Status = PaymentStatus.Failed;
                    paymentDetails.FailureReason = FailureReason.VoucherNoLongerEligible;
                    return paymentDetails;
                }
                paymentDetails.FinalPriceIdr = campaign.DiscountedPrice;
                paymentDetails.Discount = campaign.Discount;
                paymentDetails.DiscountCode = campaign.VoucherCode;
                paymentDetails.DiscountNominal = campaign.TotalDiscount;
            }

            if (paymentDetails.Method == PaymentMethod.CreditCard && paymentDetails.Data.CreditCard.RequestBinDiscount)
            {
                var binDiscount = CampaignService.GetInstance()
                    .CheckBinDiscount(rsvNo, paymentData.CreditCard.TokenId, paymentData.CreditCard.HashedPan,
                        discountCode);
                if (binDiscount == null)
                {
                    paymentDetails.Status = PaymentStatus.Failed;
                    paymentDetails.FailureReason = FailureReason.BinPromoNoLongerEligible;
                    return paymentDetails;
                }
                if (binDiscount.ReplaceMargin)
                {
                    if (reservation.Type == ProductType.Flight)
                    {
                        var rsv = reservation as FlightReservation;
                        foreach (var itin in rsv.Itineraries)
                        {
                            itin.Price.Margin = new UsedMargin
                            {
                                Name = "Margin Cancel",
                                Description = "Margin Cancelled by BIN Promo",
                                Currency = itin.Price.LocalCurrency
                            };
                            itin.Price.Local = itin.Price.OriginalIdr/itin.Price.LocalCurrency.Rate;
                            itin.Price.Rounding = 0;
                            itin.Price.FinalIdr = itin.Price.OriginalIdr;
                            itin.Price.MarginNominal = 0;
                        }
                        paymentDetails.OriginalPriceIdr = rsv.Itineraries.Sum(i => i.Price.FinalIdr);
                        paymentDetails.FinalPriceIdr = paymentDetails.OriginalPriceIdr - binDiscount.Amount;
                    }
                }
                else
                {
                    paymentDetails.FinalPriceIdr -= binDiscount.Amount;

                }
                if (paymentDetails.Discount == null)
                    paymentDetails.Discount = new UsedDiscount
                    {
                        DisplayName = binDiscount.DisplayName,
                        Name = binDiscount.DisplayName,
                        Constant = binDiscount.Amount,
                        Percentage = 0M,
                        Currency = new Currency("IDR"),
                        Description = "BIN Promo",
                        IsFlat = false
                    };
                else
                    paymentDetails.Discount.Constant += binDiscount.Amount;
                paymentDetails.DiscountNominal += binDiscount.Amount;
                var contact = reservation.Contact;
                if (contact == null)
                    return null;
                CampaignService.GetInstance().SavePanAndEmailInCache("btn", paymentData.CreditCard.HashedPan, contact.Email);
            }

            var transferFee = GetTransferFeeFromCache(rsvNo);
            if (transferFee == 0M)
                return paymentDetails;

            paymentDetails.UniqueCode = transferFee;
            paymentDetails.FinalPriceIdr += paymentDetails.UniqueCode;

            paymentDetails.LocalFinalPrice = paymentDetails.FinalPriceIdr * paymentDetails.LocalCurrency.Rate;
            var transactionDetails = ConstructTransactionDetails(rsvNo, paymentDetails);
            //var itemDetails = ConstructItemDetails(rsvNo, paymentDetails);
            ProcessPayment(paymentDetails, transactionDetails, method);
            if (paymentDetails.Status != PaymentStatus.Failed && paymentDetails.Status != PaymentStatus.Denied)
            {
                if (reservation.Type == ProductType.Flight)
                {
                    var rsv = reservation as FlightReservation;
                    rsv.Itineraries.ForEach(i => i.Price.UpdateToDb());
                }
                UpdatePaymentToDb(rsvNo, paymentDetails);
            }
            if (method == PaymentMethod.BankTransfer || method == PaymentMethod.VirtualAccount)
            {
                if (reservation.Type == ProductType.Flight)
                {
                    FlightService.GetInstance().SendTransferInstructionToCustomer(rsvNo);
                }
                else
                {
                    HotelService.GetInstance().SendTransferInstructionToCustomer(rsvNo);
                }
            }
                
            isUpdated = true;
            return paymentDetails;
        }

        public void UpdateBookerPaymentData(string rsvNo)
        {
            ReservationBase reservation;
            if (rsvNo.StartsWith("1"))
                reservation = FlightService.GetInstance().GetReservation(rsvNo);
            else
                reservation = HotelService.GetInstance().GetReservation(rsvNo);
            var paymentDetails = reservation.Payment;
            paymentDetails.FinalPriceIdr = paymentDetails.OriginalPriceIdr;
            UpdatePaymentToDb(rsvNo, paymentDetails);
        }

        public void ClearPayment(string rsvNo)
        {
            ClearPaymentSelection(rsvNo);
        }

        public void UpdatePayment(string rsvNo, PaymentDetails payment)
        {
            var isUpdated = UpdatePaymentToDb(rsvNo, payment);
            if (isUpdated && payment.Status == PaymentStatus.Settled)
            {
                //var service = typeof(FlightService);
                //var serviceInstance = service.GetMethod("GetInstance").Invoke(null, null);
                //service.GetMethod("Issue").Invoke(serviceInstance, new object[] { rsvNo });
                if (rsvNo.StartsWith("1"))
                    FlightService.GetInstance().Issue(rsvNo);
                if (rsvNo.StartsWith("2"))
                    HotelService.GetInstance().Issue(rsvNo);
            }
        }

        public Dictionary<string, PaymentDetails> GetUnpaids()
        {
            return GetUnpaidFromDb();
        }

        private static PaymentMedium GetPaymentMedium(PaymentMethod method)
        {
            switch (method)
            {
                case PaymentMethod.BankTransfer:
                case PaymentMethod.Credit:
                case PaymentMethod.Deposit:
                    return PaymentMedium.Direct;
                case PaymentMethod.CreditCard:
                case PaymentMethod.MandiriClickPay:
                case PaymentMethod.CimbClicks:
                case PaymentMethod.VirtualAccount:
                    return PaymentMedium.Veritrans;
                default:
                    return PaymentMedium.Undefined;
            }
        }

        private static void ProcessPayment(PaymentDetails paymentDetails, TransactionDetails transactionDetails, PaymentMethod method)
        {
            if (method == PaymentMethod.BankTransfer)
            {
                paymentDetails.Status = PaymentStatus.Pending;
            }
            else if (method == PaymentMethod.CreditCard || method == PaymentMethod.VirtualAccount || method == PaymentMethod.MandiriClickPay || method == PaymentMethod.CimbClicks || method == PaymentMethod.MandiriBillPayment)
            {
                var paymentResponse = SubmitPayment(paymentDetails, transactionDetails, method);
                if (method == PaymentMethod.VirtualAccount)
                {
                    paymentDetails.TransferAccount = paymentResponse.TransferAccount;
                }
                if (method == PaymentMethod.CimbClicks)
                {
                    paymentDetails.RedirectionUrl = paymentResponse.RedirectionUrl;
                }
                else
                {
                    paymentDetails.Status = paymentResponse.Status;
                }

            }
            else
            {
                paymentDetails.Status = PaymentStatus.Failed;
                paymentDetails.FailureReason = FailureReason.MethodNotAvailable;
            }
            paymentDetails.PaidAmountIdr = paymentDetails.FinalPriceIdr;
            paymentDetails.LocalFinalPrice = paymentDetails.FinalPriceIdr;
            paymentDetails.LocalPaidAmount = paymentDetails.FinalPriceIdr;
        }

        public List<SavedCreditCard> GetSavedCreditCards(string email)
        {
            using (var conn = DbService.GetInstance().GetOpenConnection())
            {
                var savedCards = GetSavedCreditCardByEmailQuery.GetInstance().Execute(conn, new { Email = email });
                return savedCards.ToList();
            }
        }

        public void SaveCreditCard(string email, string maskedCardNumber, string cardHolderName, string token, DateTime tokenExpiry)
        {
            //using (var conn = DbService.GetInstance().GetOpenConnection())
            //{
            //    var savedCard = GetSavedCreditCardQuery.GetInstance()
            //        .Execute(conn, new { Email = email, MaskedCardNumber = maskedCardNumber }).SingleOrDefault();
            //    if (savedCard == null)
            //        SavedCreditCardTableRepo.GetInstance().Insert(conn, new SavedCreditCardTableRecord
            //        {
            //            Email = email,
            //            MaskedCardNumber = maskedCardNumber,
            //            CardHolderName = cardHolderName,
            //            Token = token,
            //            TokenExpiry = tokenExpiry
            //        });
            //    else
            //        SavedCreditCardTableRepo.GetInstance().Update(conn, new SavedCreditCardTableRecord
            //        {
            //            Email = email,
            //            MaskedCardNumber = maskedCardNumber,
            //            Token = token,
            //            TokenExpiry = tokenExpiry
            //        });
            //}
        }

        private static PaymentDetails SubmitPayment(PaymentDetails payment, TransactionDetails transactionDetails, PaymentMethod method)
        {
            var paymentResponse = VeritransWrapper.ProcessPayment(payment, transactionDetails, method);
            return paymentResponse;
        }

        private static string GetThirdPartyPaymentUrl(TransactionDetails transactionDetails, List<ItemDetails> itemDetails, PaymentMethod method)
        {
            var url = VeritransWrapper.GetPaymentUrl(transactionDetails, itemDetails, method);
            return url;
        }

        private static List<ItemDetails> ConstructItemDetails(string rsvNo, PaymentDetails payment)
        {
            var itemDetails = new List<ItemDetails>();
            //var trips = reservation.Itineraries.SelectMany(itin => itin.Trips).ToList();
            //var itemNameBuilder = new StringBuilder();
            //foreach (var trip in trips)
            //{
            //    itemNameBuilder.Append(trip.OriginAirport + "-" + trip.DestinationAirport);
            //    itemNameBuilder.Append(" " + trip.DepartureDate.ToString("dd-MM-yyyy"));
            //    if (trip != trips.Last())
            //    {
            //        itemNameBuilder.Append(", ");
            //    }
            //}
            //var itemName = itemNameBuilder.ToString();
            itemDetails.Add(new ItemDetails
            {
                Id = "1",
                Name = ProductTypeCd.Parse(rsvNo).ToString(),
                Price = (long) payment.FinalPriceIdr,
                Quantity = 1
            });
            //if (payment.Discount != null)
            //    itemDetails.Add(new ItemDetails
            //    {
            //        Id = "2",
            //        Name = "Discount",
            //        Price = (long)-payment.DiscountNominal,
            //        Quantity = 1
            //    });
            //if (payment.UniqueCode != 0)
            //    itemDetails.Add(new ItemDetails
            //    {
            //        Id = "3",
            //        Name = "TransferFee",
            //        Price = (long)payment.UniqueCode,
            //        Quantity = 1
            //    });
            return itemDetails;
        }

        private static TransactionDetails ConstructTransactionDetails(string rsvNo, PaymentDetails payment)
        {
            return new TransactionDetails
            {
                OrderId = rsvNo,
                OrderTime = DateTime.UtcNow,
                Amount = (long)payment.FinalPriceIdr
            };
        }

        public decimal GetUniqueCode(string rsvNo, string bin, string discountCode)
        {
            decimal uniqueCode, finalPrice;
            var voucher = CampaignService.GetInstance().ValidateVoucherRequest(rsvNo, discountCode);
            if (voucher.VoucherStatus != VoucherStatus.Success)
            {
                var payment = PaymentDetails.GetFromDb(rsvNo);

                if (payment == null)
                    return 404404404.404404404M;

                finalPrice = payment.OriginalPriceIdr;
            }
            else
            {
                finalPrice = voucher.DiscountedPrice;
            }
            if (finalPrice <= 999 && finalPrice >= 0)
            {
                uniqueCode = -finalPrice;
            }
            else
            {
                bool isExist;
                decimal candidatePrice;
                var rnd = new Random();
                uniqueCode = GetTransferFeeFromCache(rsvNo);
                if (uniqueCode != 0M)
                {
                    candidatePrice = finalPrice + uniqueCode;
                    var rsvNoHavingTransferValue = GetRsvNoHavingTransferValue(candidatePrice);
                    isExist = rsvNoHavingTransferValue != null && rsvNoHavingTransferValue != rsvNo;
                    if (isExist)
                    {
                        var cap = -1;
                        do
                        {
                            if (cap < 999)
                                cap += 50;
                            uniqueCode = -rnd.Next(1, cap);
                            candidatePrice = finalPrice + uniqueCode;
                            rsvNoHavingTransferValue = GetRsvNoHavingTransferValue(candidatePrice);
                            isExist = rsvNoHavingTransferValue != null && rsvNoHavingTransferValue != rsvNo;
                        } while (isExist);
                    }
                    SaveTransferValue(candidatePrice, rsvNo);

                    SaveTransferFeeinCache(rsvNo, uniqueCode);
                }
                else
                {
                    var cap = -1;
                    do
                    {
                        if (cap < 999)
                            cap += 50;
                        uniqueCode = -rnd.Next(1, cap);
                        candidatePrice = finalPrice + uniqueCode;
                        var rsvNoHavingTransferValue = GetRsvNoHavingTransferValue(candidatePrice);
                        isExist = rsvNoHavingTransferValue != null && rsvNoHavingTransferValue != rsvNo;
                    } while (isExist);
                    SaveTransferValue(candidatePrice, rsvNo);
                    SaveTransferFeeinCache(rsvNo, uniqueCode);
                }
            }

            return uniqueCode;
        }
    }
}
