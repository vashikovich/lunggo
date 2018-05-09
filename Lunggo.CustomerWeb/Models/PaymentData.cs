﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lunggo.ApCommon.Flight.Model;
using Lunggo.ApCommon.Hotel.Model;
using Lunggo.ApCommon.Payment.Model;
using Lunggo.ApCommon.Product.Model;

namespace Lunggo.CustomerWeb.Models
{
    public class PaymentData
    {
        public string TrxId { get; set; }
        //public ReservationForDisplayBase Reservation { get; set; }
        //public RsvPaymentDetails RsvPaymentDetails { get; set; }
        public decimal OriginalPrice { get; set; }
        public DateTime TimeLimit { get; set; }
        public List<SavedCreditCard> SavedCreditCards { get; set; }
    }
}