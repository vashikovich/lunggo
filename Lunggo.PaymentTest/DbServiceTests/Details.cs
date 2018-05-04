﻿using System;
using Lunggo.ApCommon.Payment.Constant;
using Lunggo.ApCommon.Payment.Database;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.ApCommon.Payment.Service;
using Lunggo.Framework.TestHelpers;
using Lunggo.Repository.TableRecord;
using Lunggo.Repository.TableRepository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lunggo.PaymentTest.DbServiceTests
{
    [TestClass]
    public class Details
    {
        [TestMethod]
        public void Should_convert_valid_data_from_payment_record_to_payment_details()
        {
            TestHelper.UseDb(conn =>
            {
                var expected = new PaymentTableRecord
                {
                    RsvNo = "123456789",
                    MediumCd = PaymentMediumCd.Mnemonic(PaymentMedium.Veritrans),
                    MethodCd = PaymentMethodCd.Mnemonic(PaymentMethod.BcaKlikpay),
                    SubMethod = PaymentSubmethodCd.Mnemonic(PaymentSubmethod.BCA),
                    StatusCd = PaymentStatusCd.Mnemonic(PaymentStatus.Failed),
                    Time = DateTime.Now,
                    TimeLimit = DateTime.Now,
                    TransferAccount = "1234567890",
                    RedirectionUrl = "http://234567890",
                    ExternalId = "87654321",
                    DiscountCode = "asdfghjkl",
                    OriginalPriceIdr = 1234567890,
                    DiscountNominal = 987654321,
                    Surcharge = 3456789,
                    UniqueCode = 8765432,
                    FinalPriceIdr = 876543234,
                    PaidAmountIdr = 654345,
                    LocalCurrencyCd = "USD",
                    LocalRate = 12,
                    LocalCurrencyRounding = 10,
                    LocalFinalPrice = 47384123,
                    LocalPaidAmount = 47297424,
                    InvoiceNo = "asdfg123456",
                    InsertBy = "LunggoTester",
                    InsertDate = DateTime.UtcNow,
                    InsertPgId = "0"
                };
                PaymentTableRepo.GetInstance().Delete(conn, expected);
                PaymentTableRepo.GetInstance().Insert(conn, expected);

                var actual = new PaymentDbService().GetPaymentDetails(expected.RsvNo);

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.RsvNo, actual.RsvNo);
                Assert.AreEqual(expected.MediumCd, PaymentMediumCd.Mnemonic(actual.Medium));
                Assert.AreEqual(expected.MethodCd, PaymentMethodCd.Mnemonic(actual.Method));
                Assert.AreEqual(expected.StatusCd, PaymentStatusCd.Mnemonic(actual.Status));
                Assert.AreEqual(expected.Time?.ToString("G"), actual.Time?.ToString("G"));
                Assert.AreEqual(expected.TimeLimit?.ToString("G"), actual.TimeLimit?.ToString("G"));
                Assert.AreEqual(expected.TransferAccount, actual.TransferAccount);
                Assert.AreEqual(expected.RedirectionUrl, actual.RedirectionUrl);
                Assert.AreEqual(expected.ExternalId, actual.ExternalId);
                Assert.AreEqual(expected.DiscountCode, actual.DiscountCode);
                Assert.AreEqual(expected.OriginalPriceIdr, actual.OriginalPriceIdr);
                Assert.AreEqual(expected.DiscountNominal, actual.DiscountNominal);
                Assert.AreEqual(expected.Surcharge, actual.Surcharge);
                Assert.AreEqual(expected.UniqueCode, actual.UniqueCode);
                Assert.AreEqual(expected.FinalPriceIdr, actual.FinalPriceIdr);
                Assert.AreEqual(expected.PaidAmountIdr, actual.PaidAmountIdr);
                Assert.AreEqual(expected.LocalCurrencyCd, actual.LocalCurrency.Symbol);
                Assert.AreEqual(expected.LocalRate, actual.LocalCurrency.Rate);
                Assert.AreEqual(expected.LocalCurrencyRounding, actual.LocalCurrency.RoundingOrder);
                Assert.AreEqual(expected.LocalFinalPrice, actual.LocalFinalPrice);
                Assert.AreEqual(expected.LocalPaidAmount, actual.LocalPaidAmount);
                Assert.AreEqual(expected.InvoiceNo, actual.InvoiceNo);

                PaymentTableRepo.GetInstance().Delete(conn, expected);
            });
        }

        [TestMethod]
        public void Should_return_exception_from_DB_when_rsvNo_not_found()
        {
            TestHelper.UseDb(conn =>
            {
                var rsvNo = Guid.NewGuid().ToString();
                Assert.ThrowsException<InvalidOperationException>(() => new PaymentService().GetPaymentDetails(rsvNo));
            });
        }

        [TestMethod]
        public void Should_convert_valid_data_from_payment_details_to_payment_record()
        {
            TestHelper.UseDb(conn =>
            {
                var expected = new PaymentDetails
                {
                    RsvNo = "123456789",
                    Medium = PaymentMediumCd.Mnemonic("VERI"),
                    Method = PaymentMethodCd.Mnemonic("BKP"),
                    Submethod = PaymentSubmethodCd.Mnemonic("BCA"),
                    Status = PaymentStatusCd.Mnemonic("SET"),
                    Time = DateTime.Now,
                    TimeLimit = DateTime.Now,
                    TransferAccount = "1234567890",
                    RedirectionUrl = "http://234567890",
                    ExternalId = "87654321",
                    DiscountCode = "asdfghjkl",
                    OriginalPriceIdr = 1234567890,
                    DiscountNominal = 987654321,
                    Surcharge = 3456789,
                    UniqueCode = 8765432,
                    FinalPriceIdr = 876543234,
                    PaidAmountIdr = 654345,
                    LocalCurrency = new Currency("USD", 123, 321),
                    LocalFinalPrice = 47384123,
                    LocalPaidAmount = 47297424,
                    InvoiceNo = "asdfg123456"
                };
                PaymentTableRepo.GetInstance().Delete(conn, new PaymentTableRecord { RsvNo = expected.RsvNo });

                new PaymentDbService().InsertPaymentDetails(expected);
                var actual = PaymentTableRepo.GetInstance().Find1(conn, new PaymentTableRecord { RsvNo = expected.RsvNo });

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.RsvNo, actual.RsvNo);
                Assert.AreEqual(expected.Medium, PaymentMediumCd.Mnemonic(actual.MediumCd));
                Assert.AreEqual(expected.Method, PaymentMethodCd.Mnemonic(actual.MethodCd));
                Assert.AreEqual(expected.Status, PaymentStatusCd.Mnemonic(actual.StatusCd));
                Assert.AreEqual(expected.Time?.ToString("G"), actual.Time?.ToString("G"));
                Assert.AreEqual(expected.TimeLimit?.ToString("G"), actual.TimeLimit?.ToString("G"));
                Assert.AreEqual(expected.TransferAccount, actual.TransferAccount);
                Assert.AreEqual(expected.RedirectionUrl, actual.RedirectionUrl);
                Assert.AreEqual(expected.ExternalId, actual.ExternalId);
                Assert.AreEqual(expected.DiscountCode, actual.DiscountCode);
                Assert.AreEqual(expected.OriginalPriceIdr, actual.OriginalPriceIdr);
                Assert.AreEqual(expected.DiscountNominal, actual.DiscountNominal);
                Assert.AreEqual(expected.Surcharge, actual.Surcharge);
                Assert.AreEqual(expected.UniqueCode, actual.UniqueCode);
                Assert.AreEqual(expected.FinalPriceIdr, actual.FinalPriceIdr);
                Assert.AreEqual(expected.PaidAmountIdr, actual.PaidAmountIdr);
                Assert.AreEqual(expected.LocalCurrency, new Currency(
                        actual.LocalCurrencyCd,
                        actual.LocalRate.GetValueOrDefault(),
                        actual.LocalCurrencyRounding.GetValueOrDefault()));
                Assert.AreEqual(expected.LocalFinalPrice, actual.LocalFinalPrice);
                Assert.AreEqual(expected.LocalPaidAmount, actual.LocalPaidAmount);
                Assert.AreEqual(expected.InvoiceNo, actual.InvoiceNo);
                Assert.IsNotNull(actual.InsertBy);
                Assert.IsNotNull(actual.InsertDate);
                Assert.IsNotNull(actual.InsertPgId);

                PaymentTableRepo.GetInstance().Delete(conn, new PaymentTableRecord { RsvNo = expected.RsvNo });
            });
        }
    }
}