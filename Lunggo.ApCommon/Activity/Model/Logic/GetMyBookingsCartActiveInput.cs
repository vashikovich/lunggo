﻿using Lunggo.ApCommon.Product.Constant;
using Lunggo.ApCommon.Product.Model;
using System;

namespace Lunggo.ApCommon.Activity.Model.Logic
{
    public class GetMyBookingsCartActiveInput : ReservationBase
    {
        public override ProductType Type
        {
            get { return ProductType.Activity; }
        }

        public DateTime LastUpdate { get; set; }

        public override decimal GetTotalSupplierPrice()
        {
            throw new NotImplementedException();
        }
    }
}