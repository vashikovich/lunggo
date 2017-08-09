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
using Lunggo.Framework.Config;
using Lunggo.Framework.Encoder;
using Lunggo.Framework.Extension;
using Lunggo.Framework.Log;


namespace Lunggo.ApCommon.Payment.Wrapper.Nicepay
{
    internal class NicepayWrapper
    {
        private JsonResult objResult = new JsonResult();
        private NicepayModel objNicepay = new NicepayModel();
        private NicepayClass objNicepayClass = new NicepayClass();
        private Data objData = new Data();
        private NotificationResult objNoti = new NotificationResult();

        public PaymentDetails ProcessPayment(PaymentDetails payment, TransactionDetails transactionDetail, PaymentMethod method)
        {
            if (method == PaymentMethod.VirtualAccount)
            {
                objNicepay.currency = "IDR";
                objNicepay.BankCd = objNicepayClass.GetBankCode(PaymentSubMethodCd.Mnemonic(payment.SubMethod));
                objNicepay.DateNow = DateTime.Now.ToString("yyyymmdd");
                // Set VA expiry date +1 day (optional)
                objNicepay.vaExpDate = payment.TimeLimit.ToString("yyyymmdd");
                //Populate Mandatory parameters to send
                // payment type Bank
                objNicepay.PayMethod = "02";
                // Total gross amount
                objNicepay.amt = payment.FinalPriceIdr.ToString("0.#####");

                // Invoice Number or Referenc Number Generated by merchant 
                objNicepay.referenceNo = transactionDetail.OrderId;
                objNicepay.description = "Payment Invoice No. " + objNicepay.referenceNo;
                // Transaction description
                objNicepay.billingNm = "Donald Duck";
                objNicepay.billingPhone = "021987456321";
                objNicepay.billingEmail = "donald@duck.com";
                objNicepay.billingAddr = "King of money street";
                objNicepay.billingCity = "King";
                objNicepay.billingState = "Money";
                objNicepay.billingPostCd = "123654";
                objNicepay.billingCountry = "Indonesia";

                objNicepay.deliveryNm = "Donald Duck";
                objNicepay.deliveryPhone = "021987456321";
                objNicepay.deliveryEmail = "donald@duck.com";
                objNicepay.deliveryAddr = "King of money street";
                objNicepay.deliveryCity = "King";
                objNicepay.deliveryState = "Money";
                objNicepay.deliveryPostCd = "123654";
                objNicepay.deliveryCountry = "Indonesia";

                objNicepay.vacctValidDt = objNicepay.vaExpDate;
                objNicepay.vacctValidTm = DateTime.Now.ToString("hhmmss");

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

        private PaymentStatus PaymentResult(JsonResult response)
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
                        return response.resultMsg.ToLower() == "unpaid" ? PaymentStatus.Pending : PaymentStatus.Expired;
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
