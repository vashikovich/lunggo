﻿using Lunggo.ApCommon.Payment.Constant;
using Lunggo.ApCommon.Payment.Model.Data;
using Lunggo.ApCommon.Payment.Service;
using Lunggo.Framework.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentData = Lunggo.ApCommon.Payment.Model.PaymentData;

namespace Lunggo.PaymentTest.Units.SubmitPayment
{
    [TestClass]
    public partial class SubmitPaymentTest
    {
        [TestMethod]
        public void Should_failed_when_method_not_selected()
        {
            var method = PaymentMethod.Undefined;
            var submethod = PaymentSubmethod.Undefined;
            PaymentData paymentData = null;
            var result = new PaymentService().InvokePrivate<bool>("ValidatePaymentMethod", method, submethod, paymentData);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Should_failed_when_method_is_Bank_Transfer_but_submethod_is_not_selected()
        {
            var method = PaymentMethod.BankTransfer;
            var submethod = PaymentSubmethod.Undefined;
            PaymentData paymentData = null;
            var result = new PaymentService().InvokePrivate<bool>("ValidatePaymentMethod", method, submethod, paymentData);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Should_failed_when_method_is_Virtual_Account_but_submethod_is_not_selected()
        {
            var method = PaymentMethod.VirtualAccount;
            var submethod = PaymentSubmethod.Undefined;
            PaymentData paymentData = null;
            var result = new PaymentService().InvokePrivate<bool>("ValidatePaymentMethod", method, submethod, paymentData);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Should_failed_when_Credit_Card_does_not_include_Credit_Card_data()
        {
            var method = PaymentMethod.CreditCard;
            var submethod = PaymentSubmethod.Undefined;
            var paymentData = new PaymentData
            {
                CreditCard = null
            };
            var result = new PaymentService().InvokePrivate<bool>("ValidatePaymentMethod", method, submethod, paymentData);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Should_failed_when_Credit_Card_does_not_include_token()
        {
            var method = PaymentMethod.CreditCard;
            var submethod = PaymentSubmethod.Undefined;
            var paymentData = new PaymentData
            {
                CreditCard = new ApCommon.Payment.Model.Data.CreditCard
                {
                    TokenId = null
                }
            };
            var result = new PaymentService().InvokePrivate<bool>("ValidatePaymentMethod", method, submethod, paymentData);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Should_failed_when_Mandiri_Clickpay_does_not_include_Mandiri_Clickpay_data()
        {
            var method = PaymentMethod.MandiriClickPay;
            var submethod = PaymentSubmethod.Undefined;
            var paymentData = new PaymentData
            {
                MandiriClickPay = null
            };
            var result = new PaymentService().InvokePrivate<bool>("ValidatePaymentMethod", method, submethod, paymentData);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Should_failed_when_Mandiri_Clickpay_does_not_include_card_number()
        {
            var method = PaymentMethod.MandiriClickPay;
            var submethod = PaymentSubmethod.Undefined;
            var paymentData = new PaymentData
            {
                MandiriClickPay = new MandiriClickPay
                {
                    CardNumber = null,
                    Token = "123456"
                }
            };
            var result = new PaymentService().InvokePrivate<bool>("ValidatePaymentMethod", method, submethod, paymentData);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Should_failed_when_Mandiri_Clickpay_does_not_include_token()
        {
            var method = PaymentMethod.MandiriClickPay;
            var submethod = PaymentSubmethod.Undefined;
            var paymentData = new PaymentData
            {
                MandiriClickPay = new MandiriClickPay
                {
                    CardNumber = "1234567890123456",
                    Token = null
                }
            };
            var result = new PaymentService().InvokePrivate<bool>("ValidatePaymentMethod", method, submethod, paymentData);
            Assert.IsFalse(result);
        }



        ////[TestMethod]
        ////public void Should_return_invalid_voucher_when_voucher_is_not_valid()
        ////{
        ////    var voucherCode = '1234567890';
        ////    var expectedResult = false;
        ////    bool actualResult = JalaninFungsiApalahNamanya(voucherCode);
        ////    Assert.AreEqual(expectedResult, actualResult.Status);
        ////}

        ////[TestMethod]
        ////public void Should_not_return_failed_when_voucher_is_valid()
        ////{
        ////    var voucherCode = '1234567890';
        ////    bool actualResult = JalaninFungsiApalahNamanya(voucherCode);
        ////    Assert.AreNotEqual('Failed', actualResult.Status);
        ////}

        //[TestMethod]
        //public void Should_return_pending_on_successful_Bank_Transfer_Mandiri()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_success_on_valid_Credit_Card_Visa()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.CreditCard,
        //        Status = PaymentStatus.MethodNotSet,
        //        Data = new PaymentData(),
        //    };
        //    var actualResult = JalaninFungsiApalahNamanya(p);
        //    p.Status = PaymentStatus.Settled;
        //    var expectedResult = p;
        //    Assert.AreEqual(expectedResult, actualResult);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_denied_Credit_Card_Visa()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_success_on_valid_Credit_Card_Mastercard()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_denied_Credit_Card_Mastercard()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_CIMB_Clicks()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_CIMB_Clicks()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_Virtual_Account_BCA()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_Virtual_Account_BCA()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_BCA_KlikPay()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_BCA_KlikPay()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_Virtual_Account_Bank_BRI()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_Virtual_Account_Bank_BRI()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_Virtual_Account_Danamon()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_Virtual_Account_Danamon()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_Virtual_Account_CIMB_NIAGA()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_Virtual_Account_CIMB_NIAGA()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_Virtual_Account_Permata_Bank()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_Virtual_Account_Permata_Bank()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_Virtual_Account_Others()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_Virtual_Account_Others()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_Mandiri_ClickPay()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_Mandiri_ClickPay()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_BTN_Mobile_Banking()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_BTN_Mobile_Banking()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_IB_Muamalat()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_failed_on_unsuccessful_IB_Muamalat()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}

        //[TestMethod]
        //public void Should_return_pending_on_successful_SBII_Online_Shopping()
        //{
        //    var p = new PaymentDetails()
        //    {
        //        Medium = PaymentMedium.Direct,
        //        Method = PaymentMethod.BankTransfer,
        //        Submethod = PaymentSubmethod.Mandiri,
        //    };
        //    PaymentDetails actualResult = JalaninFungsiApalahNamanya(p);
        //    Assert.AreEqual(PaymentStatus.Settled, actualResult.Status);
        //}
    }
}
