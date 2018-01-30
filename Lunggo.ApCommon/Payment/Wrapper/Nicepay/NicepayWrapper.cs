﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Lunggo.ApCommon.Identity.Auth;
using Lunggo.ApCommon.Identity.Users;
using Lunggo.ApCommon.Payment.Constant;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.ApCommon.Payment.Wrapper.Nicepay;
using Lunggo.ApCommon.Product.Model;
using Lunggo.Framework.Config;
using Lunggo.Framework.Encoder;
using Lunggo.Framework.Extension;
using Lunggo.Framework.Log;


namespace Lunggo.ApCommon.Payment.Wrapper.Nicepay
{
    internal partial class NicepayWrapper
    {
        private static readonly NicepayWrapper Instance = new NicepayWrapper();
        private bool _isInitialized;

        internal static string _endpoint;
        internal static string _merchantId;
        internal static string _merchantKey;

        private NicepayResponse objResult = new NicepayResponse();
        private NicepayModel objNicepay = new NicepayModel();
        private NicepayClass objNicepayClass = new NicepayClass();
        private Data objData = new Data();
        private NotificationResult objNoti = new NotificationResult();

        private NicepayWrapper()
        {

        }

        internal static NicepayWrapper GetInstance()
        {
            return Instance;
        }

        internal void Init()
        {
            if (!_isInitialized)
            {
                _endpoint = ConfigManager.GetInstance().GetConfigValue("nicepay", "endPoint");
                _merchantId = ConfigManager.GetInstance().GetConfigValue("nicepay", "merchantId");
                _merchantKey = ConfigManager.GetInstance().GetConfigValue("nicepay", "merchantKey");
                _isInitialized = true;
            }
        }

        internal PaymentDetails ProcessPayment(PaymentDetails payment, TransactionDetails transactionDetail)
        {
            if (payment.Method == PaymentMethod.VirtualAccount)
            {
                objNicepay.currency = "IDR";
                objNicepay.BankCd = objNicepayClass.GetBankCode(payment.Submethod);
                objNicepay.DateNow = DateTime.UtcNow.AddHours(7).ToString("yyyyMMdd");
                // Set VA expiry date +1 day (optional)
                objNicepay.vaExpDate = payment.TimeLimit.AddHours(7).ToString("yyyyMMdd");
                //Populate Mandatory parameters to send
                // payment type Bank
                objNicepay.PayMethod = "02"; //
                // Total gross amount
                objNicepay.amt = payment.FinalPriceIdr.ToString("0.#####");//

                // Invoice Number or Referenc Number Generated by merchant 
                objNicepay.referenceNo = transactionDetail.RsvNo;//
                objNicepay.description = "Payment Invoice No. " + objNicepay.referenceNo;//
                // Transaction description

                var contact = transactionDetail.Contact;

                objNicepay.billingNm = contact.Name;//
                objNicepay.billingPhone = contact.CountryCallingCode + contact.Phone;//
                objNicepay.billingEmail = contact.Email;//
                //objNicepay.billingAddr = "Jalan Sisingamangaraja";
                //objNicepay.billingCity = "Jakarta Selatan";
                //objNicepay.billingState = "Jakarta";
                //objNicepay.billingPostCd = "12110";
                //objNicepay.billingCountry = "Indonesia";

                //objNicepay.deliveryNm = "Dwi Agustina";
                //objNicepay.deliveryPhone = "0811351793";
                //objNicepay.deliveryEmail = "dwi.agustina@travelmadezy.com";
                //objNicepay.deliveryAddr = "Jalan Sisingamangaraja";
                //objNicepay.deliveryCity = "Jakarta Selatan";
                //objNicepay.deliveryState = "Jakarta";
                //objNicepay.deliveryPostCd = "12110";
                //objNicepay.deliveryCountry = "Indonesia";

                objNicepay.vacctValidDt = objNicepay.vaExpDate;
                objNicepay.vacctValidTm = payment.TimeLimit.AddHours(7).ToString("HHmmss");

                objResult = objNicepayClass.CreateVA(objNicepay);

                if (objResult.resultCd == "0000")
                {
                    payment.TransferAccount = objResult.bankVacctNo;
                    payment.Status = PaymentResult(objResult);
                    payment.ExternalId = objResult.tXid;
                }
                else if (objResult.resultCd != null)
                {
                    //API data Not correct, you can redirect back to checkout page Or echo error message.
                    //In this sample, we echo error message
                    JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();

                    payment.Status = PaymentStatus.Failed;
                    payment.FailureReason = FailureReason.PaymentFailure;

                    var log = LogService.GetInstance();
                    var env = ConfigManager.GetInstance().GetConfigValue("general", "environment");
                    log.Post(
                        "```Payment Log```"
                        + "\n`*Environment :* " + env.ToUpper()
                        + "\n*PAYMENT DETAILS :*\n"
                        + payment.Serialize()
                            + "\n*TRANSAC DETAILS :*\n"
                            + transactionDetail.Serialize()
                        //+ "\n*ITEM DETAILS :*\n"
                        //+ itemDetails.Serialize()
                            + "\n*REQUEST :*\n"
                            + JsonSerializer.Serialize(objNicepay)
                        + "\n*RESPONSE :*\n"
                        + JsonSerializer.Serialize(objResult)
                        + "\n*Platform :* "
                        + Client.GetPlatformType(HttpContext.Current.User.Identity.GetClientId()),
                        env == "production" ? "#logging-prod" : "#logging-dev");
                }
                else
                {
                    //Timeout, you can redirect back to checkout page Or echo error message.
                    //In this sample, we echo error message
                    JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();

                    payment.Status = PaymentStatus.Failed;
                    payment.FailureReason = FailureReason.PaymentFailure;

                    var log = LogService.GetInstance();
                    var env = ConfigManager.GetInstance().GetConfigValue("general", "environment");
                    log.Post(
                        "```Payment Log, Connection Time out to Nicepay```"
                        + "\n`*Environment :* " + env.ToUpper()
                        + "\n*PAYMENT DETAILS :*\n"
                        + payment.Serialize()
                            + "\n*TRANSAC DETAILS :*\n"
                            + transactionDetail.Serialize()
                        //+ "\n*ITEM DETAILS :*\n"
                        //+ itemDetails.Serialize()
                            + "\n*REQUEST :*\n"
                            + JsonSerializer.Serialize(objNicepay)
                        + "\n*RESPONSE :*\n"
                        + JsonSerializer.Serialize(objResult)
                        + "\n*Platform :* "
                        + Client.GetPlatformType(HttpContext.Current.User.Identity.GetClientId()),
                        env == "production" ? "#logging-prod" : "#logging-dev");
                }

            }
            return payment;
        }

        private PaymentStatus PaymentResult(NicepayResponse response)
        {
            var paymentStatusResponse = objNicepayClass.ChargcheckPaymentStatuseCard(response.tXid,
                        response.referenceNo, response.amount);
            if (paymentStatusResponse.resultCd == "0000")
            {
                switch (paymentStatusResponse.status)
                {
                    case "0":
                        return PaymentStatus.Settled; //Paid
                    case "1":
                        return PaymentStatus.Challenged; //Reversal
                    case "2":
                        return PaymentStatus.Challenged; // Refund
                    case "3": // Unpaid/Expired
                        return paymentStatusResponse.resultMsg.ToLower().Contains("unpaid") ? PaymentStatus.Pending : PaymentStatus.Expired;
                    case "4":
                        return PaymentStatus.Cancelled; //Cancelled
                    case "9":
                        return PaymentStatus.Pending; //Initialization
                    default:
                        return PaymentStatus.Failed;
                }
            }
            else
            return PaymentStatus.Pending;

        }
    }
}
